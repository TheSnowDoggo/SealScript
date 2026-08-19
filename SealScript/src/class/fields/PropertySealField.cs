using System;
using System.Reflection;

namespace SealScript;

public class PropertySealField : SealField
{
    private readonly Func<SealValue, SealValue> _getter;
    private readonly Action<SealValue, SealValue> _setter;
    private readonly ArgumentType _allowedTypes;
    
    public PropertySealField(Func<SealValue, SealValue> getter, 
        Action<SealValue, SealValue> setter,
        ArgumentType allowedTypes)
    {
        _getter = getter;
        _setter = setter;
        _allowedTypes = allowedTypes;
    }

    public override SealValue Get(CallContext context, string name, SealValue self)
    {
        if (_getter == null)
        {
            throw new SealException(context, $"Field {name} does not have a getter.");
        }

        return _getter(self);
    }

    public override void Set(CallContext context, string name, SealValue self, SealValue value)
    {
        if (_setter == null)
        {
            throw new SealException(context, $"Field {name} does not have a setter.");
        }

        if (!_allowedTypes.IsAssignableFrom(value.ValueType))
        {
            throw new SealException(context, $"Field {name} expected value of type(s) [{_allowedTypes}], got {value.ValueType}.");
        }
        
        _setter(self, value);
    }
}