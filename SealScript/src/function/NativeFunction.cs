using System;
using System.Text;

namespace SealScript;

public class NativeFunction : Function
{
    public NativeFunction(Func<SealObject, CallArgs, SealValue> invoker)
    {
        Invoker = invoker;
    }
    
    public override string Name => CustomName ?? Invoker.Method.Name;
    
    public Func<SealObject, CallArgs, SealValue> Invoker { get; }
    
    public string CustomName { get; init; }
    
    public Type ExpectedObjectType { get; init; }
    
    internal override SealValue _Invoke(SealObject self, CallArgs args)
    {
        if (ExpectedObjectType != null)
        {
            if (self == null)
            {
                throw new SealException(args.Context,
                    $"{this} must be called as a instance function.");
            }

            Type selfType = self.GetType();
            
            if (selfType != ExpectedObjectType)
            {
                throw new SealException(args.Context,
                    $"{this} expected instance object to be of type {ExpectedObjectType.Name}, got {selfType}.");
            }
        }
        
        return Invoker(self, args);
    }
}