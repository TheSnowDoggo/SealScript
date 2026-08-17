namespace SealScript.Expressions;

public class CompoundArithmeticExpression : Expression
{
    public CompoundArithmeticExpression(ArithmeticType arithmeticType, AssignableExpression left, Expression right)
    {
        ArithmeticType = arithmeticType;
        Left = left;
        Right = right;
    }
    
    public ArithmeticType ArithmeticType { get; }
    
    public AssignableExpression Left { get; }
    public Expression Right { get; }
    
    public override SealValue Evaluate(CallContext context)
    {
        SealValue value = ArithmeticExpression.Evaluate(ArithmeticType, context, 
            Left.Evaluate(context), Right.Evaluate(context));
        
        Left.Assign(context, value);
        
        return value;
    }
    
    public override string ToString()
    {
        return $"{Left} {ArithmeticType}= {Right}";
    }
}