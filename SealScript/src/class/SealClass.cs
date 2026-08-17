using System;
using System.Collections.Generic;

namespace SealScript;

public class SealClass
{
    public string Name { get; init; }

    public Dictionary<string, SealField> Fields { get; init; } = [];
    public Dictionary<string, SealField> StaticFields { get; init; } = [];

    public void AddStaticField(string name, SealField field)
    {
        if (!StaticFields.TryAdd(name, field))
        {
            throw new InvalidOperationException($"A static field with name {name} has already been defined.");
        }
    }
    
    public void AddInstanceField(string name, SealField field)
    {
        if (!Fields.TryAdd(name, field))
        {
            throw new InvalidOperationException($"An instance field with name {name} has already been defined.");
        }
    }
    
    public override string ToString()
    {
        return $"Class<{Name}>";
    }
}