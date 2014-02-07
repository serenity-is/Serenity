using Serenity.Testing;
using System.Text;

namespace Serenity.Testing.Test
{
    public class TestSqlHelper
    {
        public static string Normalize(string script)
        {
            if (script == null)
                return null;

            script = script.Trim();

            StringBuilder sb = new StringBuilder();
            bool insideQuote = false;
            char prior = '\0';
            foreach (var x in script)
            {
                char c = x;
                if (c == '\'')
                    insideQuote = !insideQuote;
                else if (insideQuote)
                {
                }
                else if (c == '\r')
                    continue;
                else if (c == '\n' || c == ' ')
                {
                    if (prior == '\n' || prior == ' ' || prior == ';')
                        continue;

                    c = ' ';
                }

                prior = c;
                sb.Append(c);
            }

            return sb.ToString().Trim();
        }
    }
}