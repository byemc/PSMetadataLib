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

    public ushort TotalPageSize
    {
        get => (ushort)(PageSize + ECCSize);
    }

    public List<MemoryCardCluster> Clusters;

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

        Clusters = new List<MemoryCardCluster>();
        
        if (CardType != 2)
            throw new Exception("File given wasn't a PS2 Memory Card.");

        for (var i = 0; i < ClustersTotal; i++)
        {
            Clusters.Add(ReadCluster(fs, (uint)i));
        }
    }

    public MemoryCardFATEntry GetFATEntry(uint index)
    {
        var fatOffset = index % 256;
        var indirectIndex = index / 256;
        var indirectOffset = indirectIndex % 256;
        double dblIndirectIndex = indirectIndex / 256;
        var indirectClusterNum = IndirectFatTable[(int)dblIndirectIndex];
        var indirectCluster = ReadCluster(indirectClusterNum);
        var fatClusterNum = indirectCluster.Data[(int)indirectOffset];
        var fatCluster = ReadCluster(fatClusterNum);
        return new MemoryCardFATEntry(fatCluster.Data[fatOffset]);
    }

    public MemoryCardPage ReadPage(uint number)
    {
        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None);
        return ReadPage(fs, number);
    }

    private MemoryCardPage ReadPage(FileStream fs, uint number)
    {
        return new MemoryCardPage(fs, number, PageSize, ECCSize);
    }
    
    private MemoryCardCluster ReadCluster(FileStream fs, uint number)
    {
        return new MemoryCardCluster(fs, number, PagesPerCluster, PageSize, ECCSize);
    }
    
    public MemoryCardCluster ReadCluster(uint number)
    {
        using var fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.None);
        return ReadCluster(fs, number);
    }

    public MemoryCard(string file)
    {
        Path = file;
        Load();
    }
}

public class MemoryCardFATEntry(uint entry)
{
    private uint AllocatedBitMask = 0x80000000;
    
    public bool IsFree { get; set; } = (entry << 31) >> 31 != 1;
    public uint NextCluster { get; set; } = (entry >> 1);
    public uint OriginalEntry = entry;
}

public class MemoryCardPage
{
    public byte[] Data { get; } = new byte[512];
    public byte[] ECC { get; } = new byte[16];

    public MemoryCardPage(byte[] data, byte[] ecc)
    {
        Data = data;
        ECC = ecc;
    }

    public MemoryCardPage(Stream fs, uint number = 0, uint pageLength = 512, uint eccLength = 16)
    {
        var dataBuffer = new byte[pageLength];
        var eccBuffer = new byte[eccLength];
        var pageTotalLength = pageLength + eccLength;
        // var willOverflow = (pageTotalLength * (number + 1)) > fs.Length;
        
        // if (willOverflow)
        //     Console.WriteLine($"WARNING WILL OVERFLOW LENGTH {fs.Length} POS {pageTotalLength * (number)} NUM {number} / {fs.Length / pageTotalLength}");
        
        fs.Seek(pageTotalLength * number, SeekOrigin.Begin);
        fs.Read(dataBuffer, 0, (int)pageLength);
        fs.Read(eccBuffer,  0, (int)eccLength);

        Data = dataBuffer;
        ECC = eccBuffer;
    }
}

public class MemoryCardCluster
{
    public readonly List<MemoryCardPage> Pages = [];
    private uint PagesPerChunk { get; } = 2;
    private uint PageLength { get; } = 512;
    private uint ECCLength { get; } = 16;

    public byte[] Data
    {
        get
        {
            var buffer = new List<byte>();
            foreach (var memoryCardPage in Pages)
            {
                buffer.AddRange(memoryCardPage.Data);
                buffer.AddRange(memoryCardPage.ECC);
            }

            return buffer.ToArray();
        }
    }

    public MemoryCardCluster(List<MemoryCardPage> pages)
    {
        Pages = pages;
    }
    
    public MemoryCardCluster(Stream fs, uint number = 0, uint pagesPerChunk = 2, uint pageLength = 512,
        uint eccLength = 16)
    {
        PagesPerChunk = pagesPerChunk;
        PageLength = pageLength;
        ECCLength = eccLength;
        for (var i = 0; i < pagesPerChunk; i++)
        {
            var page = new MemoryCardPage(fs, (uint)(i + (number * pagesPerChunk)), pageLength,
                eccLength);
            Pages.Add(page);
        }
    }
}

public class MemoryCardBlock
{
    public MemoryCardPage[] Pages;

    public MemoryCardBlock(MemoryCardPage[] pages)
    {
        Pages = pages;
    }
    
    public MemoryCardBlock(Stream fs, uint number = 0, uint pagesPerBlock = 16, uint pageLength = 512,
        uint eccLength = 16)
    {
        var buffer = new MemoryCardPage[pagesPerBlock];
        for (uint i = 0; i < pagesPerBlock; i++)
        {
            buffer[i] = new MemoryCardPage(fs, (uint)(number * pagesPerBlock * (pageLength + eccLength)), pageLength,
                eccLength);
        }
    }
}