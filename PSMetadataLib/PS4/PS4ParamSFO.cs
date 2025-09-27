using System.Reflection;
using PSMetadataLib.Filetypes;
using PSMetadataLib.PS4.Enums;
using PSMetadataLib.Types;

namespace PSMetadataLib.PS4;

/**
 * Represents a PlayStation 4 PARAM.SFO file.
 *
 * Much of the details for the fields and properties of this class are thanks to the PS Developer Wiki: https://www.psdevwiki.com/ps4/param.sfo
 */
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

    public AttributeEnum? Attribute
    {
        get => Entries.TryGetValue("APP_TYPE", out var fmt)
            ? (AttributeEnum)fmt.Value
            : null;
        set => SaveValueToEntries("APP_TYPE", (uint?)value);
    }

    public Attribute2Enum? Attribute2
    {
        get => Entries.TryGetValue("APP_TYPE", out var fmt)
            ? (Attribute2Enum)fmt.Value
            : null;
        set => SaveValueToEntries("APP_TYPE", (uint?)value);
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

    public ContentId? ContentId
    {
        get { return new ContentId((string?)Entries.GetValueOrDefault("CONTENT_ID")?.Value); }
    }

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