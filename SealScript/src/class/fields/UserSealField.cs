namespace SealScript;

public class UserSealField : SealField
{
    private readonly int _location;
    private readonly ArgumentType _allowedTypes;
    
    public UserSealField(int location, ArgumentType allowedTypes)
    {
        _location = location;
        _allowedTypes = allowedTypes;
    }
    
    public override SealValue Get(CallContext context, string name, SealValue self)
    {
        return ((UserSealObject)self).GetField(_location);
    }

    public override void Set(CallContext context, string name, SealValue self, SealValue value)
    {
        var sealObject = (UserSealObject)self;

        if (!sealObject.Constructing && _allowedTypes.IsConst())
        {
            throw new SealException(context, $"Cannot set field {sealObject.Class.Name}.{name} as it is immutable.");
        }
        
        if (!_allowedTypes.IsAssignableFrom(value.ValueType))
        {
            throw new SealException(context, 
                $"Field {sealObject.Class.Name}.{name} expected value of type {_allowedTypes.ToArgumentString()}, got {value.ValueType}.");
        }
        
        sealObject.SetField(_location, value);
    }
}