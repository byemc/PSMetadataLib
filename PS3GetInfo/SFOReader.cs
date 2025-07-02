using PSMetadataLib;
using PSMetadataLib.PS3;

namespace PS3GetInfo;

public class SFOReader
{
    public static void SFOMain(string path)
    {
        // Get the base directory of the given path.
        var directory = Path.GetDirectoryName(path);
        
        // Open the SFO file.
        PS3ParamSFO param = new(path);
        
        if (param.Title is not null)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{param.Title,-80}");
            Console.ResetColor();
        }
        if (param.SubTitle is not null)
        {
            Console.WriteLine($"{param.SubTitle}");
        }
        if (param.Detail is not null)
        {
            Console.WriteLine($"{param.Detail}");
        }
        
        if (param.Category is not null)
        {
            Console.WriteLine(param.Category.GetDescription());
        }
        
        Console.WriteLine();

        var hasBonusPackages = false;
        if (param.Attribute is not null)
        {
            Console.WriteLine("SFO Attributes:");
            Console.WriteLine($"\t{param.Attribute.ToDescriptions()}");
            if ((param.Attribute & AttributeEnum.InstallPackages) != 0) hasBonusPackages = true;
        }
        if (param.Resolution is not null)
        {
            Console.WriteLine("Supported resolutions:");
            Console.WriteLine($"\t{param.Resolution.ToDescriptions()}");
        }
        if (param.SoundFormat is not null)
        {
            Console.WriteLine("Supported sound formats:");
            Console.WriteLine($"\t{param.SoundFormat.ToDescriptions()}");
        }

        if (hasBonusPackages)
        {
            Console.WriteLine();
            PS3ParamSFO bonusPackagesParam = new(Path.Join(directory, "PKGDIR", "PARAM.SFO"));
            Console.WriteLine($"Bonus packages: {bonusPackagesParam.Title}");
            // Iterate between numbers 0 and 99 until we can't find a directory titled PKG{ii} (0-9 are prefixed with 0)
            for (var i = 0; i < 100; i++)
            {
                var pkgNumber = "PKG" + (i.ToString("00"));
                var pkgDir = Path.Join(directory, "PKGDIR", pkgNumber);
                var pkgExists = Path.Exists(pkgDir);

                if (!pkgExists)
                    break;

                PS3ParamSFO pkg = new(Path.Join(pkgDir, "PARAM.SFO"));
                
                Console.WriteLine($"\t- {pkgNumber}\t{pkg.Title} ({pkg.Category?.GetDescription() ?? "Unknown category"})");
            }
        }
        
        Console.WriteLine();
        Console.WriteLine("PARAM.SFO contents: ------------------------------------------------------");
        
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"KEY",20}\t{"VALUE",-55}");
        Console.ResetColor();
        foreach (var key in param.Entries.Keys)
        {
            var value = param.Entries.GetValueOrDefault(key)?.ToString();
        
            if (value is not null)
            {
                if (value.Length > 55) value = value[..55];
            }
            
            Console.Write(
                $"{key,20}\t{value,-55}");
            Console.Write("\n");
        }
    }
}