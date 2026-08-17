using System;
using System.Text;

namespace SealScript;

public class NativeFunction : Function
{
    public const int AnyArgs = -1;
    
    public NativeFunction(Func<SealObject, CallArgs, SealValue> invoker)
    {
        Invoker = invoker;
    }
    
    public override string Name => CustomName ?? Invoker.Method.Name;
    
    public Func<SealObject, CallArgs, SealValue> Invoker { get; }
    
    public string CustomName { get; init; }
    
    public int MinArgs { get; init; }
    public int MaxArgs { get; init; } = AnyArgs;
    
    public ArgumentType[] ArgumentTypes { get; init; }
    
    public Type ExpectedObjectType { get; init; }
    
    public override SealValue Invoke(SealObject self, CallArgs args)
    {
        if (args.Length < MinArgs)
        {
            throw new SealException(args.Context,
                $"{this} expected minimum of {MinArgs} args, got {args.Length}.");
        }

        if (MaxArgs >= 0 && args.Length > MaxArgs)
        {
            throw new SealException(args.Context,
                $"{this} expected maximum of {MaxArgs} args, got {args.Length}.");
        }

        if (ArgumentTypes != null)
        {
            int length = Math.Min(ArgumentTypes.Length, args.Length);
            
            for (int i = 0; i < length; i++)
            {
                ArgumentType expected = ArgumentTypes[i];

                if (!args[i].IsTypeAllowed(expected))
                {
                    throw new SealException(args.Context,
                        $"{this} expected argument {i} to be of type {expected}, got {args[i].ValueType}.");
                }
            }
        }

        if (ExpectedObjectType != null)
        {
            if (self == null)
            {
                throw new SealException(args.Context,
                    $"{this} must be called as a instance function.");
            }

            Type selfType = self.GetType();
            
            if (selfType != ExpectedObjectType)
            {
                throw new SealException(args.Context,
                    $"{this} expected instance object to be of type {ExpectedObjectType.Name}, got {selfType}.");
            }
        }
        
        return Invoker(self, args);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Name);
        sb.Append('<');
        
        sb.Append(MinArgs);

        if (MaxArgs < 0)
        {
            sb.Append("..");
        }
        else if (MaxArgs != MinArgs)
        {
            sb.Append('-');
            sb.Append(MaxArgs);
        }

        sb.Append('>');

        sb.Append('(');

        if (ArgumentTypes != null)
        {
            sb.Append(string.Join(", ", ArgumentTypes));
        }

        sb.Append(')');
        
        return sb.ToString();
    }
}