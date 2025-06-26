
using PSMetadataLib;

namespace PS3GetInfo;

using PSMetadataLib.PS3;

public class SfbReader
{
    public static void SfbMain(string path)
    {
        Console.WriteLine("Reading PS3_DISC.SFB");

        var rootFolder = Path.GetDirectoryName(path) ?? "";
        List<string> validSfos = [];
        var sfb = new PS3DiscSFBFile(path);
        
        Console.WriteLine();
        Console.WriteLine($"Title ID: {sfb.TitleId}");
        Console.WriteLine($"Flags: {sfb.HybridFlagFlags.ToDescriptions()}");
        Console.WriteLine($"Param.SFOs:");
        foreach (var sfoLocation in sfb.SfoLocations)
        {
            var exists = Path.Exists(Path.Combine(rootFolder, sfoLocation));

            if (!exists)
                Console.ForegroundColor = ConsoleColor.Red;
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                validSfos.Add(Path.Combine(rootFolder, sfoLocation));
            }
            Console.WriteLine($"\t{Path.Combine(rootFolder, sfoLocation)}");
            Console.ResetColor();
        }
        
        Console.WriteLine("Attempting to read first PARAM.SFO file...");
        Console.WriteLine();
        
        SFOReader.SFOMain(validSfos.FirstOrDefault() ?? "");
    }
}