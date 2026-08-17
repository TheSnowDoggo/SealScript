namespace SealScript.Expressions;

public class IndexerExpression : AssignableExpression
{
    public IndexerExpression(Expression instanceExpression, Expression indexExpression)
    {
        InstanceExpression = instanceExpression;
        IndexExpression = indexExpression;
    }
    
    public Expression InstanceExpression { get; }
    public Expression IndexExpression { get; }

    public override SealValue Evaluate(CallContext context)
    {
        SealValue source = InstanceExpression.Evaluate(context);
        SealValue index = IndexExpression.Evaluate(context);

        switch (source.ValueType)
        {
        case SealValueType.String:
            return StringIndexGetter(context, (string)source, index);
        case SealValueType.Object:
            return ((SealObject)source)[context, index];
        default:
            throw new SealException(context,
                $"Value of type {source.ValueType} cannot be get indexed.");
        }
    }

    public override void Assign(CallContext context, SealValue value)
    {
        SealValue source = InstanceExpression.Evaluate(context);
        SealValue index = IndexExpression.Evaluate(context);

        switch (source.ValueType)
        {
        case SealValueType.Object:
            ((SealObject)source)[context, index] = value;
            break;
        default:
            throw new SealException(context,
                $"Value of type {source.ValueType} cannot be set indexed.");
        }
    }

    private static SealValue StringIndexGetter(CallContext context, string s, SealValue index)
    {
        if (index.ValueType != SealValueType.Number)
        {
            throw new SealException(context,
                $"Expected number while indexing string, got {index.ValueType}.");
        }

        int indexValue = (int)index;

        if (indexValue < 0 || indexValue >= s.Length)
        {
            throw new SealException(context,
                $"String index {indexValue} was out of range.");
        }

        return s[indexValue].ToString();
    }
}