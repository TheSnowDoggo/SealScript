using System.Text;

namespace SealScript;

public class FunctionDefinition
{
    public string Name { get; init; }
    public string[] Arguments { get; init; } = [];
    public Statement[] Statements { get; init; }
    
    public UserFunction CreateFunction(CallContext parentContext = null)
    {
        return new UserFunction(this, parentContext);
    }

    public string GetHeader()
    {
        return $"{Name}({string.Join<string>(", ", Arguments)})";
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"{GetHeader()} {{");

        for (int i = 0; i < Statements.Length; i++)
        {
            sb.Append("  ");
            sb.AppendLine(Statements[i].ToString());
        }
        
        sb.Append('}');
        
        return sb.ToString();
    }
}