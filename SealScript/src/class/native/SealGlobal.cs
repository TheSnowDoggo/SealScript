using System;
using System.IO;
using System.Reflection;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Global")]
public sealed class SealGlobal : SealObject
{
    static SealGlobal()
    {
        ClassInstance = ClassFactory<SealGlobal>.Generate();
    }
    
    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;
    
    #region Exports
    
    // MATH EXPORTS
    
    [FieldExport(Name = "PI")]
    public static readonly double PI = Math.PI;
    
    [FieldExport(Name = "E")]
    public static readonly double E = Math.E;
    
    [FieldExport(Name = "TAU")]
    public static readonly double Tau = Math.Tau;
    
    [FieldExport]
    public static readonly NativeFunction Sin = ToUnary(Math.Sin);
    
    [FieldExport]
    public static readonly NativeFunction Cos = ToUnary(Math.Cos);
    
    [FieldExport]
    public static readonly NativeFunction Tan = ToUnary(Math.Tan);
    
    [FieldExport]
    public static readonly NativeFunction Asin = ToUnary(Math.Asin);
    
    [FieldExport]
    public static readonly NativeFunction Acos = ToUnary(Math.Acos);
    
    [FieldExport]
    public static readonly NativeFunction Atan = ToUnary(Math.Atan);
    
    [FieldExport]
    public static readonly NativeFunction Sqrt = ToUnary(Math.Sqrt);
    
    [FieldExport]
    public static readonly NativeFunction Cbrt = ToUnary(Math.Cbrt);

    [FieldExport]
    public static readonly NativeFunction Pow = ToBinary(Math.Pow);
    
    [FieldExport]
    public static readonly NativeFunction Abs = ToUnary(Math.Abs);
    
    [FieldExport]
    public static readonly NativeFunction Exp = ToUnary(Math.Exp);

    [FieldExport]
    public static readonly NativeFunction Floor = ToUnary(Math.Floor);
    
    [FieldExport]
    public static readonly NativeFunction Ceil = ToUnary(Math.Ceiling);
    
    [FieldExport]
    public static readonly NativeFunction Truncate = ToUnary(Math.Truncate);
    
    [FieldExport]
    public static readonly NativeFunction Round = ToUnary(Math.Round);

    [FieldExport]
    public static readonly NativeFunction Min = ToBinary(Math.Min);
    
    [FieldExport]
    public static readonly NativeFunction Max = ToBinary(Math.Max);
    
    [FieldExport]
    public static readonly NativeFunction Clamp = ToTernary(Math.Clamp);
    
    [FieldExport]
    public static readonly NativeFunction Lerp = ToTernary(double.Lerp);

    [FunctionExport(ArgumentType.Number, ArgumentType.Number, MinArgs = 1)]
    public static SealValue Log(CallArgs args)
    {
        return args.Length == 1 ? Math.Log((double)args[0]) : Math.Log((double)args[0], (double)args[1]);
    }
    
    // IO EXPORTS
    
    [FunctionExport(MaxArgs = NativeFunction.AnyArgs)]
    public static void Print(CallArgs args)
    {
        Console.Write(string.Join(null, args.Values.ToArray()));
    }
    
    [FunctionExport(MaxArgs = NativeFunction.AnyArgs)]
    public static void Println(CallArgs args)
    {
        Console.WriteLine(string.Join(null, args.Values.ToArray()));
    }

    [FunctionExport]
    public static SealValue Readln(CallArgs args)
    {
        return Console.ReadLine() ?? string.Empty;
    }

    [FunctionExport]
    public static SealValue Read(CallArgs args)
    {
        return Console.Read();
    }

    [FunctionExport]
    public static void Clear(CallArgs args)
    {
        Console.Clear();
    }

    [FunctionExport(ArgumentType.String)]
    public static SealValue Require(CallArgs args)
    {
        string filepath = args[0].AsString();

        if (!Path.IsPathFullyQualified(filepath) && args.Context != null)
        {
            string executingFilePath = args.Context.SealProgram?.ExecutingFilePath;

            if (executingFilePath != null)
            {
                string directory = Path.GetDirectoryName(executingFilePath);

                if (directory != null)
                {
                    filepath = Path.Combine(directory, filepath);
                }
            }
        }

        return UserFunction.CreateFromScript(filepath).Invoke();
    }
    
