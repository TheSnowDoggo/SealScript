namespace SealScript;

public abstract class Statement
{
    public int Line { get; init; }
    public int Column { get; init; }

    public ReturnValue Run(CallContext context)
    {
        context.Line = Line;
        context.Column = Column;
        
        return _Run(context);
    }
    
    protected abstract ReturnValue _Run(CallContext context);
}