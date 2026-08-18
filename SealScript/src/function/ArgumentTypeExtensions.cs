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
}