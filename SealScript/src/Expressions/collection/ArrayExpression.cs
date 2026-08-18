using System.Collections.Generic;
using System.Text;

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

    public override string ToString()
    {
        if (ItemExpressions == null)
        {
            return "Array.new[  ]";
        }

        var sb = new StringBuilder();

        sb.Append("Array.new");

        int start = sb.Length;

        for (int i = 0; i < ItemExpressions.Length; i++)
        {
            sb.Append($", {ItemExpressions[i]}");
        }

        sb[start] = '[';
        sb[start + 1] = ' ';
        
        sb.Append(" ]");
        
        return sb.ToString();
    }
}