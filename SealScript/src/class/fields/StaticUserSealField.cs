namespace SealScript;

public class StaticUserSealField : SealField
{
    private readonly int _location;
    private readonly ArgumentType _allowedTypes;
    
    public StaticUserSealField(int location, ArgumentType allowedTypes)
    {
        _location = location;
        _allowedTypes = allowedTypes;
    }
    
    public override SealValue Get(CallContext context, string name, SealValue self)
    {
        return ((UserSealClass)self).GetStaticField(_location);
    }

    public override void Set(CallContext context, string name, SealValue self, SealValue value)
    {
        if (_allowedTypes == ArgumentType.None)
        {
            throw new SealException(context, $"Cannot set static field {name} as it is immutable.");
        }

        if (!_allowedTypes.IsAssignableFrom(value.ValueType))
        {
            throw new SealException(context, 
                $"Static field expected value of type {_allowedTypes.ToArgumentString()}, got {value.ValueType}.");
        }
        
        ((UserSealClass)self).SetStaticField(_location, value);
    }
}