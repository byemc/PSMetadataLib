using System.ComponentModel;

namespace PSMetadataLib.PS2.Enums;

[Flags]
public enum MemoryCardDirectoryMode : ushort
{
    [Description("Read")]
    DfRead      = 0x0001,
    [Description("Write")]
    DfWrite     = 0x0002,
    [Description("Execute")]
    DfExecute   = 0x0004,
    [Description("Protected")]
    DfProtected = 0x0008,
    [Description("File")]
    DfFile      = 0x0010,
    [Description("Directory")]
    DfDirectory = 0x0020,
    [Description("PocketStation save file")]
    DfPocketStn = 0x0800,
    [Description("PlayStation save file")]
    DfPSX       = 0x1000,
    [Description("Hidden")]
    DfHidden    = 0x2000,
    [Description("Exists (not deleted)")]
    DfExists    = 0x8000,
}