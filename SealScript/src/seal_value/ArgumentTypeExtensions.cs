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

        for (int i = 0; i < ArgumentTypeCount; i++)
        {
            var argumentType = (ArgumentType)(1 << i);

            if ((self & argumentType) == 0)
            {
                continue;
            }

            sb.Append(argumentType);
            sb.Append(" | ");
        }

        return sb.ToString(0, sb.Length - 3);
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
    
    public static bool IsTypeIncluded(this ArgumentType self, SealValueType sealValueType)
    {
        return (self & SealValue.ToArgumentType(sealValueType)) != 0;
    }

    public static bool IsAssignableFrom(this ArgumentType self, SealValueType sealValueType)
    {
        return self.IsTypeIncluded(sealValueType)
               || (sealValueType == SealValueType.Nil && self.IsNilAssignable());
    }

    public static bool IsConst(this ArgumentType self)
    {
        return (self & ArgumentType.Const) != 0;
    }
}