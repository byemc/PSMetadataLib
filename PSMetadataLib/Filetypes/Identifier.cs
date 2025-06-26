namespace PSMetadataLib.Filetypes;

/**
 * Identifies the type of the file given.
 */
public static class Identifier
{
    public static string? Identify(string file)
    {
        var sfoMagicHeader = "\0PSF"u8.ToArray();
        var sfbMagicHeader = ".SFB"u8.ToArray();

        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var buffer = new byte[4];
        fs.ReadExactly(buffer);
        
        if (sfoMagicHeader.SequenceEqual(buffer))
            return "SFO";
        if (sfbMagicHeader.SequenceEqual(buffer))
            return "SFB";
        
        return null;
    }
}