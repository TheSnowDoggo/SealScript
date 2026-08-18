namespace SealScript;

public class Variable
{
    public SealValue Value { get; set; }
    public ArgumentType AllowedTypes { get; init; }
}