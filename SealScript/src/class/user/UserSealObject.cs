using System.Text;

namespace SealScript;

public class UserSealObject : SealObject
{
    private readonly SealValue[] _fields;
    
    public UserSealObject(SealClass sealClass)
    {
        Class = sealClass;
        _fields = new SealValue[sealClass.Fields.Count];
    }
    
    public override SealClass Class { get; }
    
    public bool Constructing { get; set; }

    public SealValue GetField(int location)
    {
        return _fields[location];
    }

    public void SetField(int location, SealValue value)
    {
        _fields[location] = value;
    }

    public override string ToString()
    {
        if (Class.Fields.TryGetValue("toString", out SealField field))
        {
            SealValue value = field.Get(null, "toString", this);

            if (value.ValueType == SealValueType.Function)
            {
                return value.AsFunction().Invoke(this).ToString();
            }
        }

        return base.ToString();
    }
}