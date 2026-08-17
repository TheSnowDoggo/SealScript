namespace SealScript.Expressions;

public class AssignExpression : Expression
{
    public AssignExpression(AssignableExpression left, Expression right)
    {
        Left = left;
        Right = right;
    }
    
    public AssignableExpression Left { get; init; }
    public Expression Right { get; init; }

    public override SealValue Evaluate(CallContext context)
    {
        SealValue value = Right.Evaluate(context);

        Left.Assign(context, value);

        return value;
    }

    public override string ToString()
    {
        return $"{Left} = {Right}";
    }
}