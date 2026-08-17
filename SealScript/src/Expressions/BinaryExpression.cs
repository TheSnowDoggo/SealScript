namespace SealScript.Expressions;

public abstract class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, Expression right)
    {
        Left = left;
        Right = right;
    }
    
    public Expression Left { get; }
    public Expression Right { get; }

    public override string ToString()
    {
        return $"{GetType().Name}({Left}, {Right})";
    }
}