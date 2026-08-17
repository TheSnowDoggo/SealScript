using System;

namespace SealScript;

public readonly ref struct CallArgs
{
    public CallArgs(CallContext context, ReadOnlySpan<SealValue> values)
    {
        Context = context;
        Values = values;
    }
    
    public CallContext Context { get; }
    public ReadOnlySpan<SealValue> Values { get; }
    
    public int Length => Values.Length;
    
    public SealValue this[int index] => Values[index];
}