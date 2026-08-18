namespace SealScript;

public class Token : ILineNumbered
{
    public Token(int line, int column, TokenType tokenType, SealValue value = default)
    {
        Line = line;
        Column = column;
        TokenType = tokenType;
        Value = value;
    }
    
    public int Line { get; }
    public int Column { get; }
    public TokenType TokenType { get; set; }
    public SealValue Value { get; }

    public override string ToString()
    {
        return $"Token([{Line}:{Column}] {TokenType} {Value})";
    }
}