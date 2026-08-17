namespace SealScript.Expressions;

public class VariableExpression : AssignableExpression
{
    public VariableExpression(string identifier)
    {
        Identifier = identifier;
    }
    
    public string Identifier { get; }

    public override SealValue Evaluate(CallContext context)
    {
        return context.GetValue(Identifier);
    }

    public override void Assign(CallContext context, SealValue value)
    {
        context.SetValue(Identifier, value);
    }

    public override string ToString()
    {
        return Identifier;
    }
}