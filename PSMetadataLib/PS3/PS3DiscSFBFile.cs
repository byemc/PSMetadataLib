using PSMetadataLib.Filetypes;
using static PSMetadataLib.Misc;

namespace PSMetadataLib.PS3;

/**
 * Represents a PS3_DISC.SFB file.
 */
public class PS3DiscSFBFile
{
    private byte[] _magic = ".SFB"u8.ToArray();

    public string HybridFlag
    {
        get;
        private set;
    }

    public HybridFlagEnum HybridFlagFlags
    {
        get
        {
            HybridFlagEnum output = 0;
            foreach (char flag in HybridFlag.ToList())
            {
                switch (flag)
                {
                    case 'S':
                        output |= HybridFlagEnum.DiscBenefits;
                        break;
                    case 'T':
                        output |= HybridFlagEnum.Themes;
                        break;
                    case 'V':
                        output |= HybridFlagEnum.Video;
                        break;
                    case 'g':
                        output |= HybridFlagEnum.DiscGame;
                        break;
                    case 'u':
                        output |= HybridFlagEnum.FirmwareUpdate;
                        break;
                    case 'v':
                        output |= HybridFlagEnum.BluRayMovie;
                        break;
                }
            }
            return output;
        }
    }

    /**
     * Lists the potential locations of SFO files relative to the disc root. 
     */
    public List<string> SfoLocations
    {
        get
        {
            List<string> output = [];
            var flags = HybridFlagFlags;
            if (flags.HasFlag(HybridFlagEnum.Themes)) output.Add("PS3_CONTENT/THEMEDIR/PARAM.SFO");
            if (flags.HasFlag(HybridFlagEnum.Video)) output.Add("PS3_CONTENT/VIDEODIR/PARAM.SFO");
            if (flags.HasFlag(HybridFlagEnum.DiscGame))
                output.AddRange(["PS3_GAME/PARAM.SFO", "PS3_EXTRA/PARAM.SFO"]);
            if (flags.HasFlag(HybridFlagEnum.BluRayMovie)) output.Add("PS3_VPRM/PARAM.SFO");
            return output;
        }
    }

    public List<string> GetExistingSfoLocations(string basePath)
    {
        List<string> output = [];
        output.AddRange(from location in SfoLocations where File.Exists(Path.Join(basePath, location)) select Path.Join(basePath, location));
        return output;
    }

    public string TitleId { get; private set; }
    public string DiscVersion { get; private set; } = "";

    public PS3DiscSFBFile(string path)
    {
        Load(path);
    }
    
    // Loads parameters from a PS3_DISC.SFB file.
    private void Load(string path)
    {
        using FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        var magicBuffer = new byte[4];
        fs.ReadExactly(magicBuffer);
        if (!magicBuffer.SequenceEqual(_magic)) 
            throw new BadMagicSignatureException("File passed into SFB parser is not a valid SFB file. (Header doesn't match)");

        var hybridFlagDataOffset = ReadUInt32(fs, 0x30, SeekOrigin.Begin, Endian.Big);
        var hybridFlagDataLength = ReadUInt32(fs, 0, SeekOrigin.Current, Endian.Big);
        HybridFlag = ReadString(fs, (int)hybridFlagDataOffset, (int)hybridFlagDataLength);

        var titleIdDataOffset = ReadUInt32(fs, 0x50, SeekOrigin.Begin, Endian.Big);
        var titleIdDataLength = ReadUInt32(fs, 0, SeekOrigin.Current, Endian.Big);
        TitleId = ReadString(fs, (int)titleIdDataOffset, (int)hybridFlagDataLength);

        var discVersionString = ReadString(fs, 0x60, 0x8);
        if (discVersionString == "")
            return;
        var discVersionDataOffset = ReadUInt32(fs, 0x70);
        var discVersionDataLength = ReadUInt32(fs, 0, SeekOrigin.Current);
        DiscVersion = ReadString(fs, (int)discVersionDataOffset, (int)discVersionDataLength);
    }
}