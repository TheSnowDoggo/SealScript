namespace SealScript.Expressions;

public class ShortcutAndExpression : BinaryExpression
{
    public ShortcutAndExpression(Expression left, Expression right)
        : base(left, right)
    {
    }
    
    public override SealValue Evaluate(CallContext context)
    {
        SealValue a = Left.Evaluate(context);

        if (a.ValueType != SealValueType.Bool)
        {
            throw new SealException(context,
                $"Expected left-hand side of && operator to be Bool, got {a.ValueType}.");
        }

        // Shortcut to false if left-hand is false
        if (!(bool)a)
        {
            return false;
        }
        
        SealValue b = Right.Evaluate(context);
        
        if (b.ValueType != SealValueType.Bool)
        {
            throw new SealException(context,
                $"Expected right-hand side of && operator to be Bool, got {a.ValueType}.");
        }

        return (bool)b;
    }
    
    public override string ToString()
    {
        return $"{Left} && {Right}";
    }
}