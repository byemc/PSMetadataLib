using System.Text;

namespace PSMetadataLib;

internal class BadMagicSignatureException(string message) : Exception(message);

public class SfoFile
{
    public int Length { get; private set; } = 0;
    public Dictionary<string, string> Entries { get; private set; } = [];
    
    public SfoFile(string file)
    {
        Load(file);
    }
    
    public void Load(string file)
    {
        var magic = new byte[4];
        byte[] magicSignature = "\0PSF"u8.ToArray(); // This is what's expected to be in the header.

        using FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                                                                                // ^^ Allows other processes to read this file, but not write.
        // Check that the file is an SFO file.
        fs.ReadExactly(magic, 0, 4);
        if (!magic.SequenceEqual(magicSignature))
        {
            throw new BadMagicSignatureException("File passed into SFO parser is not a valid SFO file. (Header doesn't match)");
        }

        var keyTableStart = Misc.ReadUInt32(fs, 0x08); // Where the key table starts.
        var dataTableStart = Misc.ReadUInt32(fs, 0x0C); // Where the data table starts.
        var tableEntries = Misc.ReadUInt32(fs, 0x10); // How many table entries there are.
        Length = Convert.ToInt32(tableEntries);
        
        // Seek to the start of the index table.
        fs.Seek(0x14, SeekOrigin.Begin);

        for (int i = 0; i < Length; i++)
        {
            var start = 0x14 + (0x10 * i);
            var offset = 0x00;
            // Seek to the start of the index table + (the length of an index entry * i)
            fs.Seek(start, SeekOrigin.Begin);
            
            var keyOffset = Misc.ReadUInt16(fs, start);
            offset += 0x02;
            var dataFormat = Misc.ReadUInt16(fs, start + offset);
            offset += 0x02;

            var dataLength = Misc.ReadUInt32(fs, start + offset);
            offset += 0x04;
            var dataMaxLength = Misc.ReadUInt32(fs, start + offset);
            offset += 0x04;
            var dataOffset = Misc.ReadUInt32(fs, start + offset);
            
            var keyName = Misc.ReadNullTerminatedString(fs, Convert.ToInt32(keyTableStart + keyOffset));
            var keyData = Misc.ReadString(fs, Convert.ToInt32(dataTableStart + dataOffset),
                Convert.ToInt32(dataLength));

            Entries.Add(keyName, keyData);
        }
    }
}