namespace SealScript;

public abstract class SealField
{
    public abstract SealValue Get(CallContext context, string name, SealValue self);

    public abstract void Set(CallContext context, string name, SealValue self, SealValue value);
}