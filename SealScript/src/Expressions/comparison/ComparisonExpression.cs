namespace SealScript.Expressions;

public class ComparisonExpression : BinaryExpression
{
    public ComparisonExpression(ComparisonType comparisonType, Expression left, Expression right)
        : base(left, right)
    {
        ComparisonType = comparisonType;
    }
    
    public ComparisonType ComparisonType { get; }
    
    public override SealValue Evaluate(CallContext context)
    {
        return Compare(context, Left.Evaluate(context), Right.Evaluate(context));
    }

    public override string ToString()
    {
        return $"{Left} {ComparisonType} {Right}";
    }

    private bool Compare(CallContext context, SealValue a, SealValue b)
    {
        return ComparisonType switch
        {
            ComparisonType.LessThan => CompareLessThan(context, a, b),
            ComparisonType.GreaterThan => CompareLessThan(context, b, a),
            ComparisonType.LessThanOrEqual => !CompareLessThan(context, b, a),
            ComparisonType.GreaterThanOrEqual => !CompareLessThan(context, a, b),
            ComparisonType.Equals => a.Equals(b),
            ComparisonType.NotEquals => !a.Equals(b),
            _ => throw new SealException(context,
                $"Unrecognised comparison type {ComparisonType}.")
        };
    }
    
    private bool CompareLessThan(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a < (double)b;
        }
        
        if (a.ValueType == SealValueType.String && b.ValueType == SealValueType.String)
        {
            return string.CompareOrdinal((string)a, (string)b) < 0;
        }
        
        throw new SealException(context,
            $"No comparison overload found for {a.ValueType} {ComparisonType} {b.ValueType}.");
    }
}