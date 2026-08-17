using System.Collections.Generic;

namespace SealScript;

public class CallContext
{
    private readonly Dictionary<string, Stack<SealValue>> _variables = [];
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
            if (!_variables.TryGetValue(name, out Stack<SealValue> variableStack))
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

    public void DefineVariable(string name, SealValue value)
    {
        if (!_scopes.TryPeek(out HashSet<string> scope))
        {
            throw new SealException(Line, Column, "No scopes are defined.");
        }

        if (!scope.Add(name))
        {
            throw new SealException(Line, Column, $"Variable with name '{name}' has already been defined in this scope.");
        }
        
        if (!_variables.TryGetValue(name, out Stack<SealValue> variableStack))
        {
            _variables[name] = variableStack = [];
        }
        
        variableStack.Push(value);
    }

    public void SetValue(string name, SealValue newValue)
    {
        CallContext current = this;

        while (current != null)
        {
            if (current._variables.TryGetValue(name, out Stack<SealValue> variableStack))
            {
                variableStack.Pop();
                variableStack.Push(newValue);
                return;
            }
            
            current = current.ParentContext;
        }
        
        throw new SealException(Line, Column, $"No variable with name {name} defined in current scope.");
    }

    public SealValue GetValue(string name)
    {
        CallContext current = this;

        while (current != null)
        {
            if (current._variables.TryGetValue(name, out Stack<SealValue> variableStack))
            {
                return variableStack.Peek();
            }
            
            current = current.ParentContext;
        }
        
        if (SealGlobal.ClassInstance.StaticFields.TryGetValue(name, out SealField field))
        {
            return field.Get(this, SealValue.Nil);
        }
        
        throw new SealException(Line, Column, $"No variable with name {name} defined in current scope.");
    }
}