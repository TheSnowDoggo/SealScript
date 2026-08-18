using System;
using System.Text;

namespace SealScript;

public abstract class Function
{
    public const int AnyArgs = -1;
    
    public abstract string Name { get; }
    
    public int MinArgs { get; init; }
    public int MaxArgs { get; init; } = AnyArgs;
    
    public ArgumentType[] ArgumentTypes { get; init; }
    
    internal abstract SealValue _Invoke(SealObject self, CallArgs args);

    public SealValue Invoke(SealObject self, CallContext context, params ReadOnlySpan<SealValue> args)
    {
        ValidateArgs(context, args);
        return _Invoke(self, new CallArgs(context, args));
    }
    
    public SealValue Invoke(SealObject self, params ReadOnlySpan<SealValue> args)
    {
        ValidateArgs(null, args);
        return _Invoke(self, new CallArgs(null, args));
    }
    
    public SealValue Invoke(CallContext context, params ReadOnlySpan<SealValue> args)
    {
        ValidateArgs(context, args);
        return _Invoke(null, new CallArgs(context, args));
    }
    
    public SealValue Invoke(params ReadOnlySpan<SealValue> args)
    {
        ValidateArgs(null, args);
        return _Invoke(null, new CallArgs(null, args));
    }

    private void ValidateArgs(CallContext context, ReadOnlySpan<SealValue> args)
    {
        if (args.Length < MinArgs)
        {
            throw new SealException(context,
                $"{this} expected minimum of {MinArgs} args, got {args.Length}.");
        }

        if (MaxArgs >= 0 && args.Length > MaxArgs)
        {
            throw new SealException(context,
                $"{this} expected maximum of {MaxArgs} args, got {args.Length}.");
        }

        if (ArgumentTypes != null)
        {
            int length = Math.Min(ArgumentTypes.Length, args.Length);
            
            for (int i = 0; i < length; i++)
            {
                ArgumentType expected = ArgumentTypes[i];

                // Note im using IsTypeIncluded not IsAssignableTo
                // This disallows Nil assignment, e.g. Function can be assigned Nil
                // or Number | String can be assigned Nil
                if (!expected.IsTypeIncluded(args[i].ValueType))
                {
                    throw new SealException(context,
                        $"{this} expected argument {i} to be of type {expected}, got {args[i].ValueType}.");
                }
            }
        }
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

        for (int i = 0; i < ArgumentTypes.Length; i++)
        {
            if (i != 0)
            {
                sb.Append(", ");
            }
            
            sb.Append(ArgumentTypes[i].ToArgumentString());
        }
        
        sb.Append(')');
        
        return sb.ToString();
    }
}