using System;

namespace SealScript;

[AttributeUsage(AttributeTargets.Property)]
public class PropertyExportAttribute : Attribute
{
    public PropertyExportAttribute(ArgumentType settableType = ArgumentType.Any)
    {
        SettableType = settableType;
    }
    
    public ArgumentType SettableType { get; }
    
    public string Name { get; init; }
}