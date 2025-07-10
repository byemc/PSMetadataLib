using PSMetadataLib.PS3.Content;
using PSMetadataLib.PS3.Interfaces;

namespace PSMetadataLib.PS3;

public class PS3HardDrive : IPS3Directory
{
    public string Directory { get; set; }

    public List<IPS3Content> Scan()
    {
        // TODO: Add support for save data.
        List<IPS3Content> output = [];
        
        var hddGamePaths = System.IO.Directory.GetDirectories(Path.Join(Directory, "game"));
        List<IPS3Content> hddGames = [];
        foreach (var hddGamePath in hddGamePaths)
        {
            if (!File.Exists(Path.Join(hddGamePath, "PARAM.SFO")))
                continue;
            
            hddGames.Add(IPS3Content.CreateContentFromPath(hddGamePath));
        }
        
        output.AddRange(hddGames);

        return output;
    }
    
    public PS3HardDrive(string path)
    {
        Directory = path;
    }
}