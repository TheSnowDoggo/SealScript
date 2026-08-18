namespace SealScript.Statements;

public class ControlStatement : Statement
{
    public ReturnValue ReturnValue { get; init; }
    
    protected override ReturnValue _Run(CallContext context)
    {
        return ReturnValue;
    }
}