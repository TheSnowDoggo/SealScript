namespace SealScript;

public class UserSealClass : SealClass
{
    private readonly SealValue[] _staticFields;

    public UserSealClass(SealValue[] staticFields)
    {
        _staticFields = staticFields;
    }
    
    public NativeFunction Constructor { get; set; }
    
    public SealValue GetStaticField(int location)
    {
        return location == -1 ? Constructor : _staticFields[location];
    }

    public void SetStaticField(int location, SealValue value)
    {
        _staticFields[location] = value;
    }
}