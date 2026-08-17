using System;

namespace SealScript;

[AttributeUsage(AttributeTargets.Field)]
public class FieldExportAttribute : Attribute
{
    public string Name { get; init; }
}