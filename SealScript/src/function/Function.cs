using System;

namespace SealScript;

public abstract class Function
{
    public abstract string Name { get; }
    
    public abstract SealValue Invoke(SealObject self, CallArgs args);

    public SealValue Invoke(SealObject self, CallContext context, params ReadOnlySpan<SealValue> args)
    {
        return Invoke(self, new CallArgs(context, args));
    }
    
    public SealValue Invoke(SealObject self, params ReadOnlySpan<SealValue> args)
    {
        return Invoke(self, new CallArgs(null, args));
    }
    
    public SealValue Invoke(CallContext context, params ReadOnlySpan<SealValue> args)
    {
        return Invoke(null, new CallArgs(context, args));
    }
    
    public SealValue Invoke(params ReadOnlySpan<SealValue> args)
    {
        return Invoke(null, new CallArgs(null, args));
    }
}