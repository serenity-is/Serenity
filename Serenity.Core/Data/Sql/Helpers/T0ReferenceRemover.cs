using System;
using System.Text;

namespace Serenity.Data
{
    public class T0ReferenceRemover
    {
        public static string RemoveT0Aliases(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var sb = new StringBuilder();

            bool inQuote = false;
            int startIdent = -1;
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                sb.Append(c);

                if (inQuote)
                {
                    if (c == '\'')
                    {
                        inQuote = false;
                        continue;
                    }
                }
                else
                {
                    if (c == '\'')
                    {
                        inQuote = true;
                        startIdent = -1;
                    }
                    else if (c == '_' || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    {
                        if (startIdent < 0)
                            startIdent = i;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                    }
                    else if (c == '.')
                    {
                        if (startIdent >= 0 && 
                            startIdent < i &&
                            i - startIdent == 2 &&
                            expression[startIdent + 1] == '0' &&
                            Char.ToLowerInvariant(expression[startIdent]) == 't')
                        {
                            sb.Length -= 3;
                        }

                        startIdent = -1;
                    }
                    else
                        startIdent = -1;
                }
            }

            return sb.ToString();
        }
    }
}