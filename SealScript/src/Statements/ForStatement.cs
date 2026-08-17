using SealScript.Expressions;

namespace SealScript.Statements;

public class ForStatement : BlockStatement
{
    public string Identifier { get; init; }
    public Expression Expression { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        foreach (SealValue value in GetEnumerable(context))
        {
            context.OpenScope();
            
            try
            {
                context.DefineVariable(Identifier, value);
                
                for (int i = 0; i < Statements.Length; i++)
                {
                    ReturnValue returnValue = Statements[i].Run(context);

                    switch (returnValue.Type)
                    {
                    case ReturnValueType.Return:
                        return returnValue;
                    case ReturnValueType.Continue:
                        continue;
                    case ReturnValueType.Break:
                        return ReturnValue.None;
                    }
                }
            }
            finally
            {
                context.CloseScope();
            } 
        }

        return ReturnValue.None;
    }

    private IEnumerable<SealValue> GetEnumerable(CallContext context)
    {
        SealValue value = Expression.Evaluate(context);

        switch (value.ValueType)
        {
        case SealValueType.String:
            return GetStringEnumerable(value.AsString());
        case SealValueType.Object:
            SealObject sealObject = value.AsSealObject();
            
            if (sealObject is not IEnumerable<SealValue> enumerable)
            {
                throw new SealException(context,
                    $"Object of class {sealObject.Class.Name} is not enumerable.");
            }

            return enumerable;
        default:
            throw new SealException(context,
                $"Value of type {value.ValueType} is not enumerable.");
        }
    }

    private static IEnumerable<SealValue> GetStringEnumerable(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            yield return s[i].ToString();
        }
    }
}