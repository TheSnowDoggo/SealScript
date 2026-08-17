using System.Collections.Generic;

namespace SealScript.Expressions;

public class ArrayExpression : Expression
{
    public ArrayExpression(Expression[] itemExpressions = null)
    {
        ItemExpressions = itemExpressions;
    }
    
    public Expression[] ItemExpressions { get; }

    public override SealValue Evaluate(CallContext context)
    {
        if (ItemExpressions == null)
        {
            return new SealArray();
        }
        
        int length = ItemExpressions.Length;
        
        var values = new List<SealValue>(length);

        for (int i = 0; i < length; i++)
        {
            values.Add(ItemExpressions[i].Evaluate(context));
        }

        return new SealArray(values);
    }
}