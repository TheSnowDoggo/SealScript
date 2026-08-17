using System;

namespace SealScript;

[AttributeUsage(AttributeTargets.Method)]
public class FunctionExportAttribute : Attribute
{
    public FunctionExportAttribute(params ArgumentType[] argumentTypes)
    {
        ArgumentTypes = argumentTypes;
        
        MinArgs = argumentTypes.Length;
        MaxArgs = argumentTypes.Length;
    }
    
    public ArgumentType[] ArgumentTypes { get; }
    
    public string Name { get; init; }
    
    public int MinArgs { get; init; }
    public int MaxArgs { get; init; }
}