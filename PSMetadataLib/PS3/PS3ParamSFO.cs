using System.Reflection;
using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regex;

namespace PSMetadataLib.PS3;

/**
 * Represents a PlayStation 3 PARAM.SFO file.
 * 
 * Much of the details for the fields and properties of this class are thanks to the PS Developer Wiki: https://www.psdevwiki.com/ps3/PARAM.SFO
 */
public class PS3ParamSFO : SfoFile
{
   /**
     * Application version, represented as a string 5 characters long in the format "XX.YY"
     *
     * Parameter name is APP_VER
     */
    public string? AppVer
    {
        get => (string?)Entries.GetValueOrDefault("APP_VER");
        set => SaveStringToEntries("APP_VER", value, v => v.Length == 5 && IsMatch(v, _appVerRegex.Pattern),
            "APP_KEY must be 5 characters long and in the format XX.YY.");
    }
    private readonly GeneratedRegexAttribute _appVerRegex = new GeneratedRegexAttribute(@"/\d{2}[.]\d{2}/");


    /**
     * Flags effecting features of the content.
     * Documented properly at https://www.psdevwiki.com/ps3/PARAM.SFO#ATTRIBUTE, this is also where my descriptions for the enums come from.
     * Enum names are based on usage on bootable content first, and then save data, disc subfolders and finally patches.
     */
    public AttributeEnum? Attribute { get => Entries.TryGetValue("ATTRIBUTE", out var attr)
            ? (AttributeEnum)attr : null;
        set => SaveValueToEntries("ATTRIBUTE", (uint?)value);
    }

    /**
     * Specifies if the content is bootable or not. Used on games.
     *
     * Parameter name is BOOTABLE
     */
    public BootableEnum? Bootable { get => Entries.TryGetValue("BOOTABLE", out var bootable)
            ? (BootableEnum)bootable : null;
        set => SaveValueToEntries("BOOTABLE", (uint?)value);
    }
    
    /**
     * Specifies the category of the content, i.e. where it is placed in the XMB.
     */
    public PS3ParamCategoryEnum? Category
    {
        get
        {
            var cat = Entries.TryGetValue("CATEGORY", out var o) 
                ? (string)o : null;

            return cat is null ? null : MatchCategoryCodeToEnum(cat);
        }
        set => SaveStringToEntries("CATEGORY", value?.GetShortName());
    }
    
    public static PS3ParamCategoryEnum? MatchCategoryCodeToEnum(string shortCode)
    {
        var matchedValue = typeof(PS3ParamCategoryEnum)
            .GetFields()
            .FirstOrDefault(f => f.IsLiteral && shortCode == f.GetCustomAttribute<ShortNameAttribute>()!.ShortName);

        var unboxedValue = matchedValue?.GetRawConstantValue();

        if (unboxedValue is null)
        {
            return null;
        }

        return (PS3ParamCategoryEnum)unboxedValue;
    }

    /**
     * Used in save files. Text displayed beneath SUBTITLE. Maximum 1023 characters.
     */
    public string? Detail
    {
        get => (string?)Entries.GetValueOrDefault("DETAIL");
        set => SaveStringToEntries("DETAIL", value, s => s.Length <= 1023,
            "DETAIL must be less than 1024 characters long.");
    }

    /**
     * Orders content vertically in the XMB.
     *
     * Parameter is ITEM_PRIORITY
     */
    public uint? ItemPriority
    {
        get => (uint?)Entries.GetValueOrDefault("ITEM_PRIORITY");
        set => SaveValueToEntries("ITEM_PRIORITY", value);
    }

    /**
     * Language used by trophies(?). Doesn't include every language supported by this field due to the PS3 not supporting them here.
     *
     * Parameter: LANG
     */
    public LanguagesEnum? Lang { get => Entries.TryGetValue("LANG", out var bootable)
            ? (LanguagesEnum)bootable : null;
        set => SaveValueToEntries("LANG", value);
    }
    
    /**
     * License text of content (Used by HDD Games)
     *
     * Parameter: LICENSE
     */
    public string? License
    {
        get => (string?)Entries.GetValueOrDefault("LICENSE");
        set => SaveStringToEntries("LICENSE", value, s => s.Length <= 511,
            "LICENSE must be less than 512 characters long.");
    }

    /**
     * Used for parental controls
     */
    public uint? ParentalLevel
    {
        get => (uint?)Entries.GetValueOrDefault("PARENTAL_LEVEL");
        set => SaveValueToEntries("PARENTAL_LEVEL", value);
    }

    /**
     * Defines what video modes content supports.
     */
    public ResolutionEnum? Resolution
    {
        get => Entries.TryGetValue("RESOLUTION", out var bootable)
            ? (ResolutionEnum)bootable
            : null;
        set => SaveValueToEntries("RESOLUTION", value);
    }

    /**
     * Defines what sound modes the content supports
     */
    public SoundFormatEnum? SoundFormat
    {
        get => Entries.TryGetValue("SOUND_FORMAT", out var bootable)
            ? (SoundFormatEnum)bootable
            : null;
        set => SaveValueToEntries("SOUND_FORMAT", value);
    }
    
    /**
     * Subtitle of content.
     */
    public string? SubTitle
    {
        get => (string?)Entries.GetValueOrDefault("SUB_TITLE");
        set => SaveStringToEntries("SUB_TITLE", value, s => s.Length < 128,
            "SUB_TITLE must be less than 128 characters long.");
    }
    
    /**
     * Title of the content. Max of 127 characters.
     * Localised titles might be in the Entries parameter as TITLE_xx, where xx is the regional code.
     */
    public string? Title
    {
        get => (string?)Entries.GetValueOrDefault("TITLE");
        set => SaveStringToEntries("TITLE", value, s => s.Length < 128,
            "TITLE must be less than 128 characters long.");
    }
    
    /**
     * Title ID of the content. Max of 15 characters.
     */
    public string? TitleId
    {
        get => (string?) Entries.GetValueOrDefault("TITLE_ID");
        set => SaveStringToEntries("TITLE_ID", value, s => s.Length < 16,
            "TITLE_ID must be less than 16 characters long.");
    }
    
    /**
     * Create PS3ParamSFO file using information from a PARAM.SFO file given in `path`
     */
    public PS3ParamSFO(string path)
    {
        // Loads the entries from the PARAM.SFO into Entries.
        Load(path);

        if (Length <= 0)
        {
            throw new FileLoadException("Could not find any parameters in the PARAM.SFO file.");
        }
    }
}