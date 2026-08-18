using System;

namespace SealScript;

[Flags]
public enum StatementParsingFlag
{
    None = 0,
    NoTerminators = 1 << 0,
}