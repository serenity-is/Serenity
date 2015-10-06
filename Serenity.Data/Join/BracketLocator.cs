using System;
using System.Collections.Generic;
using System.Text;

namespace Serenity.Data
{
    public class BracketLocator
    {
        public static string ReplaceBracketContents(string expression, char validChar1, Func<string, string> replace)
        {
            if (expression == null)
                return null;

            bool inQuote = false;
            int startBracket = -1;
            var sb = new StringBuilder(expression.Length);
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                sb.Append(c);

                if (inQuote)
                {
                    if (c == '\'')
                        inQuote = false;
                }
                else if (c == '\'')
                {
                    inQuote = true;
                    startBracket = -1;
                }
                else if (c == '[')
                {
                    startBracket = i;
                }
                else if (c == ']')
                {
                    if (startBracket >= 0 && startBracket < i - 1)
                    {
                        var contents = expression.Substring(startBracket + 1, i - startBracket - 1);
                        var replaced = replace(contents);
                        if (contents != replaced)
                        {
                            sb.Length -= contents.Length + 1;
                            sb.Append(replaced);
                            sb.Append("]");
                        }
                    }
                    else
                        startBracket = -1;
                }
                else if (c == '_' || c == validChar1 || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                }
                else if (startBracket > 0)
                {
                    startBracket = -1;
                }
            }

            return sb.ToString();
        }
    }
}