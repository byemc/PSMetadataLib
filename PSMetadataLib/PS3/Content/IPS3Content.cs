
namespace PSMetadataLib.PS3.Content;

/**
 * Interface for classes that represent content on the PS3 XMB.
 */
public interface IPS3Content
{
    /**
     * Location of the content on the disc
     */
    public string Location { get; set; }
    
    /**
     * Title ID of the content. Required to the PARAM.SFO file.
     */
    public string TitleId { get; set; }

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
