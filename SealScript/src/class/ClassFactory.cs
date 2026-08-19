using System;
using System.Linq;
using System.Reflection;

namespace SealScript;

public static class ClassFactory<TObject>
    where TObject : SealObject
{
    public static SealClass Generate()
    {
        var attribute = typeof(TObject).GetCustomAttribute<SealObjectExportAttribute>();
        
        var sealClass = new SealClass()
        {
            Name = attribute?.Name ?? typeof(TObject).Name,
        };

        GenerateMethods(sealClass);

        GenerateProperties(sealClass);
        
        GenerateStaticFields(sealClass);
        
        return sealClass;
    }

    private static void GenerateMethods(SealClass sealClass)
    {
        foreach (MethodInfo methodInfo in typeof(TObject)
                     .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = methodInfo.GetCustomAttribute<FunctionExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            Type returnType = methodInfo.ReturnType;

            if (returnType != typeof(SealValue) 
                && returnType != typeof(void))
            {
                throw new InvalidOperationException($"Method {methodInfo.Name} must return a SealValue or void.");
            }
            
            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 && parameters[0].ParameterType != typeof(CallArgs))
            {
                throw new InvalidOperationException($"Method {methodInfo.Name} must take {typeof(CallArgs)} as the only parameter.");
            }
        
            string name = attribute.Name ?? methodInfo.Name.ToSnakeCase();
        
            if (methodInfo.IsStatic)
            {
                Func<CallArgs, SealValue> methodInvoke;

                if (returnType == typeof(void))
                {
                    var voidInvoke = CreateDelegate<Action<CallArgs>>(methodInfo);
                    
                    methodInvoke = args =>
                    {
                        voidInvoke(args);
                        return SealValue.Nil;
                    };
                }
                else
                {
                    methodInvoke = CreateDelegate<Func<CallArgs, SealValue>>(methodInfo);
                }

                var nativeFunction = new NativeFunction((_, args) => methodInvoke(args))
                {
                    CustomName = name,
                    MinArgs = attribute.MinArgs,
                    MaxArgs = attribute.MaxArgs,
                    ArgumentTypes = attribute.ArgumentTypes,
                };
                
                var field = new ValueSealField(nativeFunction);
                
                sealClass.AddStaticField(name, field);
            }
            else
            {
                Func<TObject, CallArgs, SealValue> methodInvoke;

                if (returnType == typeof(void))
                {
                    var voidInvoke = CreateDelegate<Action<TObject, CallArgs>>(methodInfo);

                    methodInvoke = (self, args) =>
                    {
                        voidInvoke(self, args);
                        return SealValue.Nil;
                    };
                }
                else
                {
                    methodInvoke = CreateDelegate<Func<TObject, CallArgs, SealValue>>(methodInfo);
                }

                var nativeFunction = new NativeFunction((self, args) => methodInvoke((TObject)self, args))
                {
                    CustomName = name,
                    MinArgs = attribute.MinArgs,
                    MaxArgs = attribute.MaxArgs,
                    ArgumentTypes = attribute.ArgumentTypes,
                    ExpectedObjectType = typeof(TObject),
                };
                
                var field = new ValueSealField(nativeFunction);
                
                sealClass.AddInstanceField(name, field);
            }
        }
    }

    private static void GenerateStaticFields(SealClass sealClass)
    {
        foreach (FieldInfo fieldInfo in typeof(TObject)
                     .GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = fieldInfo.GetCustomAttribute<FieldExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            string name = attribute.Name ?? fieldInfo.Name.ToSnakeCase();

            var value = SealValue.CreateFrom(fieldInfo.GetValue(null));

            var field = new ValueSealField(value);

            sealClass.AddStaticField(name, field);
        }
    }

    private static void GenerateProperties(SealClass sealClass)
    {
        foreach (PropertyInfo propertyInfo in typeof(TObject)
                     .GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
        {
            var attribute = propertyInfo.GetCustomAttribute<PropertyExportAttribute>();

            if (attribute == null)
            {
                continue;
            }

            if (propertyInfo.PropertyType != typeof(SealValue))
            {
                throw new InvalidOperationException($"Property {propertyInfo.Name} must take a SealValue.");
            }
            
            string name = attribute.Name ?? propertyInfo.Name.ToSnakeCase();

            bool isStatic = propertyInfo.GetAccessors().Any(a => a.IsStatic);
            
            Func<SealValue, SealValue> getter = null;
            Action<SealValue, SealValue> setter = null;
            
            if (isStatic)
            {
                if (propertyInfo.GetMethod != null)
                {
                    var propertyGetter = CreateDelegate<Func<SealValue>>(propertyInfo.GetMethod);
                    
                    getter = _ => propertyGetter();
                }

                if (propertyInfo.SetMethod != null)
                {
                    var propertySetter = CreateDelegate<Action<SealValue>>(propertyInfo.SetMethod);
                    
                    setter = (_, value) => propertySetter(value);
                }
                
                var field = new PropertySealField(getter, setter, attribute.SettableType);

                sealClass.AddStaticField(name, field);
            }
            else
            {
                if (propertyInfo.GetMethod != null)
                {
                    var propertyGetter = CreateDelegate<Func<TObject, SealValue>>(propertyInfo.GetMethod);

                    getter = self => propertyGetter((TObject)self);
                }

                if (propertyInfo.SetMethod != null)
                {
                    var propertySetter = CreateDelegate<Action<TObject, SealValue>>(propertyInfo.SetMethod);
                    
                    setter = (self, value) => propertySetter((TObject)self, value);
                }
                
                var field = new PropertySealField(getter, setter, attribute.SettableType);
                
                sealClass.AddInstanceField(name, field);
            }
        }
    }
    
    private static T CreateDelegate<T>(MethodInfo method)
        where T : Delegate
    {
        return (T)Delegate.CreateDelegate(typeof(T), method);
    }
}