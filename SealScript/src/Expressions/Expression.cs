namespace SealScript.Expressions;

public abstract class Expression
{
    public abstract SealValue Evaluate(CallContext context);
}