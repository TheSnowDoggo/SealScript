using System.Text;
using SealScript.Expressions;

namespace SealScript;

public class FunctionDefinition
{
    public string Name { get; init; }
    public string[] Arguments { get; init; } = [];
    public Expression[] DefaultArguments { get; init; } = [];
    public Statement[] Statements { get; init; }
    
    public int MinArgs { get; init; }
    public int MaxArgs { get; init; } = Function.AnyArgs;
    
    public ArgumentType[] ArgumentTypes { get; init; }
    
    public UserFunction CreateFunction(CallContext parentContext = null)
    {
        return new UserFunction(this, parentContext)
        {
            MinArgs = MinArgs,
            MaxArgs = MaxArgs,
            ArgumentTypes = ArgumentTypes,
        };
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