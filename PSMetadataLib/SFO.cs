using System.Data;
using System.Numerics;
using System.Text;

namespace PSMetadataLib;

internal class BadMagicSignatureException(string message) : Exception(message);

public enum ParamDataFormatEnum : ushort
{
    UTF8S = 4,
    UTF8 = 516,
    INT32 = 1028,
}

public class SFOParamValue
{
    public object Value;
    public ParamDataFormatEnum Format;

    public SFOParamValue(string value, bool special = false)
    {
        Value = value;
        Format = special ? ParamDataFormatEnum.UTF8S : ParamDataFormatEnum.UTF8;
    }

    public SFOParamValue(uint value)
    {
        Value = value;
        Format = ParamDataFormatEnum.INT32;
    }
    
    public SFOParamValue(int value)
    {
        Value = (uint)value;
        Format = ParamDataFormatEnum.INT32;
    }

    public override string ToString()
    {
        return Value.ToString() ?? base.ToString() ?? "";
    }

    public static implicit operator SFOParamValue(string value)
    {
        return new SFOParamValue(value);
    }
    public static implicit operator SFOParamValue(int value)
    {
        return new SFOParamValue(value);
    }
    public static implicit operator SFOParamValue(uint value)
    {
        return new SFOParamValue(value);
    }
}

public class SfoFile
{
    protected int Length { get; private set; } = 0;
    public Dictionary<string, SFOParamValue> Entries { get; private set; } = [];

    private byte[] MagicSignature = "\0PSF"u8.ToArray(); // This is what's expected to be in the header.
    
    protected void SaveValueToEntries(string key, SFOParamValue? value)
    {
        // Saves a value to Entries, or deletes it if "value" is null.

        if (value is null) Entries.Remove(key);
        else Entries[key] = value;
    }
    
    protected void SaveBoolToEntries(string key, bool? value)
    {
        // Saves a bool as uint to Entries, or deletes it if "value" is null.

        if (value is null) Entries.Remove(key);
        else Entries[key] = Convert.ToUInt32(value);
    }

    protected void SaveStringToEntries(string key, string? value, Func<string, bool>? lengthConstraint = null, string? exceptionMessage = "", bool special = false)
    {
        if (value is not null)
        {
            if (lengthConstraint?.Invoke(value) ?? true) Entries[key] = new SFOParamValue(value, special);
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

        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
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

            if ((ParamDataFormatEnum)dataFormat == ParamDataFormatEnum.INT32)
            {
                var keyData = Misc.ReadUInt32(fs, (int)dataTableStart + (int)dataOffset);
                Entries.Add(keyName, keyData);
            }
            else if ((ParamDataFormatEnum)dataFormat == ParamDataFormatEnum.UTF8S)
            {
                var keyDataBuffer = new byte[dataLength];
                fs.Seek((int)dataTableStart + (int)dataOffset, SeekOrigin.Begin);
                fs.ReadExactly(keyDataBuffer);
                Entries.Add(keyName, new SFOParamValue(Encoding.UTF8.GetString(keyDataBuffer), true));
            }
            else
            {
                var keyData = Misc.ReadNullTerminatedString(fs, (int)dataTableStart + (int)dataOffset);
                Entries.Add(keyName, new SFOParamValue(keyData));
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
            ParamDataFormatEnum dataFormat;
            
            switch (value.Format)
            { 
                // Turn the value into bytes
                case ParamDataFormatEnum.INT32:
                {
                    trueValue.AddRange(BitConverter.GetBytes((uint)value.Value));
                    dataFormat = ParamDataFormatEnum.INT32;
                    break;
                }
                case ParamDataFormatEnum.UTF8:
                {
                    trueValue.AddRange(Encoding.UTF8.GetBytes((string)value.Value + "\0"));
                    dataFormat = ParamDataFormatEnum.UTF8;
                    break;
                }
                case ParamDataFormatEnum.UTF8S:
                    trueValue.AddRange(Encoding.UTF8.GetBytes((string)value.Value));
                    dataFormat = ParamDataFormatEnum.UTF8;
                    break;
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
        var keyTableStart = 0x14 + (uint)indexTable.Count;
        var keyTableEnd = (uint)(keyTableStart + keyTable.Count);
        var paddingNeeded = (uint)((Math.Ceiling(keyTableEnd / 4.0) * 4) - keyTableEnd);
        var padding = new byte[paddingNeeded];
        keyTable.AddRange(padding);

        // Data table starts after key table, with padding to align it to four bytes
        var dataTableStart = (uint)(keyTableStart + keyTable.Count);

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