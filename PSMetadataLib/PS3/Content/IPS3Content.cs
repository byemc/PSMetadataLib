
namespace PSMetadataLib.PS3.Content;

/**
 * Interface for classes that represent content on the PS3 XMB.
 */
public interface IPS3Content
{
    public static IPS3Content CreateContentFromPath(string path)
    {
        PS3ParamSFO sfoFile = new(Path.Join(path, "PARAM.SFO"));
        switch (sfoFile.Category)
        {
            case PS3ParamCategoryEnum.DiscGame:
            case PS3ParamCategoryEnum.HddGame:
                return new PS3Game(path);
            case PS3ParamCategoryEnum.InstallPackage:
                return new PS3InstallPackage(path);
            default:
                return new PS3Content(path);
        }
    }
    
    /**
     * Location of the content on the disc
     */
    public string Location { get; set; }
    
    /**
     * Title ID of the content. Required to the PARAM.SFO file.
     */
    public string TitleId { get; set; }
    
    public string Title { get; set; }
    
    public PS3ParamCategoryEnum Category { get; }

    /**
     * Returns true if ICON0.PNG is available in Location.
     * The icon is required by the XMB.
     */
    public bool HasIcon { get; }

    /**
     * Returns true if the icon video file (ICON1.PAM) is present.
     */
    public bool HasIconVideo { get; }
    /**
     * Returns true if the icon sound file (SND0.AT3) is present.
     */
    public bool HasIconSound { get; }
}
