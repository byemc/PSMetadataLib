// See https://aka.ms/new-console-template for more information

using System.Numerics;
using System.Text;
using PSMetadataLib;

if (string.IsNullOrWhiteSpace(args.FirstOrDefault()))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to a PS2 Memory Card. (Nothing provided)");
    Environment.Exit(98);
}

var path = args.FirstOrDefault();

if (!File.Exists(path))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[!] Please provide a path to a PS2 Memory Card. (Non-existent path provided)");
    Environment.Exit(98);
}

Console.WriteLine("Waiting for memory card to load..");

// Load the memory card.
var memory = new PSMetadataLib.PS2.MemoryCard(path);

Console.Clear();

Console.WriteLine("~~ PS2 MEMORY CARD ~~");
Console.WriteLine($"Version {memory.Version}");

Console.WriteLine();

Console.WriteLine("~~ INFO ~~");
Console.WriteLine($"Page length\t{memory.PageSize}");
Console.WriteLine($"Pages/cluster\t{memory.PagesPerCluster}");
Console.WriteLine($"Pages/block\t{memory.PagesPerBlock}");
Console.WriteLine($"No. clusters\t{memory.ClustersTotal}");
Console.WriteLine($"Cluster count\t{memory.Clusters.Count}");
Console.WriteLine($"Alloc start\t{memory.AllocStart}");
Console.WriteLine($"Alloc end\t{memory.AllocEnd}");
Console.WriteLine($"First cluster\t{memory.ClusterRootdir}");
Console.WriteLine($"Card Type\t{memory.CardFlags.ToDescriptions()}");

Console.WriteLine();

const uint entryNumber = 4;
var fatEntry = memory.GetFATEntry(entryNumber);

Console.WriteLine($"Entry {entryNumber}: {fatEntry.IsFree}, {fatEntry.NextCluster:b32} (ORIGINAL: {fatEntry.OriginalEntry:b32})");

Console.WriteLine($"Reading Cluster {memory.AllocStart + fatEntry.NextCluster}");

var cluster = memory.Clusters[(int)(memory.AllocStart + fatEntry.NextCluster)];

// Console.WriteLine($"\tPage count: {cluster.Pages.Count}");
// foreach (var block in cluster.Pages)
// {
//     for (var i = 0; i < block.Data.LongLength; i++)
//     {
//         if ((i % 32) == 0)
//         {
//             Console.Write($"\n0x{i:x8}\t");
//             for (var x = i; x < (i + 32); x++)
//             {
//                 if (x >= block.Data.LongLength)
//                 {
//                     Console.Write($" {"--",2}");
//                     continue;
//                 }
//
//                 if (block.Data[x] > 32 && block.Data[x] < 127)
//                     Console.BackgroundColor = ConsoleColor.DarkGreen;
//                 else if (block.Data[x] < 255)
//                     Console.BackgroundColor = ConsoleColor.DarkRed;
//                 Console.Write($" {block.Data[x].ToString("X"),2} ");
//                 Console.ResetColor();
//             }
//
//             Console.Write("\t");
//         }
//
//         if (block.Data[i] > 32 && block.Data[i] < 127)
//             Console.BackgroundColor = ConsoleColor.DarkGreen;
//         else if (block.Data[i] < 255)
//             Console.BackgroundColor = ConsoleColor.DarkRed;
//         Console.Write((block.Data[i] < 32 || block.Data[i] > 126)
//             ? "."
//             : Encoding.ASCII.GetString(new[] { block.Data[i] }));
//         Console.ResetColor();
//     }
//
//     Console.WriteLine($"\n\t\tECC: {Encoding.ASCII.GetString(block.ECC)}");
// }