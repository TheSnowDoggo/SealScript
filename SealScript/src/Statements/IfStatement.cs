using System.Text;
using SealScript.Expressions;

namespace SealScript.Statements;

public class IfStatement : BlockStatement
{
    public Expression Expression { get; init; }
    public BlockStatement ElseBlock { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        if (Expression.Evaluate(context).InterpretAsBool())
        {
            return base._Run(context);
        }

        if (ElseBlock != null)
        {
            return ElseBlock.Run(context);
        }
        
        return ReturnValue.None;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"if {Expression} ");
        
        AppendStatements(sb);

        if (ElseBlock != null)
        {
            sb.Append($" {ElseBlock}");
        }
        
        return sb.ToString();
    }
}