using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SealScript;

public readonly struct SealValue : IEquatable<SealValue>
{
    private readonly double _value;
    private readonly object _obj;
    
    public SealValue(bool value)
    {
        ValueType = SealValueType.Bool;
        _value = value ? 1 : 0;
    }
    
    public SealValue(double value)
    {
        ValueType = SealValueType.Number;
        _value = value;
    }
    
    public SealValue(string obj)
        : this(SealValueType.String, obj)
    {
    }

    public SealValue(Function obj)
        : this(SealValueType.Function, obj)
    {
    }

    public SealValue(SealObject sealObj)
        : this(SealValueType.Object, sealObj)
    {
    }

    public SealValue(SealClass sealClass)
        : this(SealValueType.Class, sealClass)
    {
    }
    
    private SealValue(SealValueType valueType, object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        
        ValueType = valueType;
        _obj = obj;
    }

    public static readonly SealValue Nil = new SealValue();
    
    public SealValueType ValueType { get; }

    public static bool operator ==(SealValue left, SealValue right) => left.Equals(right);
    public static bool operator !=(SealValue left, SealValue right) => !left.Equals(right);

    public static implicit operator SealValue(bool value) => new SealValue(value);
    public static implicit operator SealValue(string value) => new SealValue(value);
    public static implicit operator SealValue(double value) => new SealValue(value);
    public static implicit operator SealValue(Function value) => new SealValue(value);
    public static implicit operator SealValue(SealObject value) => new SealValue(value);
    public static implicit operator SealValue(SealClass value) => new SealValue(value);
    
    public static explicit operator bool(SealValue value)
        => value._value != 0;
    public static explicit operator double(SealValue value)
        => value._value;
    public static explicit operator string(SealValue value)
        => (string)value._obj;
    public static explicit operator Function(SealValue value)
        => (Function)value._obj;
    public static explicit operator SealObject(SealValue value)
        => (SealObject)value._obj;
    public static explicit operator SealClass(SealValue value)
        => (SealClass)value._obj;
    
    public static SealValue CreateFrom(object obj)
    {
        return obj switch
        {
            null => Nil,
            SealValue  sealValue   => sealValue,
            bool       boolValue   => boolValue,
            double     doubleValue => doubleValue,
            string     stringValue => stringValue,
            Function   function    => function,
            SealObject sealObject  => sealObject,
            SealClass  sealClass   => sealClass,
            _ => throw new InvalidOperationException($"Cannot convert type {obj.GetType().Name} to SealValue.")
        };
    }
    
    public static ArgumentType ToArgumentType(SealValueType valueType)
    {
        return (ArgumentType)(1 << (int)valueType);
    }
    
    public bool AsBool()
    {
        return _value != 0;
    }
    
    public double AsNumber()
    {
        return _value;
    }

    public string AsString()
    {
        return (string)_obj;
    }

    public Function AsFunction()
    {
        return (Function)_obj;
    }
    
    public TFunction AsFunction<TFunction>()
        where TFunction : Function
    {
        return (TFunction)_obj;
    }

    public SealObject AsSealObject()
    {
        return (SealObject)_obj;
    }
    
    public TSealObject AsSealObject<TSealObject>()
        where TSealObject : SealObject
    {
        return (TSealObject)_obj;
    }

    public SealClass AsSealClass()
    {
        return (SealClass)_obj;
    }
    
    public TSealClass AsSealClass<TSealClass>()
        where TSealClass : SealClass
    {
        return (TSealClass)_obj;
    }
    
    public bool InterpretAsBool() => ValueType switch
    {
        SealValueType.Nil => false,
        SealValueType.Bool or SealValueType.Number => _value != 0,
        SealValueType.String => AsString().Length != 0,
        SealValueType.Function => true,
        SealValueType.Object => true,
        SealValueType.Class => true,
        _ => throw new InvalidOperationException($"Cannot convert type {ValueType} to bool."),
    };
    
    public bool Equals(SealValue other)
    {
        if (other.ValueType != ValueType)
        {
            return false;
        }

        return ValueType switch
        {
            SealValueType.Nil => true,
            SealValueType.Bool or SealValueType.Number => _value.Equals(other._value),
            _ => Equals(_obj, other._obj),
        };
    }

    public override bool Equals(object obj)
    {
        return obj is SealValue other && Equals(other);
    }

    public override int GetHashCode()
    {
        int hashCode = ValueType switch
        {
            SealValueType.Nil    => 0,
            SealValueType.Bool   => ((bool)this).GetHashCode(),
            SealValueType.Number => ((double)this).GetHashCode(),
            _ => _obj.GetHashCode(),
        };
        
        return HashCode.Combine(ValueType, hashCode);
    }

    public override string ToString()
    {
        return ToString(true);
    }

    public string ToString(bool useRawString)
    {
        return ValueType switch
        {
            SealValueType.Nil => "nil",
            SealValueType.Bool => AsBool() ? "true" : "false",
            SealValueType.Number => AsNumber().ToString(CultureInfo.InvariantCulture),
            SealValueType.String => useRawString ? AsString() : AsString().ToUnescaped(),
            _ => _obj.ToString(),
        };
    }
}