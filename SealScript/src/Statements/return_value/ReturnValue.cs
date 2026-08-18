namespace SealScript;

public struct ReturnValue
{
    public ReturnValue(ReturnValueType returnValueType, SealValue value = default)
    {
        Type = returnValueType;
        Value = value;
    }
    
    public static readonly ReturnValue None = new ReturnValue(ReturnValueType.None);
    public static readonly ReturnValue Continue = new ReturnValue(ReturnValueType.Continue);
    public static readonly ReturnValue Break = new ReturnValue(ReturnValueType.Break);

    public ReturnValueType Type { get; }
    public SealValue Value { get; }
}