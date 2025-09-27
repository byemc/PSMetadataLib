using System.ComponentModel;

namespace PSMetadataLib.PS4.Enums;

public enum AppTypeEnum : uint
{
    [Description("Not Specified")]
    NotSpecified = 0,
    [Description("Paid Standalone Full App")]
    PaidStandaloneFullApp = 1,
    [Description("Upgradable App")]
    UpgradableApp = 2,
    [Description("Demo App")]
    DemoApp = 3,
    [Description("Freemium App")]
    FreemiumApp = 4
}