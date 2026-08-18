namespace SealScript.Expressions;

public class LiteralExpression : Expression
{
    public LiteralExpression(SealValue value)
    {
        Value = value;
    }
    
    public static readonly LiteralExpression Nil = new LiteralExpression(SealValue.Nil);
    public static readonly LiteralExpression False = new LiteralExpression(false);
    public static readonly LiteralExpression Zero = new LiteralExpression(0);
    public static readonly LiteralExpression EmptyString = new LiteralExpression(string.Empty);
    
    public SealValue Value { get; }

    public static LiteralExpression GetDefault(ArgumentType type) => type switch
    {
        ArgumentType.Bool => False,
        ArgumentType.Number => Zero,
        ArgumentType.String => EmptyString,
        _ => Nil,
    };
    
    public override SealValue Evaluate(CallContext context)
    {
        return Value;
    }

    public override string ToString()
    {
        return Value.ToString(false);
    }
}