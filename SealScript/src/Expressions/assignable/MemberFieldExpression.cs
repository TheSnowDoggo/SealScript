namespace SealScript.Expressions;

public class MemberFieldExpression : AssignableExpression
{
    public MemberFieldExpression(Expression instanceExpression, string memberIdentifier)
    {
        InstanceExpression = instanceExpression;
        MemberIdentifier = memberIdentifier;
    }
    
    public Expression InstanceExpression { get; }
    public string MemberIdentifier { get; }

    public override SealValue Evaluate(CallContext context)
    {
        SealField field = GetField(context, out SealValue self);

        return field.Get(context, self);
    }

    public override void Assign(CallContext context, SealValue value)
    {
        SealField field = GetField(context, out SealValue self);
        
        field.Set(context, self, value);
    }

    public SealField GetField(CallContext context, out SealValue self)
    {
        self = InstanceExpression.Evaluate(context);

        SealClass sealClass;
        SealField field;

        switch (self.ValueType)
        {
        case SealValueType.Object:
            var obj = (SealObject)self;
            sealClass = obj.Class;

            if (!sealClass.Fields.TryGetValue(MemberIdentifier, out field))
            {
                throw new SealException(context,
                    $"Object of class {sealClass.Name} does not contain instance member {MemberIdentifier}.");
            }
            
            return field;
        case SealValueType.Class:
            sealClass = (SealClass)self;

            if (!sealClass.StaticFields.TryGetValue(MemberIdentifier, out field))
            {
                throw new SealException(context,
                    $"Class {sealClass.Name} does not contain static member {MemberIdentifier}.");
            }
            
            return field;
        default:
            throw new SealException(context,
                $"Cannot access member from type {self.ValueType}.");
        }
    }
}