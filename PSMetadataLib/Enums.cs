using System.ComponentModel;
using System.Net.Mime;
using System.Reflection;
using PSMetadataLib.PS3;

namespace PSMetadataLib;

public enum LanguagesEnum : uint
{
    [SupportLevel()]                                                                                                    // Everything
    Japanese,
    [SupportLevel(), Description("English (United States)")]
    English,
    [SupportLevel(~SystemSupportEnum.FifthGen)]                                                                         // PS2, PS3, PS Vita, PS4, PS5
    French,
    [SupportLevel(~SystemSupportEnum.FifthGen), Description("Spanish (Spain)")]
    Spanish,
    [SupportLevel(~SystemSupportEnum.FifthGen)]
    German,
    [SupportLevel(~SystemSupportEnum.FifthGen)]
    Italian,
    [SupportLevel(~SystemSupportEnum.FifthGen), Description("Portuguese (Portugal)")]
    Portuguese,
    [SupportLevel(~SystemSupportEnum.FifthGen)]
    Russian,
    [SupportLevel(~SystemSupportEnum.FifthGen)]
    Korean,
    [SupportLevel(~SystemSupportEnum.FifthGen), Description("Chinese (Traditional)")]
    ChineseTraditional,
    [SupportLevel(~SystemSupportEnum.FifthGen), Description("Chinese (Simplified)")]
    ChineseSimplified,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]                                          // PS3, PS Vita, PS4, PS5
    Finnish,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]
    Swedish,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]
    Danish,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]
    Norwegian,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]
    Polish,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen)), Description("Portuguese (Brazil)")]
    PortugueseBrazil,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen)), Description("English (United Kingdom)")]
    EnglishUnitedKingdom,
    [SupportLevel(~(SystemSupportEnum.FifthGen | SystemSupportEnum.SixthGen))]
    Turkish,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen), Description("Spanish (Latin America)")]   // PS4 & PS5
    SpanishLatinAmerica,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Arabic,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen), Description("French (Canada)")]
    FrenchCanada,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Czech,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Hungarian,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Greek,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Romanian,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Thai,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Vietnamese,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Indonesian,
    [SupportLevel(SystemSupportEnum.SeventhGen | SystemSupportEnum.NinthGen)]
    Ukrainian,
}

public class ShortNameAttribute(string shortName) : Attribute
{
    public readonly string ShortName = shortName;
}

[Flags]
public enum SystemSupportEnum : long
{
    [Description("PlayStation 1")]
    PSX = 1<<0,
    [Description("PlayStation 2")]
    PS2 = 1<<1,
    [Description("PlayStation Portable")]
    PSP = 1<<2,
    [Description("PlayStation 3")]
    PS3 = 1<<3,
    [Description("PlayStation Vita")]
    PSVita = 1<<4,
    [Description("PlayStation 4")]
    PS4 = 1<<5,
    [Description("PlayStation 5")]
    PS5 = 1<<6,
    
    All = PSX | PS2 | PS3 | PS4 | PS5 | PSP | PSVita,
    FifthGen = PSX,
    SixthGen = PS2 | PSP,
    SeventhGen = PS3 | PSVita,
    EightGen = PS4,
    NinthGen = PS5
} 

public class SupportLevelAttribute(SystemSupportEnum supportLevel = SystemSupportEnum.All) : Attribute
{
    public readonly SystemSupportEnum SupportLevel = supportLevel;
}

public static class EnumExtensions
{
    public static string ToDescriptions(this Enum value)
    {
        var type = value.GetType();
        var values = Enum.GetValues(type).Cast<Enum>();

        var selectedFlags = values
            .Where(value.HasFlag)
            .Where(v => Convert.ToInt32(v) != 0).ToArray(); // Exclude 0 unless it's the only one

        if (selectedFlags.Length == 0 && Convert.ToInt32(value) == 0)
        {
            // Handle 0 (e.g. None) explicitly if it's the only value
            return value.GetDescription();
        }

        return string.Join(", ", selectedFlags.Select(v => v.GetDescription()));
    }

    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }
    
    public static string GetShortName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<ShortNameAttribute>();
        return attr?.ShortName ?? value.ToString();
    }
}
