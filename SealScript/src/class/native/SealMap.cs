using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Map")]
public class SealMap : SealObject, IReadOnlyCollection<SealValue>
{
    private readonly Dictionary<SealValue, SealValue> _values;

    public SealMap()
    {
        _values = [];
    }
    
    public SealMap(Dictionary<SealValue, SealValue> values)
    {
        _values = values;
    }
    
    static SealMap()
    {
        ClassInstance = ClassFactory<SealMap>.Generate();
    }
    
    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;

    public int Count => _values.Count;
    
    [PropertyExport]
    public SealValue Size => _values.Count;

    public override SealValue this[CallContext context, SealValue index]
    {
        get => _values.GetValueOrDefault(index, SealValue.Nil);
        set
        {
            if (value.ValueType == SealValueType.Nil)
            {
                _values.Remove(index);
            }
            else
            {
                _values[index] = value;
            }
        }
    }

    [FunctionExport]
    public static SealValue New(CallArgs args)
    {
        return new SealMap();
    }

    [FunctionExport(ArgumentType.Any, ArgumentType.Any)]
    public SealValue Add(CallArgs args)
    {
        return _values.TryAdd(args[0], args[1]);
    }

    [FunctionExport]
    public SealValue Erase(CallArgs args)
    {
        return _values.Remove(args[0]);
    }

    [FunctionExport(ArgumentType.Any)]
    public SealValue Contains(CallArgs args)
    {
        return _values.ContainsKey(args[0]);
    }

    [FunctionExport(ArgumentType.Any, ArgumentType.Any, MinArgs = 1)]
    public SealValue Get(CallArgs args)
    {
        return _values.GetValueOrDefault(args[0], args.Length > 1 ? args[1] : SealValue.Nil);
    }
    
    [FunctionExport]
    public void Clear(CallArgs args)
    {
        _values.Clear();
    }

    public IEnumerator<SealValue> GetEnumerator()
    {
        foreach (KeyValuePair<SealValue, SealValue> kvp in _values)
        {
            yield return kvp.Key;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        if (_values.Count == 0)
        {
            return "{  }";
        }
        
        var sb = new StringBuilder();

        foreach (var kvp in _values)
        {
            sb.Append($", {kvp.Key.ToString(false)}: {kvp.Value.ToString(false)}");
        }

        sb[0] = '{';
        sb[1] = ' ';

        sb.Append(" }");
        
        return sb.ToString();
    }
}