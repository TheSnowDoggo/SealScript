namespace SealScript.Expressions;

public abstract class AssignableExpression : Expression
{
    public abstract void Assign(CallContext context, SealValue value);
}