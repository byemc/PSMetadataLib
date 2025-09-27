using System.ComponentModel;
using System.Numerics;
using System.Text;

namespace PSMetadataLib;

public enum Endian
{
    Little,
    Big
}

public static class Misc
{
    public static uint ReadUInt32(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin, Endian endian = Endian.Little)
    {
        var buffer = new byte[4];
        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        switch (BitConverter.IsLittleEndian)
        {
            case false when endian.Equals(Endian.Little):
            case true when endian.Equals(Endian.Big):
                Array.Reverse(buffer);
                break;
        }
        return BitConverter.ToUInt32(buffer);
    }

    public static uint[] ReadUInt32Array(FileStream stream, int offset, int length,
        SeekOrigin origin = SeekOrigin.Begin, Endian endian = Endian.Little)
    {
        var output = new uint[length];
        for (var i = 0; i < length; i++)
        {
            output[i] = (uint)ReadUInt32(stream, offset + (i * 0x04));
        }

        return output;
    }
    
    public static ushort ReadUInt16(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin, Endian endian = Endian.Little)
    {
        var buffer = new byte[2];
        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        switch (BitConverter.IsLittleEndian)
        {
            case false when endian.Equals(Endian.Little):
            case true when endian.Equals(Endian.Big):
                Array.Reverse(buffer);
                break;
        }
        return BitConverter.ToUInt16(buffer);
    }

    public static byte ReadByte(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin,
        Endian endian = Endian.Little)
    {
        var buffer = new byte[1];
        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        switch (BitConverter.IsLittleEndian)
        {
            case false when endian.Equals(Endian.Little):
            case true when endian.Equals(Endian.Big):
                Array.Reverse(buffer);
                break;
        }
        return buffer[0];
    }
    
    // Reads a string of known length from a file.
    public static string ReadString(FileStream stream, int offset, int length, SeekOrigin origin = SeekOrigin.Begin)
    { 
        var buffer = new byte[length];

        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        return Encoding.UTF8.GetString(buffer);
    }

    // Reads a FileStream from `offset` until it finds a NUL byte, then returns everything it has read up to that point.
    public static string ReadNullTerminatedString(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        stream.Seek(offset, origin);

        var buffer = new byte[64];
        var result = "";
        while (stream.Read(buffer, 0, 64) > 0)
        {
            if (Encoding.UTF8.GetString(buffer).Contains('\0'))
            {
                result += Encoding.UTF8.GetString(buffer).Split('\0').FirstOrDefault() ?? "";
                break;
            }
            result += Encoding.UTF8.GetString(buffer);
        }

        return result;
    }
}