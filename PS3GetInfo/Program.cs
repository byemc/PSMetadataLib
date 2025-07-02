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
            Console.WriteLine("[!] Please provide a path to PS3 content.");
            Environment.Exit(99);
        }
        
        var path = args.FirstOrDefault();
        
        if (!Path.Exists(path))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to PS3 content.");
            Environment.Exit(99);
        }

        if (!Path.Exists(Path.Join(path, "PARAM.SFO"))) return;
        
        // Attempt to load the SFO file.
        var tmp = new PS3ParamSFO(Path.Join(path, "PARAM.SFO"));
        
        if (tmp.Category is PS3ParamCategoryEnum.HddGame or PS3ParamCategoryEnum.DiscGame)
        {
            GameProgram.Main(path);
        }

    }
}
