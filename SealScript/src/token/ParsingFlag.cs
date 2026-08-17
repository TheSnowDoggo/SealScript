using System;

namespace SealScript;

[Flags]
public enum ParsingFlag
{
    None = 0,
    NoTerminators = 1 << 0,
}