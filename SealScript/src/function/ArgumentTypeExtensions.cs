using System.Text;

namespace SealScript;

public static class ArgumentTypeExtensions
{
    private const int ArgumentTypeCount = 7;
    
    public static string ToArgumentString(this ArgumentType self)
    {
        switch (self)
        {
        case ArgumentType.None:
            return "None";
        case ArgumentType.Any:
            return "Any";
        }

        var sb = new StringBuilder();
        bool first = true;

        for (int i = 0; i < ArgumentTypeCount; i++)
        {
            var argumentType = (ArgumentType)(1 << i);

            if ((self & argumentType) == 0)
            {
                continue;
            }

            if (!first)
            {
                sb.Append(" | ");
            }
            else
            {
                first = false;
            }
            
            sb.Append(argumentType);
        }

        return sb.ToString();
    }
    
    public static bool IsSingleType(this ArgumentType self)
    {
        bool found = false;

        for (int i = 0; i < ArgumentTypeCount; i++)
        {
            var argumentType = (ArgumentType)(1 << i);

            if ((self & argumentType) == 0)
            {
                continue;
            }

            if (found)
            {
                return false;
            }

            found = true;
        }

        return found;
    }

    public static bool IsObjectType(this ArgumentType self)
    {
        return (self & (ArgumentType.Function | ArgumentType.Object | ArgumentType.Class)) != 0;
    }

    public static bool IsNilAssignable(this ArgumentType self)
    {
        return (self & ArgumentType.Nil) != 0
               || IsObjectType(self)
               || !IsSingleType(self);
    }
    
    public static bool IsTypeIncluded(this ArgumentType argumentType, SealValueType sealValueType)
    {
        return (argumentType & SealValue.ToArgumentType(sealValueType)) != 0;
    }

    public static bool IsAssignableTo(this ArgumentType argumentType, SealValueType sealValueType)
    {
        return argumentType.IsTypeIncluded(sealValueType)
               || (argumentType.IsNilAssignable() && sealValueType == SealValueType.Nil);
    }
}