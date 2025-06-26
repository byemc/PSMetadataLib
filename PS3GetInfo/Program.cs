// Gets information about a given PS3 game using its SFO file.

using PS3GetInfo;

public class Program
{
    public static void Main(string[] args)
    {
        if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to a PS3 metadata file. (PARAM.SFO or PS3_DISC.SFB)");
            Environment.Exit(99);
        }
        
        var path = args.FirstOrDefault();
        
        if (!Path.Exists(path))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[!] Please provide a path to a PS3 metadata file. (PARAM.SFO or PS3_DISC.SFB)");
            Environment.Exit(99);
        }

        switch (PSMetadataLib.Filetypes.Identifier.Identify(path))
        {
            case "SFO":
                SFOReader.SFOMain(path);
                break;
            case "SFB":
                SfbReader.SfbMain(path);
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[!] Please provide a path to a PS3 metadata file. (PARAM.SFO or PS3_DISC.SFB)");
                Environment.Exit(99);
                break;
        }
    }
}
