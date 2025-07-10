using PSMetadataLib;
using PSMetadataLib.PS3;
using PSMetadataLib.PS3.Content;

namespace PS3GetInfo;

public class FileSystemReader
{
    public static void Main(PS3RootFilesystem filesystem)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[!] This program will only find content if it's in its proper location (e.g. /dev_hdd0/game)");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine("Content on PS3 filesystem:");
        var contents = filesystem.Scan();
        contents.Sort((a, b) => a.Category.GetDescription().CompareTo(b.Category.GetDescription()));
        var current = (PS3ParamCategoryEnum)9999;
        foreach (var content in contents)
        {
            if (current != content.Category)
            {
                current = content.Category;
                Console.WriteLine($"- {current.GetDescription()}:");
            }

            var details = new List<string>();
            
            if (content is PS3Game game)
            {
                details.Add($"Attribute: {game.Attribute?.ToDescriptions()}");
                if (game.IsPackageInstaller)
                {
                    PS3ParamSFO bonusPackagesParam = new(Path.Join(content.Location, "PKGDIR", "PARAM.SFO"));
                    details.Add($"Bonus packages: {bonusPackagesParam.Title}");
                }
            }
            
            details.Add(content.GetType().ToString());

            var prefix = details.Count > 0 ? "-" : "+";
            
            Console.WriteLine($"|\t{prefix} {content.Title}");
            foreach (var detail in details)
            {
                Console.WriteLine($"|\t|\t- {detail}");
            }
        }
    }
}