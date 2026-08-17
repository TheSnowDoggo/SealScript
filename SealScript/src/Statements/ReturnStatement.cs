using SealScript.Expressions;

namespace SealScript.Statements;

public class ReturnStatement : Statement
{
    public Expression Expression { get; init; }

    protected override ReturnValue _Run(CallContext context)
    {
        return new ReturnValue(ReturnValueType.Return, Expression.Evaluate(context));
    }

    public override string ToString()
    {
        return $"return {Expression};";
    }
}