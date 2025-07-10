using PSMetadataLib.PS3.Content;
using PSMetadataLib.PS3.Interfaces;

namespace PSMetadataLib.PS3;

/**
 * Represents the root of a PS3's filesystem. Initialise with a path to the root of a PS3's filesystem and you can auto-discover content on it.
 * (Note: use PS3HardDrive for the root of the PS3's hard drive. This is for the filesystem as in on RPCS3 or when FTPing to a running PS3.
 */
public class PS3RootFilesystem : IPS3Directory
{
    public string Directory { get; set; }

    private PS3HardDrive _hardDrive0;
    private PS3BluRayDisc _bluRayDisc;
    
    /**
     * Compiles a list of content in this PS3's filesystem. Will not keep track of who owns save data!!
     */
    public List<IPS3Content> Scan()
    {
        List<IPS3Content> output = [];
        
        output.AddRange(_hardDrive0.Scan());
        output.AddRange(_bluRayDisc.Scan());
        
        return output;
    }
    
    public PS3RootFilesystem(string path)
    {
        Directory = path;
        _hardDrive0 = new PS3HardDrive(Path.Join(Directory, "dev_hdd0"));
        _bluRayDisc = new PS3BluRayDisc(Path.Join(Directory, "dev_bdvd"));
    }
}