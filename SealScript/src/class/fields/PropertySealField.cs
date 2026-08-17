using System;
using System.Reflection;

namespace SealScript;

public class PropertySealField : SealField
{
    private readonly Func<SealValue, SealValue> _getter;
    private readonly Action<SealValue, SealValue> _setter;
    private readonly ArgumentType _settableType;
    
    public PropertySealField(Func<SealValue, SealValue> getter, 
        Action<SealValue, SealValue> setter,
        ArgumentType settableType)
    {
        _getter = getter;
        _setter = setter;
        _settableType = settableType;
    }

    public override SealValue Get(CallContext context, SealValue self)
    {
        if (_getter == null)
        {
            throw new SealException(context, "Field does not have a getter.");
        }

        return _getter(self);
    }

    public override void Set(CallContext context, SealValue self, SealValue value)
    {
        if (_setter == null)
        {
            throw new SealException(context, "Field does not have a setter.");
        }

        if (!value.IsTypeAllowed(_settableType))
        {
            throw new SealException(context, $"Field expected value of type(s) [{_settableType}], got {value.ValueType}.");
        }
        
        _setter(self, value);
    }
}