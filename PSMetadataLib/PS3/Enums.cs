using System.ComponentModel;
using System.Reflection;

namespace PSMetadataLib.PS3;

[Flags]
public enum HybridFlagEnum : uint
{
    [Description("Disc Benefits"), ShortName("S")]
    DiscBenefits = 1U<<0,
    [ShortName("T")]
    Themes,
    [ShortName("V")]
    Video,
    [ShortName("g"), Description("Disc Game & Disc Extra Contents")]
    DiscGame,
    [ShortName("u"), Description("Firmware update")]
    FirmwareUpdate,
    [ShortName("v"), Description("Blu-ray Movie")]
    BluRayMovie
}

[Flags]
public enum AttributeEnum : uint
{
    PSPRemotePlayV1 = 1<<0,
    CopyProtected = 1<<0, // Save files
    SubfolderEnabled = 1<<0, // Subfolders
        
    PSPExport = 1<<1,
    PSPRemotePlayV2 = 1<<2,
    XMBInGameForcedEnabled = 1<<3,
    XMBInGameDisabled = 1<<4,
    XMBInGameBackgroundMusic = 1<<5,
    SystemVoiceChat = 1<<6, // dubious
    PSVitaRemotePlay = 1<<7,
    MoveControllerWarning = 1<<8,
    NavigationControllerWarning = 1<<9,
    PlayStationEyeWarning = 1<<10,
    MoveCalibrationNotification = 1<<11,
    Stereoscopic3DWarning = 1<<12,
    InstallDisc = 1<<16,
    InstallPackages = 1<<17,
    GamePurchaseEnabled = 1<<19,
    PCEngine = 1<<21,
    LicenseLogoDisabled = 1<<22,
    MoveControllerEnabled = 1<<23,
    NeoGeo = PCEngine | 1<<26,
}

[Flags]
public enum BootableEnum : uint
{
    NotBootable = 0,
    Mode1 = 1<<0,
    Mode2 = 1<<1
}

[Flags]
public enum SoundFormatEnum : uint
{
    [Description("LPCM 2.0")]
    Stereo = 1<<0, // LPCM 2.0
    [Description("LPCM 5.1")]
    Surround = 1<<2, // LPCM 5.1 Surround
    [Description("LPCM 7.1")]
    Surround7 = 1<<3, // LPCM 7.1 Surriound
    [Description("Dolby Digital 5.1")]
    DolbyDigital = 1<<8 | 1<<1, // Dolby Digital 5.1 + 2 for some reason
    [Description("Dolby Home Theatre 5.1")]
    DTS = 1<<9 | 1<<1,
        
    All = Stereo | Surround | Surround7 | DolbyDigital | DTS,
    LPCM = Stereo | Surround | Surround7
}

[Flags]
public enum ResolutionEnum : uint
{
    [Description("480")]
    NTSC = 1<<0,
    [Description("576")]
    PAL = 1<<1,
    [Description("720")]
    HD720 = 1<<2,
    [Description("1080")]
    HD1080 = 1<<3,
    [Description("480 16:9")]
    NTSCWidescreen = 1<<4,
    [Description("576 16:9")]
    PALWidescreen = 1<<5
}

public enum PS3ParamCategoryEnum : short
{
    // Content found on a Blu-ray disc
    [ShortName("DG"), Description("Disc game")]
    DiscGame,
    [ShortName("AR")]
    AutoinstallRoot,
    [ShortName("DP")]
    DiscPackages,
    [ShortName("IP")]
    InstallPackages,
    [ShortName("TR")]
    ThemeRoot,
    [ShortName("VR")]
    VideoRoot,
    [ShortName("VI")]
    VideoItem,
    [ShortName("XR")]
    ExtraRoot,
    [ShortName("TI")]
    ThemeItem,
    [ShortName("DM")]
    DiscMovie,
    
    // Content found on the hard drive.
    [ShortName("AP"), Description("App: Photos")]
    AppPhoto,
    [ShortName("AM"), Description("App: Music")]
    AppMusic,
    [ShortName("AV"), Description("App: Video")]
    AppVideo,
    [ShortName("BV")]
    BroadcastVideo,
    [ShortName("AT"), Description("App: TV")]
    AppTv,
    [ShortName("WT")]
    WebTv,
    [ShortName("HG"), Description("HDD Game")]
    HddGame,
    [ShortName("CB")]
    CellBe,
    [ShortName("AS")]
    AppStore,
    [ShortName("HM"), Description("PlayStation Home")]
    Home,
    [ShortName("SF"), Description("PlayStation Store")]
    StoreFrontend,
    [ShortName("2G"), Description("PlayStation 2 game -- PS2 compatible PS3s only.")]
    Ps2Game,
    [ShortName("2P"), Description("PS2 Classics")]
    Ps2Psn,
    [ShortName("1P"), Description("PS1 Classics")]
    Ps1Psn,
    [ShortName("MN"), Description("PSP Minis")]
    PspMinis,
    [ShortName("PE"), Description("PSP Remasters")]
    PspEmulator,
    [ShortName("PP")]
    Psp,
    [ShortName("GD"), Description("Game Data")]
    GameData,
    [ShortName("2D"), Description("PS2 Data")]
    Ps2Data,
    [ShortName("SD"), Description("Save Data")]
    SaveData,
    [ShortName("AM")]
    MemoryStick
}
