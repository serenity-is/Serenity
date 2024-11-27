namespace Serenity.Tests;

public static class Normalize
{
    public static string NormalizeSql(this string script)
    {
        return Sql(script);
    }

    public static string Sql(string script)
    {
        if (script == null)
            return null;

        script = script.Trim();

        StringBuilder sb = new();
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