namespace SealScript.Expressions;

public class ClassDefinitionExpression : Expression
{
    public ClassDefinitionExpression(ClassDefinition definition)
    {
        Definition = definition;
    }
    
    public ClassDefinition Definition { get; }

    public override SealValue Evaluate(CallContext context)
    {
        return Definition.CreateClass(context);
    }

    public override string ToString()
    {
        return $"CreateClass<{Definition.Name}>";
    }
}