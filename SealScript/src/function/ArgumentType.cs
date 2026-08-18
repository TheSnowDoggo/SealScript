using System;

namespace SealScript;

[Flags]
public enum ArgumentType
{
    None     = 0,
    Nil      = 1 << 0,
    Bool     = 1 << 1,
    Number   = 1 << 2,
    String   = 1 << 3,
    Function = 1 << 4,
    Object   = 1 << 5,
    Class    = 1 << 6,
    Any      = (1 << 7) - 1,
}