using System;
using SealScript.Collections;

namespace SealScript.Expressions;

public class InvokeExpression : Expression
{
    public InvokeExpression(Expression functionExpression, Expression[] argumentExpressions)
    {
        FunctionExpression = functionExpression;
        ArgumentExpressions = argumentExpressions;
    }
    
    public Expression FunctionExpression { get; }
    public Expression[] ArgumentExpressions { get; }

    public override SealValue Evaluate(CallContext context)
    {
        SealObject self = null;
        SealValue functionValue;
        
        if (FunctionExpression is MemberFieldExpression memberFieldExpression)
        {
            functionValue = memberFieldExpression.GetField(context, out SealValue value).Get(context, value);

            if (value.ValueType == SealValueType.Object)
            {
                self = (SealObject)value;
            }
        }
        else
        {
            functionValue = FunctionExpression.Evaluate(context);
        }

        if (functionValue.ValueType != SealValueType.Function)
        {
            throw new SealException(context,
                $"Cannot invoke non-invokable type {functionValue.ValueType}.");
        }

        var function = (Function)functionValue;

        using var args = new PooledBuffer<SealValue>(ArgumentExpressions.Length);

        for (int i = 0; i < ArgumentExpressions.Length; i++)
        {
            args[i] = ArgumentExpressions[i].Evaluate(context);
        }

        return function.Invoke(self, context, args);
    }

    public override string ToString()
    {
        return $"{FunctionExpression}({string.Join<Expression>(", ", ArgumentExpressions)})";
    }
}