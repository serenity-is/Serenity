using System.Text;

namespace Serenity.Data
{
    public class ParamPrefixReplacer
    {
        public static string Replace(string expression, char paramPrefix)
        {
            if (expression == null)
                return null;

            bool inQuote = false;
            var sb = new StringBuilder(expression.Length);
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                
                if (inQuote)
                {
                    sb.Append(c);
                    if (c == '\'')
                        inQuote = false;
                }
                else if (c == '\'')
                {
                    sb.Append(c);
                    inQuote = true;
                }
                else if (c == '@')
                {
                    sb.Append(paramPrefix);
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}