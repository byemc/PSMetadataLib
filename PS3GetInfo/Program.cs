// Gets information about given PS3 content.

using PSMetadataLib.PS3;

namespace PS3GetInfo;

public class Program
{
    public static void Main(string[] args)
    {
        if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to PS3 content. (Nothing provided)");
            Environment.Exit(98);
        }
        
        var path = args.FirstOrDefault() ?? "";
        var pathLc = path.ToLower();
        
        if (!Path.Exists(path))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to PS3 content. (Non-existent path provided)");
            Environment.Exit(98);
        }

        if (
            !Path.GetFileName(pathLc).Equals("param.sfo") &&
            !Path.GetFileName(pathLc).Equals("ps3_disc.sfb") &&
            !File.Exists(Path.Join(pathLc, "param.sfo")) &&
            !File.Exists(Path.Join(pathLc, "ps3_disc.sfb")) &&
            !Directory.Exists(Path.Join(pathLc, "dev_hdd0"))
        )
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to PS3 content. (Invalid path provided)");
            Environment.Exit(99);
        }

        if (Directory.Exists(Path.Join(path, "dev_hdd0")))
        {
            var filesystem = new PS3RootFilesystem(path);
            FileSystemReader.Main(filesystem);
            return;
        }

        // Attempt to load the SFO file.
        if (Path.GetFileName(pathLc).Equals("param.sfo") || File.Exists(Path.Join(pathLc, "param.sfo")))
        {
            var tmp = new PS3ParamSFO(path);

            if (tmp.Category is PS3ParamCategoryEnum.HddGame or PS3ParamCategoryEnum.DiscGame)
            {
                GameProgram.Main(path);
            }
            else
            {
                SFOReader.SFOMain(path);
            }

            return;
        }
        
        if (Path.GetFileName(path).Equals("PS3_DISC.SFB") || File.Exists(Path.Join(path, "PS3_DISC.SFB")))
        {
            var toJoin = Path.GetFileName(path).Equals("PS3_DISC.SFB") ? "" : "PS3_DISC.SFB";
            SfbReader.SfbMain(Path.Join(path, toJoin));
            return;
        }
    }
}