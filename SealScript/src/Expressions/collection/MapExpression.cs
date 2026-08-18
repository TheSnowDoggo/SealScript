using System.Collections.Generic;
using System.Text;

namespace SealScript.Expressions;

public class MapExpression : Expression
{
    public MapExpression(Dictionary<Expression, Expression> itemExpressions = null)
    {
        ItemExpressions = itemExpressions;
    }
    
    public Dictionary<Expression, Expression> ItemExpressions { get; }
    
    public override SealValue Evaluate(CallContext context)
    {
        if (ItemExpressions == null)
        {
            return new SealMap();
        }
        
        var values = new Dictionary<SealValue, SealValue>();

        foreach (var kvp in ItemExpressions)
        {
            SealValue key = kvp.Key.Evaluate(context);
            SealValue value = kvp.Value.Evaluate(context);

            if (!values.TryAdd(key, value))
            {
                throw new SealException(context, $"Failed to create map, duplicate key {key} was defined.");
            }
        }

        return new SealMap(values);
    }

    public override string ToString()
    {
        if (ItemExpressions == null)
        {
            return "Map.new{  }";
        }

        var sb = new StringBuilder();

        sb.Append("Map.new");

        int start = sb.Length;

        foreach (var kvp in ItemExpressions)
        {
            sb.Append($", {kvp.Key}: {kvp.Value}");
        }

        sb[start] = '{';
        sb[start + 1] = ' ';
        
        sb.Append(" }");
        
        return sb.ToString();
    }
}