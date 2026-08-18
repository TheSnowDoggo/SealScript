using System.IO;
using System.Text;
using SealScript.Expressions;
using SealScript.Statements;

namespace SealScript;

public class UserFunction : Function
{
    public UserFunction(FunctionDefinition definition, CallContext parentContext = null)
    {
        Definition = definition;
        ParentContext = parentContext;
    }

    public override string Name => Definition.Name;

    public FunctionDefinition Definition { get; }
    public CallContext ParentContext { get; }
    
    public static UserFunction CreateFromScript(string filePath)
    {
        Token[] tokens;
        
        using (FileStream fs = File.OpenRead(filePath))
        {
            tokens = new Tokenizer(fs).Tokenize();
        }
        
        var stream = new TokenStream(tokens);

        var parser = new StatementParser(stream);

        FunctionDefinition definition = parser.Parse();

        var sealProgram = new SealProgram()
        {
            ExecutingFilePath = Path.GetFullPath(filePath),
        };

        var parentContext = new CallContext(sealProgram, null);
        
        return definition.CreateFunction(parentContext);
    }
    
    internal override SealValue _Invoke(SealObject self, CallArgs args)
    {
        var localContext = new CallContext(ParentContext?.SealProgram, ParentContext);
        
        localContext.OpenScope();

        DefineArguments(localContext, self, args);
        
        Statement[] statements = Definition.Statements;
            
        for (int i = 0; i < statements.Length; i++)
        {
            Statement statement = statements[i];
                
            ReturnValue returnValue = statement.Run(localContext);

            switch (returnValue.Type)
            {
            case ReturnValueType.Return:
                return returnValue.Value;
            case ReturnValueType.Continue:
                throw new SealException(statement, 
                    "Cannot continue out of function.");
            case ReturnValueType.Break:
                throw new SealException(statement, 
                    "Cannot break out of function.");
            }
        }

        return SealValue.Nil;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Name);
        
        sb.Append('(');

        string[] arguments = Definition.Arguments;
        Expression[] defaultArguments = Definition.DefaultArguments;
        
        int defaultArgumentStart = arguments.Length - defaultArguments.Length;
        
        for (int i = 0; i < Definition.Arguments.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(", ");
            }
            
            sb.Append(Definition.Arguments[i]);
            sb.Append(": ");
            sb.Append(Definition.ArgumentTypes[i].ToArgumentString());
            
            int defaultArgumentIndex = i - defaultArgumentStart;

            if (defaultArgumentIndex >= 0)
            {
                sb.Append(" = ");
                sb.Append(defaultArguments[defaultArgumentIndex]);
            }
        }

        sb.Append(')');
        
        return sb.ToString();
    }

    private void DefineArguments(CallContext localContext, SealObject self, CallArgs args)
    {
        if (self != null)
        {
            localContext.DefineVariable("self", self, ArgumentType.None);
        }
        
        string[] arguments = Definition.Arguments;

        if (args.Length > arguments.Length)
        {
            throw new SealException(args.Context,
                $"Function expected maximum of {arguments.Length} arguments, got {args.Length}.");
        }

        for (int i = 0; i < args.Length; i++)
        {
            localContext.DefineVariable(arguments[i], args[i]);
        }

        if (args.Length == arguments.Length)
        {
            return;
        }
        
        Expression[] defaultArguments = Definition.DefaultArguments;

        for (int i = args.Length; i < arguments.Length; i++)
        {
            Expression defaultArgument = defaultArguments[i - args.Length];
            
            localContext.DefineVariable(arguments[i], defaultArgument.Evaluate(localContext));
        }
    }
}