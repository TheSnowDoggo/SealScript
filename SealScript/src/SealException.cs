using System;

namespace SealScript;

public class SealException : Exception
{
    public SealException(int line, int column, string message)
        : base($"Error at {line}:{column} {message}")
    {
    }
    
    public SealException(ILineNumbered lineNumbered, string message)
        : this(lineNumbered?.Line ?? -1, lineNumbered?.Column ?? -1, message)
    {
    }
}