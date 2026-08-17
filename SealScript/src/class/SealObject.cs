namespace SealScript;

public abstract class SealObject
{
    public abstract SealClass Class { get; }

    public virtual SealValue this[CallContext context, SealValue index]
    {
        get => throw new SealException(context,
            $"Object of class {Class.Name} cannot be get indexed.");
        set => throw new SealException(context,
            $"Object of class {Class.Name} cannot be set indexed.");
    }

    public override string ToString()
    {
        return $"Object<{Class.Name}>";
    }
}