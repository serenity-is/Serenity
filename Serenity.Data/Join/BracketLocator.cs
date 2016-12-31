using System;
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

        public static string ReplaceBrackets(string expression, ISqlDialect dialect)
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
                    if (c == '\'')
                        inQuote = false;

                    sb.Append(c);
                }
                else if (c == '\'')
                {
                    inQuote = true;
                    sb.Append(c);
                }
                else if (c == '[')
                {
                    if (i > 0 &&
                        (Char.IsLetterOrDigit(expression[i - 1]) ||
                         expression[i - 1] == '_'))
                    {
                        // might be an array indexer expression like a[5]
                        sb.Append(c);
                        continue;
                    }

                    var end = expression.IndexOf(']', i + 1);
                    if (end < 0)
                    {
                        sb.Append(c);
                        continue;
                    }

                    if (end < expression.Length - 1 &&
                        (expression[end + 1] == '_' ||
                         Char.IsLetterOrDigit(expression[end + 1])))
                    {
                        sb.Append(c);
                        continue;
                    }

                    long l;
                    var sub = expression.SafeSubstring(i + 1, end - i - 1);
                    if (sub.Length == 0 ||
                        sub.IndexOf('\'') >= 0 ||
                        sub.IndexOf('[') >= 0 ||
                        Int64.TryParse(sub, out l))
                    {
                        sb.Append(c);
                        continue;
                    }

                    sb.Append(dialect.QuoteIdentifier(sub));
                    i = end;
                    continue;
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}