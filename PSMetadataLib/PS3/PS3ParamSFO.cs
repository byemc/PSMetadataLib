using System.Reflection;
using System.Text.RegularExpressions;
using PSMetadataLib.Filetypes;
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
     * Account ID, for use on save data.
     *
     * Maps to ACCOUNT_ID. 
     */
    public string? AccountId
    {
        get => (string?)Entries.GetValueOrDefault("ACCOUNT_ID")?.Value;
        set => SaveStringToEntries("ACCOUNT_ID", value, s => s.Length == 16, special:true);
    }

    /**
     * Enables the analog sticks in PSX games.
     *
     * Maps to ANALOG_MODE
     */
    public bool? AnalogMode
    {
        get => (bool?)Entries.GetValueOrDefault("ANALOG_MODE")?.Value;
        set => SaveBoolToEntries("ANALOG_MODE", value);
    }
    
   /**
     * Application version, represented as a string 5 characters long in the format "XX.YY"
     *
     * Parameter name is APP_VER
     */
    public string? AppVer
    {
        get => (string?)Entries.GetValueOrDefault("APP_VER")?.Value;
        set => SaveStringToEntries("APP_VER", value, v => v.Length == 5 && IsMatch(v, _appVerRegex.Pattern),
            "APP_KEY must be 5 characters long and in the format XX.YY.", maxLength:0x8);
    }
    private readonly GeneratedRegexAttribute _appVerRegex = new GeneratedRegexAttribute(@"/\d{2}[.]\d{2}/");


    /**
     * Flags effecting features of the content.
     * Documented properly at https://www.psdevwiki.com/ps3/PARAM.SFO#ATTRIBUTE, this is also where my descriptions for the enums come from.
     * Enum names are based on usage on bootable content first, and then save data, disc subfolders and finally patches.
     */
    public AttributeEnum? Attribute { get => Entries.TryGetValue("ATTRIBUTE", out var attr)
            ? (AttributeEnum)attr.Value : null;
        set => SaveValueToEntries("ATTRIBUTE", (uint?)value);
    }

    /**
     * Specifies if the content is bootable or not. Used on games.
     *
     * Parameter name is BOOTABLE
     */
    public BootableEnum? Bootable { get => Entries.TryGetValue("BOOTABLE", out var bootable)
            ? (BootableEnum)bootable.Value : null;
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
                ? (string)o.Value : null;

            return cat is null ? null : MatchCategoryCodeToEnum(cat);
        }
        set => SaveStringToEntries("CATEGORY", value?.GetShortName(), maxLength:0x4);
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
     * Content Identifier for trial/demo games to show a "purchase" button in the XMB. Used by HDD games.
     *
     * In the format XXYYYY-NP_COMMUNICATION_ID-LICENSE_ID, but this isnt enforced in this library yet.
     *
     * Maps to CONTENT_ID.
     */
    public string? ContentId
    {
        get => (string?)Entries.GetValueOrDefault("CONTENT_ID")?.Value;
        set => SaveStringToEntries("CONTENT_ID", value);    // TODO: enforce length
    }

    /**
     * Used in save files. Text usually displayed beneath SUBTITLE. Maximum 1023 characters.
     *
     * Use the byte \0A to place a linebreak.
     *
     * Maps to DETAIL, except on PSP Minis save data where it's mapped to SAVEDATA_DETAIL.
     * This is determined when the property here is set, and if the other is present it will be unset.
     */
    public string? Detail
    {
        get
        {
            var key = "DETAIL";
            if (Category == PS3ParamCategoryEnum.PspMinis) key = "SAVEDATA_DETAIL";
            return (string?)Entries.GetValueOrDefault(key)?.Value;
        }
        set
        {
            if (Category == PS3ParamCategoryEnum.PspMinis)
            {
                SaveStringToEntries("DETAIL", null, s => s.Length <= 1023,
                    "DETAIL must be less than 1024 characters long.");
                SaveStringToEntries("SAVEDATA_DETAIL", value, s => s.Length <= 1023,
                    "DETAIL must be less than 1024 characters long.");
            }
            else
            {
                SaveStringToEntries("DETAIL", value, s => s.Length <= 1023,
                    "DETAIL must be less than 1024 characters long.");
                SaveStringToEntries("SAVEDATA_DETAIL", null, s => s.Length <= 1023,
                    "DETAIL must be less than 1024 characters long.");
            }
        }
    }
    
    /**
     * Orders content vertically in the XMB.
     *
     * Maps to ITEM_PRIORITY
     */
    public uint? ItemPriority
    {
        get => (uint?)Entries.GetValueOrDefault("ITEM_PRIORITY")?.Value;
        set => SaveValueToEntries("ITEM_PRIORITY", value);
    }

    /**
     * Language used by trophies(?). Doesn't include every language supported by this field due to the PS3 not supporting them here.
     *
     * Parameter: LANG
     */
    public LanguagesEnum? Lang { get => Entries.TryGetValue("LANG", out var lang)
            ? (LanguagesEnum)lang.Value : null;
        set => SaveValueToEntries("LANG", (uint?)value);
    }
    
    /**
     * License text of content (Used by HDD Games)
     *
     * Parameter: LICENSE
     */
    public string? License
    {
        get => (string?)Entries.GetValueOrDefault("LICENSE")?.Value;
        set => SaveStringToEntries("LICENSE", value, s => s.Length <= 511,
            "LICENSE must be less than 512 characters long.");
    }

    /**
     * Network Platform Communications ID, also used on the folder the trophy installer is in
     *
     * Maps to NP_COMMUNICATION_ID
     */
    public string? NpCommunicationId
    {
        get => (string?)Entries.GetValueOrDefault("NP_COMMUNICATION_ID")?.Value;
        set => SaveStringToEntries("NP_COMMUNICATION_ID", value, s => s.Length <= 12,
            "LICENSE must be less than 13 characters long.");
    }
    
    /**
     * Used by PS3 Save data
     */
    public string? Params
    {
        get => (string?)Entries.GetValueOrDefault("PARAMS")?.Value;
        set => SaveStringToEntries("PARAMS", value, s => s.Length == 1024,
            "PARAMS must be 1024 bytes long.");
    }

    /**
     * Used for parental controls
     */
    public uint? ParentalLevel
    {
        get => (uint?)Entries.GetValueOrDefault("PARENTAL_LEVEL")?.Value;
        set => SaveValueToEntries("PARENTAL_LEVEL", value);
    }
    
    /**
     * Minimum PS3 firmware version needed to boot content.
     * Format: XX.YYYY
     *
     * Mapped to PS3_SYSTEM_VER
     */
    public string? Ps3SystemVersion
    {
        get => (string?)Entries.GetValueOrDefault("PS3_SYSTEM_VER")?.Value;
        set => SaveStringToEntries("PS3_SYSTEM_VER", value, v => v.Length == 7 && IsMatch(v, _sysVerRegex.Pattern),
            "PS3_SYSTEM_VER must be 7 characters long and in the format XX.YYY.");
    }
    private readonly GeneratedRegexAttribute _sysVerRegex = new GeneratedRegexAttribute(@"/\d{2}[.]\d{3}/");
    
    /**
     * Defines what video modes content supports.
     */
    public ResolutionEnum? Resolution
    {
        get => Entries.TryGetValue("RESOLUTION", out var value)
            ? (ResolutionEnum)value.Value
            : null;
        set => SaveValueToEntries("RESOLUTION", (uint?)value);
    }

    /**
     * Defines what sound modes the content supports
     */
    public SoundFormatEnum? SoundFormat
    {
        get => Entries.TryGetValue("SOUND_FORMAT", out var fmt)
            ? (SoundFormatEnum)fmt.Value
            : null;
        set => SaveValueToEntries("SOUND_FORMAT", (uint?)value);
    }
    
    /**
     * Subtitle of content.
     */
    public string? SubTitle
    {
        get => (string?)Entries.GetValueOrDefault("SUB_TITLE")?.Value;
        set => SaveStringToEntries("SUB_TITLE", value, s => s.Length < 128,
            "SUB_TITLE must be less than 128 characters long.");
    }
    
    /**
     * Title of the content. Max of 127 characters.
     * Localised titles might be in the Entries parameter as TITLE_xx, where xx is the regional code.
     */
    public string? Title
    {
        get => (string?)Entries.GetValueOrDefault("TITLE")?.Value;
        set => SaveStringToEntries("TITLE", value, s => s.Length < 128,
            "TITLE must be less than 128 characters long.", maxLength:0x080);
    }
    
    /**
     * Title ID of the content. Max of 15 characters.
     */
    public string? TitleId
    {
        get => (string?) Entries.GetValueOrDefault("TITLE_ID")?.Value;
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