    // RANGE EXPORTS

    [FunctionExport(ArgumentType.Number, ArgumentType.Number, ArgumentType.Number, MinArgs = 1)]
    public static SealValue Range(CallArgs args) =>  args.Length switch
    {
        1 => new SealRange(SealRange.CreateRange((double)args[0])),
        2 => new SealRange(SealRange.CreateRange((double)args[0], (double)args[1])),
        3 => new SealRange(SealRange.CreateRange((double)args[0], (double)args[1], (double)args[2])),
        _ => throw new SealException(args.Context, "Received invalid number of arguments."),
    };
    
    // NATIVE EXPORTS

    [FunctionExport(ArgumentType.Any)]
    public static SealValue Typeof(CallArgs args)
    {
        return args[0].ValueType.ToString();
    }

    [FunctionExport(ArgumentType.Function | ArgumentType.Object | ArgumentType.Class)]
    public static SealValue Nameof(CallArgs args)
    {
        SealValue value = args[0];

        return value.ValueType switch
        {
            SealValueType.Function => ((Function)value).Name,
            SealValueType.Object => ((SealObject)value).Class.Name,
            SealValueType.Class => ((SealClass)value).Name,
            _ => throw new ArgumentException($"Value {value.ValueType} must be a Function, Object or Class."),
        };
    }

    [FunctionExport(ArgumentType.String)]
    public static SealValue Len(CallArgs args)
    {
        return args[0].AsString().Length;
    }
    
    #endregion
    
    public static void ImportClass(SealClass sealClass)
    {
        ClassInstance.AddStaticField(sealClass.Name, new ValueSealField(sealClass));
    }

    public static void ImportAssembly(Assembly assembly)
    {
        foreach (Type type in assembly.GetExportedTypes())
        {
            if (!type.IsAssignableTo(typeof(SealObject)))
            {
                continue;
            }

            var attribute = type.GetCustomAttribute<SealObjectExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            FieldInfo fieldInfo = type.GetField(attribute.ClassField);

            if (fieldInfo == null)
            {
                throw new InvalidOperationException($"Field {attribute.ClassField} not found in class {type.Name}.");
            }

            if (!fieldInfo.IsStatic)
            {
                throw new InvalidOperationException($"Field {attribute.ClassField} in class {type.Name} must be static.");
            }
            
            if (!fieldInfo.FieldType.IsAssignableTo(typeof(SealClass)))
            {
                throw new InvalidOperationException($"Field {attribute.ClassField} in class {type.Name} must be a SealClass.");
            }

            var sealClass = (SealClass)fieldInfo.GetValue(null);
            
            ImportClass(sealClass);
        }
    }

    public static void ImportExecutingAssembly()
    {
        ImportAssembly(Assembly.GetExecutingAssembly());
    }
    
    private static NativeFunction ToUnary(Func<double, double> unary)
    {
        return new NativeFunction((_, args) => unary((double)args[0]))
        {
            MinArgs = 1, MaxArgs = 1,
            ArgumentTypes = [ArgumentType.Number]
        };
    }
    
    private static NativeFunction ToBinary(Func<double, double, double> binary)
    {
        return new NativeFunction((_, args) => binary((double)args[0], (double)args[1]))
        {
            MinArgs = 2, MaxArgs = 2,
            ArgumentTypes = [ArgumentType.Number, ArgumentType.Number]
        };
    }
    
    private static NativeFunction ToTernary(Func<double, double, double, double> ternary)
    {
        return new NativeFunction((_, args) => ternary((double)args[0], (double)args[1], (double)args[2]))
        {
            MinArgs = 3, MaxArgs = 3,
            ArgumentTypes = [ArgumentType.Number, ArgumentType.Number, ArgumentType.Number]
        };
    }
}