namespace SealScript;

public abstract class SealField
{
    public abstract SealValue Get(CallContext context, SealValue self);

    public abstract void Set(CallContext context, SealValue self, SealValue value);
}