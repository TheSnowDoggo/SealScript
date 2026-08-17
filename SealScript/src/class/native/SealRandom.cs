using System;

namespace SealScript;

[SealObjectExport(nameof(ClassInstance), Name = "Random")]
public class SealRandom : SealObject
{
    private readonly Random _random;
    
    public SealRandom()
    {
        _random = new Random();
    }
    
    public SealRandom(int seed)
    {
        _random = new Random(seed);
    }
    
    static SealRandom()
    {
        ClassInstance = ClassFactory<SealRandom>.Generate();
    }

    public static readonly SealClass ClassInstance;

    public override SealClass Class => ClassInstance;

    [FunctionExport(ArgumentType.Number, MinArgs = 0)]
    public static SealValue New(CallArgs args)
    {
        return args.Length == 0 ? new SealRandom() : new SealRandom((int)args[0]);
    }

    [FunctionExport(ArgumentType.Number, ArgumentType.Number, MinArgs = 0)]
    public SealValue Randf(CallArgs args)=> args.Length switch
    {
        0 => _random.NextDouble(),
        1 => _random.NextDouble() * (double)args[0],
        2 => double.Lerp((double)args[0], (double)args[1], _random.NextDouble()),
        _ => throw new SealException(args.Context, "Received invalid number of arguments.")
    };
    
    [FunctionExport(ArgumentType.Number, ArgumentType.Number, MinArgs = 0)]
    public SealValue Randi(CallArgs args) => args.Length switch
    {
        0 => _random.Next(),
        1 => _random.Next((int)args[0]),
        2 => _random.Next((int)args[0], (int)args[1]),
        _ => throw new SealException(args.Context, "Received invalid number of arguments.")
    };
}