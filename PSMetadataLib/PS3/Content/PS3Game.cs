using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace PSMetadataLib.PS3.Content;

/**
 * Represents a game on the hard drive of a PS3.
 */
public class PS3Game : PS3Software
{
    public bool IsDiscGame
    {
        get => ParamSfo.Category.Equals(PS3ParamCategoryEnum.DiscGame);
        set => ParamSfo.Category = value ? PS3ParamCategoryEnum.DiscGame : PS3ParamCategoryEnum.HddGame;
    }
    
    public PS3Game(string path)
    {
        // Check if there's a PARAM.SFO file.
        var paramSfoPath = Path.Join(path, "PARAM.SFO");
        if (Path.Exists(paramSfoPath))
            ParamSfo = new PS3ParamSFO(paramSfoPath);
        else
            throw new Exceptions.InvalidPS3ContentException("The given path wasn't valid PS3 content.");
        
        Location = path;
    }

    // Extra stuff
    /**
     * Returns true if ATTRIBUTE contains the InstallPackages flag.
     */
    public bool IsPackageInstaller => (Attribute & AttributeEnum.InstallPackages) != 0;

    /**
     * Returns a list of packages included with the game (in %GAMEDIR%/PKGDIR)
     */
    public List<PS3InstallPackage> Packages {
        get
        {
            var results = Directory.GetDirectories(Path.Join(Location, "PKGDIR"), "PKG??");
            List<PS3InstallPackage> output = [];
            output.AddRange(results.Select(result => new PS3InstallPackage(result)));
            return output;
        }    
    } 
    public PS3Game()
    {
        ParamSfo = new PS3ParamSFO();
    }
} 