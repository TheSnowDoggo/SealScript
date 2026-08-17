using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Array")]
public class SealArray : SealObject, IReadOnlyCollection<SealValue>
{
    private readonly List<SealValue> _values;

    public SealArray()
    {
        _values = [];
    }
    
    public SealArray(int length)
    {
        _values = new List<SealValue>(length);

        for (int i = 0; i < length; i++)
        {
            _values.Add(default);
        }
    }

    public SealArray(List<SealValue> values)
    {
        _values = values;
    }
    
    static SealArray()
    {
        ClassInstance = ClassFactory<SealArray>.Generate();
    }

    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;

    public int Count => _values.Count;
    
    [PropertyExport]
    public SealValue Size => _values.Count;

    public SealValue this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    public override SealValue this[CallContext context, SealValue index]
    {
        get => _values[GetArrayIndex(context, index)];
        set => _values[GetArrayIndex(context, index)] = value;
    }

    [FunctionExport(ArgumentType.Number, MinArgs = 0)]
    public static SealValue New(CallArgs args)
    {
        return args.Length == 0 ? new SealArray() : new SealArray((int)args[0]);
    }
    
    [FunctionExport(ArgumentType.Any)]
    public void PushBack(CallArgs args)
    {
        _values.Add(args[0]);
    }
    
    [FunctionExport(ArgumentType.Any)]
    public void PushFront(CallArgs args)
    {
        _values.Insert(0, args[0]);
    }

    [FunctionExport(ArgumentType.Number, ArgumentType.Any)]
    public void Insert(CallArgs args)
    {
        _values.Insert((int)args[0], args[1]);
    }

    [FunctionExport(ArgumentType.Any)]
    public SealValue Erase(CallArgs args)
    {
        return _values.Remove(args[0]);
    }
    
    [FunctionExport(ArgumentType.Number)]
    public SealValue EraseAt(CallArgs args)
    {
        int index = (int)args[0];

        if (index < 0 || index >= _values.Count)
        {
            return false;
        }
        
        _values.RemoveAt(index);
        
        return true;
    }

    [FunctionExport]
    public void Clear(CallArgs args)
    {
        _values.Clear();
    }

    private int GetArrayIndex(CallContext context, SealValue index)
    {
        if (index.ValueType != SealValueType.Number)
        {
            throw new SealException(context,
                $"Expected number while indexing string, got {index.ValueType}.");
        }

        int indexValue = (int)index;

        if (indexValue < 0 || indexValue >= Count)
        {
            throw new SealException(context,
                $"Array index {indexValue} was out of range.");
        }

        return indexValue;
    }
    
    public override string ToString()
    {
        if (_values.Count == 0)
        {
            return "[  ]";
        }
        
        var sb = new StringBuilder();

        foreach (var value in _values)
        {
            sb.Append($", {value.ToString(false)}");
        }

        sb[0] = '[';
        sb[1] = ' ';

        sb.Append(" ]");
        
        return sb.ToString();
    }

    public IEnumerator<SealValue> GetEnumerator()
    {
        return _values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _values.GetEnumerator();
    }
}