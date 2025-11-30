// See https://aka.ms/new-console-template for more information

using PSMetadataLib;
using PSMetadataLib.PS4;

if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to PS4 content. (Nothing provided)");
    Environment.Exit(98);
}

var path = args.FirstOrDefault();

if (!Path.Exists(path))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to PS4 content. (Non-existent path provided)");
    Environment.Exit(98);
}

if (
    !File.Exists(path) &&
    !File.Exists(Path.Join(path, "param.sfo"))
)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to PS4 content. (Invalid path provided)");
    Environment.Exit(99);
}

// Attempt to load the SFO file.
if (File.Exists(path) || File.Exists(Path.Join(path, "param.sfo")))
{
    var toJoin = !File.Exists(Path.Join(path, "param.sfo")) ? "" : "param.sfo";
    var tmp = new PS4ParamSFO(Path.Join(path, toJoin));
    
    Console.WriteLine(tmp.Title);
    Console.WriteLine($"{"APP TYPE",12}\t{tmp.AppType?.ToDescriptions()}");
    Console.WriteLine($"{"ATTRIBUTE",12}\t{tmp.Attribute?.ToDescriptions()}");
    Console.WriteLine($"{"ATTRIBUTE2",12}\t{tmp.Attribute2?.ToDescriptions()}");
    Console.WriteLine($"{"CATEGORY",12}\t{tmp.Category}");
    Console.WriteLine($"{"CONTENT_ID",12}\t{tmp.ContentId}");

    return;
}