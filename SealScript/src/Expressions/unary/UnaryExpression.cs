using System;

namespace SealScript.Expressions;

public class UnaryExpression : Expression
{
    public UnaryExpression(UnaryType unaryType, Expression operand)
    {
        UnaryType = unaryType;
        Operand = operand;
    }
    
    public UnaryType UnaryType { get; }
    public Expression Operand { get; set; }

    public static char ToSymbol(UnaryType unaryType) => unaryType switch
    {
        UnaryType.Minus => '-',
        UnaryType.Not => '!',
        _ => throw new ArgumentException($"Invalid unary type {unaryType}.")
    };

    public override SealValue Evaluate(CallContext context)
    {
        return Evaluate(context, Operand.Evaluate(context));
    }

    public override string ToString()
    {
        return $"{ToSymbol(UnaryType)}{Operand}";
    }

    private SealValue Evaluate(CallContext context, SealValue a)
    {
        return UnaryType switch
        {
            UnaryType.Minus => EvaluateMinus(context, a),
            UnaryType.Not => EvaluateNot(context, a),
            _ => throw new SealException(context,
                $"Unrecognosed unary type {UnaryType}.")
        };
    }

    private static SealValue EvaluateMinus(CallContext context, SealValue a)
    {
        if (a.ValueType == SealValueType.Number)
        {
            return -(double)a;
        }
        
        throw new SealException(context,
            $"No unary minus overload found for -{a.ValueType}");
    }
    
    private static SealValue EvaluateNot(CallContext context, SealValue a)
    {
        if (a.ValueType == SealValueType.Bool)
        {
            return !(bool)a;
        }
        
        throw new SealException(context,
            $"No not overload found for !{a.ValueType}");
    }
}