using PSMetadataLib;
using PSMetadataLib.PS3;

namespace PS3GetInfo;

public class SFOReader
{
    public static void SFOMain(string path)
    {
        // Open the SFO file.
        PS3ParamSFO param = new(path);
        
        if (param.Title is not null)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{param.Title,-75}");
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
        
        Console.WriteLine();
        Console.WriteLine("PARAM.SFO contents: ------------------------------------------------------");
        
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"{"KEY",20}\t{"VALUE",-50}");
        Console.ResetColor();
        foreach (var key in param.Entries.Keys)
        {
            var value = param.Entries.GetValueOrDefault(key)?.ToString();
        
            if (value is not null)
            {
                if (value.Length > 50) value = value[..38] + " [truncated]";
            }
            
            Console.Write(
                $"{key,20}\t{value,-50}");
            Console.Write("\n");
        }
    }
}