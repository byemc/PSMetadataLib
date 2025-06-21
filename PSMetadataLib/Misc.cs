using System.ComponentModel;
using System.Text;

namespace PSMetadataLib;

public class Misc
{
    public static uint ReadUInt32(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        var buffer = new byte[4];
        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        return BitConverter.ToUInt32(buffer);
    }
    
    public static ushort ReadUInt16(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        var buffer = new byte[2];
        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        return BitConverter.ToUInt16(buffer);
    }
    
    // Reads a string of known length from a file.
    public static string ReadString(FileStream stream, int offset, int length, SeekOrigin origin = SeekOrigin.Begin)
    { 
        var buffer = new byte[length];

        stream.Seek(offset, origin);
        stream.ReadExactly(buffer);
        return Encoding.UTF8.GetString(buffer);
    }

    public static string ReadNullTerminatedString(FileStream stream, int offset, SeekOrigin origin = SeekOrigin.Begin)
    {
        stream.Seek(offset, origin);
        
        /* TODO: Make this not suck
            So the issue here is that im reading one byte at a time. */
        byte lastByte = Convert.ToByte(stream.ReadByte());
        List<byte> buffer = [];
        
        while (lastByte != 0x00)
        {
            buffer.Add(lastByte);
            lastByte = Convert.ToByte(stream.ReadByte());
        }
        
        // If we're here, then we're at the end of the string.
        var bytes = buffer.ToArray();
        return Encoding.UTF8.GetString(bytes);
    }
}