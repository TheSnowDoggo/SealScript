namespace SealScript;

public class StaticUserSealField : SealField
{
    private readonly int _location;
    
    public StaticUserSealField(int location)
    {
        _location = location;
    }
    
    public override SealValue Get(CallContext context, SealValue self)
    {
        return ((UserSealClass)self).GetStaticField(_location);
    }

    public override void Set(CallContext context, SealValue self, SealValue value)
    {
        ((UserSealClass)self).SetStaticField(_location, value);
    }
}