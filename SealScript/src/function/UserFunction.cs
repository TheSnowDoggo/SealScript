using System.IO;
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
    
    public override SealValue Invoke(SealObject self, CallArgs args)
    {
        var localContext = new CallContext(ParentContext?.SealProgram, ParentContext);
        
        localContext.OpenScope();

        if (self != null)
        {
            localContext.DefineVariable("self", self);
        }

        string[] arguments = Definition.Arguments;

        if (args.Length > arguments.Length)
        {
            throw new SealException(args.Context,
                $"Function expected maximum of {arguments.Length} arguments, got {args.Length}.");
        }

        for (int i = 0; i < arguments.Length; i++)
        {
            localContext.DefineVariable(arguments[i], i < args.Length ? args[i] : SealValue.Nil);
        }
        
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
        return Definition.GetHeader();
    }
}