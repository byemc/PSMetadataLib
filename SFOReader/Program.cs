// Quick lil sample program that takes an SFO file as an argument

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

foreach (var sfoEntry in sfo.Entries)
{
    Console.WriteLine($"{sfoEntry.Key}\t{sfoEntry.Value}");
}
