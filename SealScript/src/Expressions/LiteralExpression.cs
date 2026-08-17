namespace SealScript.Expressions;

public class LiteralExpression : Expression
{
    public LiteralExpression(SealValue value)
    {
        Value = value;
    }
    
    public static readonly LiteralExpression Nil = new LiteralExpression(SealValue.Nil);
    
    public SealValue Value { get; }

    public override SealValue Evaluate(CallContext context)
    {
        return Value;
    }

    public override string ToString()
    {
        return Value.ValueType == SealValueType.String ? $"\"{Value}\"" : Value.ToString();
    }
}