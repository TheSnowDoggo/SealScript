using SealScript.Expressions;

namespace SealScript.Statements;

public class ExpressionStatement : Statement
{
    public Expression Expression { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        Expression.Evaluate(context);
        
        return ReturnValue.None;
    }

    public override string ToString()
    {
        return Expression.ToString();
    }
}