using System;

namespace SealScript;

public class ValueSealField : SealField
{
    private readonly SealValue _value;
    
    public ValueSealField(SealValue value)
    {
        _value = value;
    }

    public override SealValue Get(CallContext context, SealValue self)
    {
        return _value;
    }

    public override void Set(CallContext context, SealValue self, SealValue value)
    {
        throw new SealException(context, "Cannot set readonly field.");
    }
}