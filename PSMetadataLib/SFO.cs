using System.Data;
using System.Numerics;
using System.Text;

namespace PSMetadataLib;

internal class BadMagicSignatureException(string message) : Exception(message);

internal enum ParaDataFormatEnum : ushort
{
    None,
    UTF8S = 0x0400,
    UTF8 = 516,
    INT32 = 1028,
}

public class SfoFile
{
    protected int Length { get; private set; } = 0;
    public Dictionary<string, object> Entries { get; private set; } = [];

    private byte[] MagicSignature = "\0PSF"u8.ToArray(); // This is what's expected to be in the header.
    
    protected void SaveValueToEntries(string key, object? value)
    {
        // Saves a value to Entries, or deletes it if "value" is null.

        if (value is null) Entries.Remove(key);
        else Entries[key] = value;
    }

    protected void SaveStringToEntries(string key, string? value, Func<string, bool>? lengthConstraint = null, string? exceptionMessage = "")
    {
        if (value is not null)
        {
            if (lengthConstraint?.Invoke(value) ?? true) Entries[key] = value;
            else throw new ConstraintException(exceptionMessage ?? $"{key} failed to meet constraints");
        }
        else
            Entries.Remove(key);
    }

    protected SfoFile()
    {
        
    }
    
    public SfoFile(string file)
    {
        Load(file);
    }

    protected void Load(string file)
    {
        var magic = new byte[4];

        using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                                                                                // ^^ Allows other processes to read this file, but not write.
        // Check that the file is an SFO file.
        fs.ReadExactly(magic, 0, 4);
        if (!magic.SequenceEqual(MagicSignature))
        {
            throw new BadMagicSignatureException("File passed into SFO parser is not a valid SFO file. (Header doesn't match)");
        }

        var keyTableStart = Misc.ReadUInt32(fs, 0x08); // Where the key table starts.
        var dataTableStart = Misc.ReadUInt32(fs, 0x0C); // Where the data table starts.
        var tableEntries = Misc.ReadUInt32(fs, 0x10); // How many table entries there are.
        Length = Convert.ToInt32(tableEntries);
        
        // Seek to the start of the index table.
        fs.Seek(0x14, SeekOrigin.Begin);

        for (var i = 0; i < Length; i++)
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
            
            if (1028 == dataFormat)
            {
                var keyData = Misc.ReadUInt32(fs, Convert.ToInt32(dataTableStart + dataOffset));
                Entries.Add(keyName, keyData);
            }
            else
            {
                var keyData = Misc.ReadNullTerminatedString(fs, Convert.ToInt32(dataTableStart + dataOffset));
                Entries.Add(keyName, keyData);
            }

        }
    }

    public void Save(string path)
    {
        List<byte> output = [];
        
        // Header
        List<byte> header = [];
        header.AddRange(MagicSignature);
        header.AddRange([0x01, 0x01, 0x00, 0x00]);      // 1.01
        uint keyTableStart;
        uint dataTableStart;
        var tablesEntries = (uint)Length;
        
        // The tables
        List<byte> indexTable = [];
        List<byte> keyTable = [];
        uint keyTableOffset = 0;
        List<byte> dataTable = [];
        uint dataTableOffset = 0;
        
        foreach (var (key, value) in Entries)
        {
            List<byte> entry = [];
            
            List<byte> trueValue = [];
            var dataFormat = ParaDataFormatEnum.None;
            
            switch (value)
            { 
                // Turn the value into bytes
                case uint valUint:
                {
                    trueValue.AddRange(BitConverter.GetBytes(valUint));
                    dataFormat = ParaDataFormatEnum.INT32;
                    break;
                }
                case string valString:
                {
                    trueValue.AddRange(Encoding.UTF8.GetBytes(valString + "\0"));
                    dataFormat = ParaDataFormatEnum.UTF8;
                    break;
                }
                default: // Ignore it
                    continue;
            }
            
            var dataLength = (uint)trueValue.Count;
            var dataMaxLength = BitOperations.RoundUpToPowerOf2(dataLength);
            
            entry.AddRange(BitConverter.GetBytes((ushort)keyTableOffset));  // Adds key_offset
            entry.AddRange(BitConverter.GetBytes((ushort)dataFormat));      // Adds data_fmt
            entry.AddRange(BitConverter.GetBytes(dataLength));              // data_len
            entry.AddRange(BitConverter.GetBytes(dataMaxLength));           // data_max_len
            entry.AddRange(BitConverter.GetBytes(dataTableOffset));         // data_offset
            
            // Write to the key and data tables
            keyTable.AddRange(Encoding.UTF8.GetBytes(key + "\0"));
            keyTableOffset += (uint)key.Length + 1;                                 // BLUS12345 7 bytes + \0 1 byte = 8 bytes
            
            dataTable.AddRange(trueValue);
            dataTable.AddRange(new byte[dataMaxLength - dataLength]);
            dataTableOffset += dataMaxLength;
            
            indexTable.AddRange(entry);                                        // Adds the entry to index_table.
        }
        
        // Working out where the key table will start.
        // Header is a fixed size 0x14
        // header + index table should be it?
        keyTableStart = 0x14 + (uint)indexTable.Count;
        var keyTableEnd = (uint)(keyTableStart + keyTable.Count);
        var paddingNeeded = (uint)((Math.Ceiling(keyTableEnd / 4.0) * 4) - keyTableEnd);
        var padding = new byte[paddingNeeded];
        keyTable.AddRange(padding);

        // Data table starts after key table, with padding to align it to four bytes
        dataTableStart = (uint)(keyTableStart + keyTable.Count);
        
        // Add these to the header
        header.AddRange(BitConverter.GetBytes(keyTableStart));
        header.AddRange(BitConverter.GetBytes(dataTableStart));
        header.AddRange(BitConverter.GetBytes(tablesEntries));

        output.AddRange(header);
        output.AddRange(indexTable);
        output.AddRange(keyTable);
        output.AddRange(dataTable);
        
        File.WriteAllBytes(path, output.ToArray());
    }
}