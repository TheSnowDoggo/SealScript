namespace SealScript;

public class UserSealField : SealField
{
    private readonly int _location;
    
    public UserSealField(int location)
    {
        _location = location;
    }
    
    public override SealValue Get(CallContext context, SealValue self)
    {
        return ((UserSealObject)self).GetField(_location);
    }

    public override void Set(CallContext context, SealValue self, SealValue value)
    {
        ((UserSealObject)self).SetField(_location, value);
    }
}