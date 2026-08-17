namespace SealScript.Expressions;

public class FunctionDefinitionExpression : Expression
{
    public FunctionDefinitionExpression(FunctionDefinition definition)
    {
        Definition = definition;
    }
    
    public FunctionDefinition Definition { get; }
    
    public override SealValue Evaluate(CallContext context)
    {
        return Definition.CreateFunction(context);
    }

    public override string ToString()
    {
        return Definition.ToString();
    }
}