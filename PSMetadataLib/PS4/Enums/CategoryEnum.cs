using System.ComponentModel;

namespace PSMetadataLib.PS4.Enums;

// This is shared with the PSVita.
public enum CategoryEnum : short
{
    [ShortName("ac")]
    AdditionalContent,
    [ShortName("bd"), Description("Blu-Ray Disc")]
    BlurayDisc,
    [ShortName("gc")]
    GameContent,
    [ShortName("gd"), Description("Digital Game Application")]
    GameDigitalApplication,
    [ShortName("gda")]
    SystemApplication,
    [ShortName("gdb")]
    Unknown1,
    [ShortName("gdc")]
    NonGameBigApplication,
    [ShortName("gdd")]
    BGApplication,
    [ShortName("gde")]
    MiniApp,
    [ShortName("gdg")]
    CommonDialog,
    [ShortName("gdk")]
    VideoServiceWebApp,
    [ShortName("gdl")]
    PSCloudBetaApp,
    [ShortName("gdO")]
    PS2Classic,
    [ShortName("gp")]
    GameApplicationPatch,
    [ShortName("gpc")]
    MiniAppPatch,
    [ShortName("gpd")]
    VideoServiceWebAppPatch,
    [ShortName("gpe")]
    PSCloudBetaAppPatch,
    [ShortName("sd")]
    SaveData,
    [ShortName("la")]
    LicenseArea,
    [ShortName("wda")]
    Unknown2
}