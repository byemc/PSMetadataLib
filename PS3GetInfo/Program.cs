// Gets information about given PS3 content.

using PS3GetInfo;
using PSMetadataLib.Filetypes;
using PSMetadataLib.PS3;

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
        
        var path = args.FirstOrDefault();
        
        if (!Path.Exists(path))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to PS3 content. (Non-existent path provided)");
            Environment.Exit(98);
        }

        if (
            !Path.GetFileName(path).Equals("PARAM.SFO") &&
            !Path.GetFileName(path).Equals("PS3_DISC.SFB") &&
            !File.Exists(Path.Join(path, "PARAM.SFO")) &&
            !File.Exists(Path.Join(path, "PS3_DISC.SFB")) &&
            !Directory.Exists(Path.Join(path, "dev_hdd0"))
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
        if (Path.GetFileName(path).Equals("PARAM.SFO") || File.Exists(Path.Join(path, "PARAM.SFO")))
        {
            var toJoin = Path.GetFileName(path).Equals("PARAM.SFO") ? "" : "PARAM.SFO";
            var tmp = new PS3ParamSFO(Path.Join(path, toJoin));

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
