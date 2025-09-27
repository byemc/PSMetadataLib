namespace PSMetadataLib.PS2.Enums;

[Flags]
public enum MemoryCardFlagsEnum : byte
{
    EECSupport = 0x01,
    BadBlockSupport = 0x08,
    ErasedState = 0x10
}