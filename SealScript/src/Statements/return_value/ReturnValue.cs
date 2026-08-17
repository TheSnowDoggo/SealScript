namespace SealScript;

public struct ReturnValue
{
    public ReturnValue(ReturnValueType returnValueType, SealValue value = default)
    {
        Type = returnValueType;
        Value = value;
    }
    
    public static readonly ReturnValue None = new ReturnValue(ReturnValueType.None);

    public ReturnValueType Type { get; }
    public SealValue Value { get; }
}