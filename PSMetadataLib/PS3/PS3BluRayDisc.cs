using PSMetadataLib.PS3.Content;
using PSMetadataLib.PS3.Interfaces;

namespace PSMetadataLib.PS3;

public class PS3BluRayDisc(string path) : IPS3Directory
{
    public string Directory { get; set; } = path;

    public List<IPS3Content> Scan()
    {
        if (!System.IO.Directory.Exists(Path.Join(Directory, "PS3_DISC.SFB")))
            return [];
        var sfbFile = new PS3DiscSFBFile(Path.Join(Directory, "PS3_DISC.SFB"));
        var locations = sfbFile.GetExistingSfoLocations(Directory);

        return locations.Select(location => IPS3Content.CreateContentFromPath(Path.GetDirectoryName(location)!)).ToList();
    }
}