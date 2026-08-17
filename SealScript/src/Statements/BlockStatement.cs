using System.Text;

namespace SealScript.Statements;

public class BlockStatement : Statement
{
    public Statement[] Statements { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        context.OpenScope();

        try
        {
            for (int i = 0; i < Statements.Length; i++)
            {
                Statement statement = Statements[i];
                
                ReturnValue returnValue = statement.Run(context);

                switch (returnValue.Type)
                {
                case ReturnValueType.Return:
                    return returnValue;
                case ReturnValueType.Continue:
                    throw new SealException(statement, 
                        "Cannot continue out of block.");
                case ReturnValueType.Break:
                    throw new SealException(statement, 
                        "Cannot break out of block.");
                }
            }
        }
        finally
        {
            context.CloseScope();
        }
        
        return ReturnValue.None;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        AppendStatements(sb);
        
        return sb.ToString();
    }

    protected void AppendStatements(StringBuilder sb)
    {
        sb.AppendLine("{");

        foreach (Statement statement in Statements)
        {
            sb.Append("  ");
            sb.AppendLine(statement.ToString());
        }

        sb.Append('}');
    }
}