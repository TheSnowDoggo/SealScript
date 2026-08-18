using System.Collections.Generic;

namespace SealScript;

public class CallContext : ILineNumbered
{
    private readonly Dictionary<string, Stack<Variable>> _variables = [];
    private readonly Stack<HashSet<string>> _scopes = [];

    public CallContext(SealProgram sealProgram, CallContext parentContext)
    {
        SealProgram = sealProgram;
        ParentContext = parentContext;
    }
    
    public SealProgram SealProgram { get; }
    public CallContext ParentContext { get; }
    
    public int Line { get; set; }
    public int Column { get; set; }
    
    public void OpenScope()
    {
        _scopes.Push([]);
    }

    public void CloseScope()
    {
        if (!_scopes.TryPop(out HashSet<string> scope))
        {
            throw new SealException(Line, Column, "No scopes are defined.");
        }

        foreach (string name in scope)
        {
            if (!_variables.TryGetValue(name, out Stack<Variable> variableStack))
            {
                continue;
            }
            
            if (variableStack.Count > 0)
            {
                variableStack.Pop();
            }

            if (variableStack.Count == 0)
            {
                _variables.Remove(name);
            }
        }
    }

    public void DefineVariable(string name, SealValue value, ArgumentType allowedTypes = ArgumentType.Any)
    {
        if (!_scopes.TryPeek(out HashSet<string> scope))
        {
            throw new SealException(Line, Column, "No scopes are defined.");
        }

        if (!scope.Add(name))
        {
            throw new SealException(Line, Column, $"Variable with name '{name}' has already been defined in this scope.");
        }
        
        if (!_variables.TryGetValue(name, out Stack<Variable> variableStack))
        {
            _variables[name] = variableStack = [];
        }
        
        if (allowedTypes != ArgumentType.None
            && !allowedTypes.IsAssignableTo(value.ValueType))
        {
            throw new SealException(this,
                $"Variable {name} expected value of type {allowedTypes.ToArgumentString()}, got {value.ValueType}.");
        }

        var variable = new Variable()
        {
            Value = value,
            AllowedTypes = allowedTypes,
        };
        
        variableStack.Push(variable);
    }

    public void SetValue(string name, SealValue newValue)
    {
        Variable variable = GetVariable(name);

        if (variable == null)
        {
            throw new SealException(this, $"No variable with name {name} defined in current scope.");
        }

        if (variable.AllowedTypes == ArgumentType.None)
        {
            throw new SealException(this, $"Variable {name} cannot be set as it is immutable.");
        }

        if (!variable.AllowedTypes.IsAssignableTo(newValue.ValueType))
        {
            throw new SealException(this,
                $"Variable {name} expected value of type {variable.AllowedTypes.ToArgumentString()}, got {newValue.ValueType}.");
        }
        
        variable.Value = newValue;
    }

    public SealValue GetValue(string name)
    {
        Variable variable = GetVariable(name);

        if (variable != null)
        {
            return variable.Value;
        }
        
        if (SealGlobal.ClassInstance.StaticFields.TryGetValue(name, out SealField field))
        {
            return field.Get(this, name, SealValue.Nil);
        }
        
        throw new SealException(this, $"No variable with name {name} defined in current scope.");
    }

    private Variable GetVariable(string name)
    {
        CallContext current = this;
        
        while (current != null)
        {
            if (current._variables.TryGetValue(name, out Stack<Variable> variableStack))
            {
                return variableStack.Peek();
            }
            
            current = current.ParentContext;
        }

        return null;
    }
}