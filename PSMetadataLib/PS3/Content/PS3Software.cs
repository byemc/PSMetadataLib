namespace PSMetadataLib.PS3.Content;

/**
 * Represents most PS3 software.
 */
public class PS3Software : PS3Content
{
    // PARAMETERS
    /**
     * OPTIONAL The app version in XX.YY format as a string. Max 5 characters.
     */
    public string? AppVer
    {
        get => ParamSfo.AppVer;
        set => ParamSfo.AppVer = value;
    }

    /**
     * OPTIONAL 32 flags to activate features and boot modes (e.g. splash screens, remote play, etc). Use PS3.AttributeEnum for this.
     */
    public AttributeEnum? Attribute
    {
        get => ParamSfo.Attribute;
        set => ParamSfo.Attribute = value;
    }

    /**
     * REQUIRED: Specifies if content is bootable or not. Will default to Not Bootable if not specified.
     */
    public BootableEnum Bootable
    {
        get => ParamSfo.Bootable ?? BootableEnum.NotBootable;
        set => ParamSfo.Bootable = value;
    }

    /**
     * OPTIONAL
     */
    public string? ContentId
    {
        get => ParamSfo.ContentId;
        set => ParamSfo.ContentId = value;
    }

    /**
     * REQUIRED. Copyright text for the game.
     */
    public string License
    {
        get => ParamSfo.License ?? "";
        set => ParamSfo.License = value;
    }

    /**
     * OPTIONAL
     */
    public string? NpCommunicationId
    {
        get => ParamSfo.NpCommunicationId;
        set => ParamSfo.NpCommunicationId = value;
    }

    /**
     * REQUIRED. The parental control level for the content. Defaults to 1 (playable by everyone)
     */
    public int ParentalLevel
    {
        get => (int?)ParamSfo.ParentalLevel ?? 1;
        set => ParamSfo.ParentalLevel = (uint)value;
    }

    /**
     * REQUIRED. Minimum PS3 system version to run the game. In the format XX.YYYY.
     */
    public string PS3SystemVersion
    {
        get => ParamSfo.Ps3SystemVersion ?? "01.0000";
        set => ParamSfo.Ps3SystemVersion = value;
    }

    /**
     * OPTIONAL. Is currently a stub and will be added in a later library version.
     */
    public int RegionDeny
    {
        get;
        set;
    }

    /**
     * REQUIRED. Specifies what video modes the content supports. Will default to 480 4:3 if not specified.
     */
    public ResolutionEnum Resolution
    {
        get => ParamSfo.Resolution ?? ResolutionEnum.NTSC;
        set => ParamSfo.Resolution = value;
    }

    /**
     * REQUIRED. Specifies what audio modes the content supports. Will default to LPCM 2.0 if not specified.
     */
    public SoundFormatEnum SoundFormat
    {
        get => ParamSfo.SoundFormat ?? SoundFormatEnum.Stereo;
        set => ParamSfo.SoundFormat = value;
    }
    
    /*
     * REQUIRED. The main title of the content. Will default to a blank string.
     */
    public string Title
    {
        get => ParamSfo.Title ?? "";
        set => ParamSfo.Title = value;
    }

    /**
     * REQUIRED. The disc revision or package revision in XX.YY format. Different from AppVer.
     */
    public string Version
    {
        get => ParamSfo.Version ?? "01.00";
        set => ParamSfo.Version = value;
    }
}
