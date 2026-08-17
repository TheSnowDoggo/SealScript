using System.Collections.Generic;
using SealScript.Expressions;

namespace SealScript;

public class ClassDefinition
{
    public string Name { get; init; }
    
    public Dictionary<string, SealField> Fields { get; init; }
    public Expression[] FieldsExpressions { get; init; }
    
    public Dictionary<string, SealField> StaticFields { get; init; }
    public Expression[] StaticFieldsExpressions { get; init; }
    
    public FunctionDefinition ConstructorDefinition { get; init; }

    public UserSealClass CreateClass(CallContext context)
    {
        UserFunction constructor = ConstructorDefinition?.CreateFunction(context);

        SealValue[] staticFieldValues = InitializeStaticFields(context);
        
        var sealClass = new UserSealClass(staticFieldValues)
        {
            Name = Name,
            Fields = Fields,
            StaticFields = StaticFields,
        };

        sealClass.Constructor = new NativeFunction((_, args) =>
        {
            var instance = new UserSealObject(sealClass);

            for (int i = 0; i < FieldsExpressions.Length; i++)
            {
                instance.SetField(i, FieldsExpressions[i].Evaluate(context));
            }

            if (constructor != null)
            {
                instance.Constructing = true;
            
                constructor.Invoke(instance, args);
            
                instance.Constructing = false;
            }

            return instance;
        });
        
        return sealClass;
    }

    private SealValue[] InitializeStaticFields(CallContext context)
    {
        int length = StaticFieldsExpressions.Length;
        
        var values = new SealValue[length];

        for (int i = 0; i < length; i++)
        {
            values[i] = StaticFieldsExpressions[i].Evaluate(context);
        }
        
        return values;
    }
}