// Quick lil sample program that takes an SFO file as an argument and dumps out a table of information.

using PSMetadataLib;

if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to an SFO file.");
    Environment.Exit(99);
}

var sfoPath = args.FirstOrDefault();

if (!Path.Exists(sfoPath))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to an SFO file.");
    Environment.Exit(999);
}

// Open the SFO file.
SfoFile sfo = new(sfoPath);

Console.BackgroundColor = ConsoleColor.Blue;
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"{"KEY",20}\t{"Value",-50}");
Console.ResetColor();
foreach (var key in sfo.Entries.Keys)
{
    var value = sfo.Entries.GetValueOrDefault(key)?.ToString();

    if (value is not null)
    {
        if (value.Length > 50) value = value[..38] + " [truncated]";
    }
    
    Console.Write(
        $"{key,20}\t{value,-50}");
    Console.Write("\n");
}
