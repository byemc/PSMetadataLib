using System.ComponentModel;
using System.Text;
using PSMetadataLib;
using PSMetadataLib.PS2.Enums;

namespace PSMetadataLib.PS2;

// VERY useful!! http://www.csclub.uwaterloo.ca:11068/mymc/ps2mcfs.html

public class MemoryCard
{
    private const string _magic = "Sony PS2 Memory Card Format ";
    public string Path;

    public const ushort ECCSize = 16;
    public string Version { get; private set; }
    public ushort PageSize { get; private set; }

    public ushort FullPageSize
    {
        get => (ushort)(PageSize + ECCSize);
    }

    public ushort PagesPerCluster { get; private set; }
    public ushort PagesPerBlock { get; private set; }
    public uint ClustersTotal { get; private set; }
    public uint AllocStart { get; private set; }
    public uint AllocEnd { get; private set; }
    public uint ClusterRootdir { get; private set; }
    public uint BBlock1 { get; private set; }
    public uint BBlock2 { get; private set; }
    public uint[] IndirectFatTable { get; private set; }
    public uint[] BadBlockTable { get; private set; }
    public uint CardType { get; private set; }
    public MemoryCardFlagsEnum CardFlags { get; private set; }

    public MemoryCardDirectory? RootDirectory { get; set; }


    protected void Load()
    {
        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None);

        var magic = Misc.ReadString(fs, 0x00, 0x1C, SeekOrigin.Begin);
        if (magic != _magic)
            throw new Exception($"File given was not a PlayStation 2 Memory Card. Expected: '{_magic}', got '{magic}'");

        // Read the superblock
        Version = Misc.ReadString(fs, 0, 0x00C, SeekOrigin.Current);
        PageSize = Misc.ReadUInt16(fs, 0x000028);
        PagesPerCluster = Misc.ReadUInt16(fs, 0x00002A);
        PagesPerBlock = Misc.ReadUInt16(fs, 0x00002C);
        ClustersTotal = Misc.ReadUInt32(fs, 0x00030);
        AllocStart = Misc.ReadUInt32(fs, 0x00034);
        AllocEnd = Misc.ReadUInt32(fs, 0x00038);
        ClusterRootdir = Misc.ReadUInt32(fs, 0x0003C);
        BBlock1 = Misc.ReadUInt32(fs, 0x00040);
        BBlock2 = Misc.ReadUInt32(fs, 0x00044);

        IndirectFatTable = Misc.ReadUInt32Array(fs, 0x50, 32);
        BadBlockTable = Misc.ReadUInt32Array(fs, 0xD0, 32);

        CardType = Misc.ReadByte(fs, 0x000150);
        CardFlags = (MemoryCardFlagsEnum)Misc.ReadByte(fs, 0x000151);

        if (CardType != 2)
            throw new Exception("File given wasn't a PS2 Memory Card.");
    }

    public byte[] ReadPage(FileStream fs, uint pageNumber)
    {
        var buffer = new byte[FullPageSize];
        fs.Seek(pageNumber * FullPageSize, SeekOrigin.Begin);
        fs.ReadExactly(buffer);
        return buffer;
    }

    public byte[] ReadCluster(FileStream fs, uint clusterNumber)
    {
        var buffer = new byte[FullPageSize * PagesPerCluster];
        var offset = clusterNumber > 0 ? 0 : 0;
        fs.Seek(((clusterNumber * PagesPerCluster) * FullPageSize) + offset, SeekOrigin.Begin);
        fs.ReadExactly(buffer);
        return buffer;
    }

    public byte[] ReadBlock(FileStream fs, uint blockNumber)
    {
        var buffer = new byte[FullPageSize * PagesPerBlock];
        fs.Seek((blockNumber * PagesPerBlock) * FullPageSize, SeekOrigin.Begin);
        fs.ReadExactly(buffer);
        return buffer;
    }

    public MemoryCard(string file)
    {
        Path = file;
        Load();
    }
}

public class MemoryCardDirectory
{
    public List<MemoryCardDirectoryEntry> Entries = [];

    public MemoryCardDirectory(Stream directoryStream)
    {
    }
}

public class MemoryCardDirectoryEntry
{
    public MemoryCardDirectoryMode Mode { get; set; }
    public uint Length { get; private set; }
    public MemoryCardDateTime CreationTime { get; private set; } = new(0);
    public MemoryCardDateTime ModifiedTime { get; private set; } = new(0);
    public uint Cluster { get; set; }
    public uint? DirEntry { get; set; }
    public uint? Attr { get; set; }
    public string Name { get; set; }

    public MemoryCardDirectoryEntry(Stream directoryEntryStream)
    {
    }
}

public class MemoryCardDateTime
{
    public ulong Timestamp { get; set; }

    public byte Seconds
    {
        get { return BitConverter.GetBytes(Timestamp)[1]; }
        set
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            bytes[1] = value;
            Timestamp = BitConverter.ToUInt64(bytes);
        }
    }

    public byte Minutes
    {
        get { return BitConverter.GetBytes(Timestamp)[2]; }
        set
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            bytes[2] = value;
            Timestamp = BitConverter.ToUInt64(bytes);
        }
    }

    public byte Hours
    {
        get { return BitConverter.GetBytes(Timestamp)[3]; }
        set
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            bytes[3] = value;
            Timestamp = BitConverter.ToUInt64(bytes);
        }
    }

    public byte Day
    {
        get { return BitConverter.GetBytes(Timestamp)[4]; }
        set
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            bytes[4] = value;
            Timestamp = BitConverter.ToUInt64(bytes);
        }
    }

    public byte Month
    {
        get { return BitConverter.GetBytes(Timestamp)[5]; }
        set
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            bytes[5] = value;
            Timestamp = BitConverter.ToUInt64(bytes);
        }
    }

    public int Year
    {
        get
        {
            var bytes = BitConverter.GetBytes(Timestamp);
            byte[] output = [bytes[6], bytes[7]];
            return BitConverter.ToInt32(output);
        }
        set
        {
            var nop = value;
        }
    }

    public MemoryCardDateTime(ulong timestamp)
    {
        Timestamp = timestamp;
    }
}