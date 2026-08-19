using System;

namespace SealScript;

[AttributeUsage(AttributeTargets.Class)]
public class SealObjectExportAttribute : Attribute
{
    public SealObjectExportAttribute(string classField)
    {
        ClassField = classField;
    }
    
    public string ClassField { get; init; }
    public string Name { get; init; }
}