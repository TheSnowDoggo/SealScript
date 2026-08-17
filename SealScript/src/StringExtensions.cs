using System.Text;

namespace SealScript;

public static class StringExtensions
{
    public static string ToSnakeCase(this string s)
    {
        var sb = new StringBuilder();

        int i;

        // Skip leading whitespace
        for (i = 0; i < s.Length; i++)
        {
            if (s[i] > ' ')
            {
                break;
            }
        }

        int lastCatagory = -1;
        
        for (; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '_')
            {
                sb.Append('_');
                lastCatagory = -1;
                continue;
            }

            int catagory = c switch
            {
                ' ' => 0,
                >= 'A' and <= 'Z' => 1,
                >= 'a' and <= 'z' => 2,
                >= '0' and <= '9' => 3,
                _ => -1
            };

            // Order of catagories is the order where seperation shouldn't occur
            if (catagory < lastCatagory)
            {
                sb.Append('_');
            }
            
            lastCatagory = catagory;

            switch (catagory)
            {
            case 1: // Uppercase
                sb.Append((char)(c - 'A' + 'a'));
                break;
            case 2 or 3: // Lowercase or Digit
                sb.Append(c);
                break;
            }
        }
        
        return sb.ToString();
    }
}