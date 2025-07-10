namespace PSMetadataLib.PS3.Content;

public class PS3Content : IPS3Content
{
    public string Location { get; set; }
    
    // Metadata related
    
    /**
     * Returns true if ICON0.PNG is available in Location.
     * The icon is required by the XMB.
     */
    public bool HasIcon => Path.Exists(Path.Join(Location, "ICON0.PNG"));

    /**
     * Returns true if the icon video file (ICON1.PAM) is present.
     */
    public bool HasIconVideo => Path.Exists(Path.Join(Location, "ICON1.PAM"));

    /**
     * Returns true if the icon sound file (SND0.AT3) is present.
     */
    public bool HasIconSound => Path.Exists(Path.Join(Location, "SND0.AT3"));

    public bool HasBackgroundImg => Path.Exists(Path.Join(Location, "PIC1.PNG"));
    
    // Parameters
    /**
     * REQUIRED. The title ID of the content. Will default to a blank string.
     */
    public string TitleId
    {
        get => ParamSfo.TitleId ?? "";
        set => ParamSfo.TitleId = value;
    }
    
    public string Title
    {
        get => ParamSfo.Title ?? "";
        set => ParamSfo.Title = value;
    }
    
    public AttributeEnum Attribute
    {
        get => ParamSfo.Attribute ?? 0;
        set => ParamSfo.Attribute = value;
    }
    
    /**
     * REQUIRED: Specifies the category of the content.
     */
    public PS3ParamCategoryEnum Category => ParamSfo.Category ?? PS3ParamCategoryEnum.HddGame;
    
    internal PS3ParamSFO ParamSfo;

    public PS3Content(string path)
    {
        // Check if there's a PARAM.SFO file.
        var paramSfoPath = Path.Join(path, "PARAM.SFO");
        if (Path.Exists(paramSfoPath))
            ParamSfo = new PS3ParamSFO(paramSfoPath);
        else
            throw new Exceptions.InvalidPS3ContentException("The given path wasn't valid PS3 content.");
        
        Location = path;
    }

    public PS3Content()
    {
        ParamSfo = new PS3ParamSFO();
    }
}