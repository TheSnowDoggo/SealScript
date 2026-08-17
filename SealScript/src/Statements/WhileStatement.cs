using System.Text;
using SealScript.Expressions;

namespace SealScript.Statements;

public class WhileStatement : BlockStatement
{
    public Expression Condition { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        while (Condition.Evaluate(context).InterpretAsBool())
        {
            context.OpenScope();
                
            try
            {
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"while {Condition} ");

        AppendStatements(sb);
        
        return sb.ToString();
    }
}