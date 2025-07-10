using PSMetadataLib.PS3.Content;

namespace PSMetadataLib.PS3.Interfaces;

/**
 * Represents a directory in a PlayStation 3's filesystem.
 */
public interface IPS3Directory
{
    public string Directory { get; set; }

    public List<PS3Content> Scan()
    {
        return [];
    }
}