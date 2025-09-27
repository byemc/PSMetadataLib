namespace PSMetadataLib.PS4.Enums;

[Flags]
public enum AttributeEnum: uint
{
    InitialUserLogout = 1<<0,
    CrossButtonForCommonDialog = 1<<1,
    PSMoveWarningDialogMenuOtion = 1<<2,
    SupportsStereoscopic3D = 1<<3,
    SuspendedWhenPSButtonIsPressed = 1<<4,
    SystemButtonForCommonDialog = 1<<5,
    OverridesShareMenu = 1<<6,
    SuspendedWhenSpecialResolutionAndPSButtonIsPressed = 1<<9,
    HDCPEnabled = 1<<10,
    HDCPDisabledNonGame = 1<<11,
    PSVRSupported = 1<<15,
    SixCPUMode = 1<<16,
    SevenCPUMode = 1<<17,
    NEOModeSupported = 1<<24,
    PSVRRequired = 1<<27,
    HDRSupported = 1<<30,
}

// I'm putting ATTRIBUTE2 here too because I'm lazy
[Flags]
public enum Attribute2Enum : uint
{
    SupportsVideoRecording = 1<<2,
    SupportsContentSearch = 1<<3,
    PSVRPersonalEyeToEyeDistanceDisabled = 1<<4,
    PSVRPersonalEyeToEyeDistanceDynamicallyChangable = 1<<5,
    SupportsBroadcastSeperateMode = 1<<9,
    SupportsOneToOneMatchOldSDK = 1<<12,
    SupportsTeamOnTeamTournamentOldSDK = 1<<13
}