using System;

namespace SealScript.Expressions;

public class ArithmeticExpression : BinaryExpression
{
    public ArithmeticExpression(ArithmeticType arithmeticType, Expression left, Expression right)
        : base(left, right)
    {
        ArithmeticType = arithmeticType;
    }
    
    public ArithmeticType ArithmeticType { get; }
    
    public static SealValue Evaluate(ArithmeticType arithmeticType, CallContext context, SealValue a, SealValue b)
    {
        return arithmeticType switch
        {
            ArithmeticType.Multiply => EvaluateMultiply(context, a, b),
            ArithmeticType.Divide => EvaluateDivide(context, a, b),
            ArithmeticType.Modulo => EvaluateModulo(context, a, b),
            ArithmeticType.Add => EvaluateAdd(context, a, b),
            ArithmeticType.Subtract => EvaluateSubtract(context, a, b),
            ArithmeticType.And => EvaluateAnd(context, a, b),
            ArithmeticType.Xor => EvaluateXor(context, a, b),
            ArithmeticType.Or  => EvaluateOr(context, a, b),
            _ => throw new SealException(context,
                $"Unrecognised aritmetic type {arithmeticType}.")
        };
    }

    public static char ToSymbol(ArithmeticType arithmeticType) => arithmeticType switch
    {
        ArithmeticType.Multiply => '*',
        ArithmeticType.Divide   => '/',
        ArithmeticType.Modulo   => '%',
        ArithmeticType.Add      => '+',
        ArithmeticType.Subtract => '-',
        ArithmeticType.And      => '&',
        ArithmeticType.Xor      => '^',
        ArithmeticType.Or       => '|',
        _ => throw new ArgumentException($"Invalid arithmetic type {arithmeticType}.")
    };
    
    public override SealValue Evaluate(CallContext context)
    {
        return Evaluate(ArithmeticType, context, Left.Evaluate(context), Right.Evaluate(context));
    }
    
    public override string ToString()
    {
        return $"{ToSymbol(ArithmeticType)}({Left}, {Right})";
    }
    
    private static SealValue EvaluateMultiply(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a * (double)b;
        }
        
        throw new SealException(context,
            $"No multiply overload found for {a.ValueType} * {b.ValueType}");
    }
    
    private static SealValue EvaluateDivide(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a / (double)b;
        }
        
        throw new SealException(context,
            $"No divide overload found for {a.ValueType} / {b.ValueType}");
    }
    
    private static SealValue EvaluateModulo(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a % (double)b;
        }
        
        throw new SealException(context,
            $"No modulo overload found for {a.ValueType} % {b.ValueType}");
    }

    private static SealValue EvaluateAdd(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.String || b.ValueType == SealValueType.String)
        {
            return a.ToString() + b;
        }

        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a + (double)b;
        }

        throw new SealException(context,
            $"No add overload found for {a.ValueType} + {b.ValueType}");
    }
    
    private static SealValue EvaluateSubtract(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (double)a - (double)b;
        }
        
        throw new SealException(context,
            $"No subtract overload found for {a.ValueType} - {b.ValueType}");
    }
    
    private static SealValue EvaluateAnd(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (int)(double)a & (int)(double)b;
        }
        
        if (a.ValueType == SealValueType.Bool && b.ValueType == SealValueType.Bool)
        {
            return (bool)a & (bool)b;
        }
        
        throw new SealException(context,
            $"No and overload found for {a.ValueType} & {b.ValueType}");
    }
    
    private static SealValue EvaluateXor(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (int)(double)a ^ (int)(double)b;
        }
        
        if (a.ValueType == SealValueType.Bool && b.ValueType == SealValueType.Bool)
        {
            return (bool)a ^ (bool)b;
        }
        
        throw new SealException(context,
            $"No xor overload found for {a.ValueType} ^ {b.ValueType}");
    }
    
    private static SealValue EvaluateOr(CallContext context, SealValue a, SealValue b)
    {
        if (a.ValueType == SealValueType.Number && b.ValueType == SealValueType.Number)
        {
            return (int)(double)a | (int)(double)b;
        }
        
        if (a.ValueType == SealValueType.Bool && b.ValueType == SealValueType.Bool)
        {
            return (bool)a | (bool)b;
        }
        
        throw new SealException(context,
            $"No or overload found for {a.ValueType} | {b.ValueType}");
    }
}