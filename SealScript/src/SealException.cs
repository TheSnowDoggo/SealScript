using System;

namespace SealScript;

public class SealException : Exception
{
    public SealException(int line, int column, string message)
        : base($"Error at {line}:{column} {message}")
    {
        Line = line;
        Column = column;
    }

    public SealException(CallContext context, string message)
        : this(context.Line, context.Column, message)
    {
    }
    
    public SealException(Token token, string message)
        : this(token.Line, token.Column, message)
    {
    }
    
    public SealException(TokenStream stream, string message)
        : this(stream.Line, stream.Column, message)
    {
    }
    
    public SealException(Statement statement, string message)
        : this(statement.Line, statement.Column, message)
    {
    }
    
    public int Line { get; }
    public int Column { get; }
}