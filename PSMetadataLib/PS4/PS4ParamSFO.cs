using System.Reflection;
using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regex;
using PSMetadataLib.Filetypes;
using PSMetadataLib.PS4.Enums;
using PSMetadataLib.Types;

namespace PSMetadataLib.PS4;

/// <summary>
/// Represents a PlayStation 4 PARAM.SFO file. <br />
/// Much of the details for the fields and properties of this class are thanks to the PS Developer Wiki: https://www.psdevwiki.com/ps4/param.sfo
/// </summary>
/// <seealso cref="SfoFile" />
/// <remarks>This is unfinished!!</remarks>
public class PS4ParamSFO : SfoFile
{
    /**
     * The title of the content. Max length 127 characters.
     *
     * Maps to TITLE.
     */
    public string? Title
    {
        get => (string?)Entries.GetValueOrDefault("TITLE")?.Value;
        set => SaveStringToEntries("TITLE", value, s => s.Length < 128,
            "TITLE must be less than 128 characters long.", maxLength: 0x080);
    }

    /**
     * Title ID of the content. Max of 15 characters.
     */
    public string? TitleId
    {
        get => (string?)Entries.GetValueOrDefault("TITLE_ID")?.Value;
        set => SaveStringToEntries("TITLE_ID", value, s => s.Length < 16,
            "TITLE_ID must be less than 16 characters long.");
    }

    public AppTypeEnum? AppType
    {
        get => Entries.TryGetValue("APP_TYPE", out var fmt)
            ? (AppTypeEnum)fmt.Value
            : null;
        set => SaveValueToEntries("APP_TYPE", (uint?)value);
    }
    
    public string? AppVer
    {
        get => (string?)Entries.GetValueOrDefault("APP_VER")?.Value;
        set => SaveStringToEntries("APP_VER", value, v => v.Length == 5 && IsMatch(v, _appVerRegex.Pattern),
            "APP_KEY must be 5 characters long and in the format XX.YY.", maxLength:0x8);
    }
    private readonly GeneratedRegexAttribute _appVerRegex = new GeneratedRegexAttribute(@"/\d{2}[.]\d{2}/");

    public AttributeEnum? Attribute
    {
        get => Entries.TryGetValue("ATTRIBUTE", out var fmt)
            ? (AttributeEnum)fmt.Value
            : null;
        set => SaveValueToEntries("ATTRIBUTE", (uint?)value);
    }

    public Attribute2Enum? Attribute2
    {
        get => Entries.TryGetValue("ATTRIBUTE2", out var fmt)
            ? (Attribute2Enum)fmt.Value
            : null;
        set => SaveValueToEntries("ATTRIBUTE2", (uint?)value);
    }

    public CategoryEnum? Category
    {
        get
        {
            var cat = Entries.TryGetValue("CATEGORY", out var o)
                ? (string)o.Value
                : null;
            return cat is null ? null : MatchCategoryCodeToEnum(cat);
        }
        set => SaveStringToEntries("CATEGORY", value?.GetShortName(), maxLength: 0x4);
    }

    public static CategoryEnum? MatchCategoryCodeToEnum(string shortCode)
    {
        var matchedValue = typeof(CategoryEnum)
            .GetFields()
            .FirstOrDefault(f => f.IsLiteral && shortCode == f.GetCustomAttribute<ShortNameAttribute>()!.ShortName);

        var unboxedValue = matchedValue?.GetRawConstantValue();

        if (unboxedValue is null)
        {
            return null;
        }

        return (CategoryEnum)unboxedValue;
    }

    // TODO: Allow setting
    public ContentId? ContentId => new((string?)Entries.GetValueOrDefault("CONTENT_ID")?.Value);

    public string? Version
    {
        get => (string?)Entries.GetValueOrDefault("VERSION")?.Value;
        set => SaveStringToEntries("VERSION", value, s => s.Length < 7,
            "VERSION must be 6 characters or less long.");
    }

    /**
     * Create PS3ParamSFO file using information from a PARAM.SFO file given in `path`
     */
    public PS4ParamSFO(string path)
    {
        // Loads the entries from the PARAM.SFO into Entries.
        Load(path);

        if (Length <= 0)
        {
            throw new FileLoadException("Could not find any parameters in the PARAM.SFO file.");
        }
    }

    public PS4ParamSFO()
    {
    }
}