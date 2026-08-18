namespace SealScript.Statements;

public class DefineStatement : ExpressionStatement
{
    public string Name { get; init; }
    public ArgumentType AllowedTypes { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        context.DefineVariable(Name, Expression.Evaluate(context), AllowedTypes);

        return ReturnValue.None;
    }

    public override string ToString()
    {
        return Expression == null ? $"var {Name};" : $"var {Name} = {Expression};";
    }
}