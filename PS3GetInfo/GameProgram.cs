using PSMetadataLib;
using PSMetadataLib.PS3.Content;

namespace PS3GetInfo;

public class GameProgram
{
    private static void PrintColouredBool(string label, bool boolean)
    {
        Console.Write($"{label}\t");
        if (boolean)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("YES");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NO");
        }
        Console.ResetColor();
    }
    
    public static void Main(string gamePath)
    {
        var game = new PS3Game(gamePath);
        Console.WriteLine($"{game.Title} ({game.TitleId})");
        Console.WriteLine($"{game.Category.GetDescription()}");
        Console.WriteLine($"Attributes: {game.Attribute?.ToDescriptions() ?? "None"}");
        var packages = game.Packages;
        if (packages.Count > 0)
        {
            Console.WriteLine($"Packages:");
            foreach (var package in packages)
            {
                Console.WriteLine($"\t- {package.Title} ({package.TitleId})");
            }
        }
        Console.WriteLine();

        Console.WriteLine("Metadata:");
        PrintColouredBool("\tHas icon video:", game.HasIconVideo);
        PrintColouredBool("\tHas icon sound:", game.HasIconSound);
        PrintColouredBool("\tHas XMB bkg:", game.HasBackgroundImg);
    }
}