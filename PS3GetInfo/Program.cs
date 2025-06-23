// Gets information about a given PS3 game using its SFO file.

using System.Runtime.InteropServices.Marshalling;
using PSMetadataLib;
using PSMetadataLib.PS3;

if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to an SFO file.");
    Environment.Exit(99);
}

var path = args.FirstOrDefault();

if (!Path.Exists(path))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to an SFO file.");
    Environment.Exit(999);
}
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
Console.WriteLine($"{"KEY",20}\t{"Value",-50}");
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
