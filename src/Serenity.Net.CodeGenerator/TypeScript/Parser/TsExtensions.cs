namespace Serenity.TypeScript.TsParser;

public static class TsExtensions
{
    public static string[] Exec(Regex r, string text)
    {
        var result = new List<string>();
        foreach (var x in r.Match(text).Captures)
            if (x is Capture c)
                result.Add(c.Value);
        return [.. result];
    } 

    public static string Slice(string str, int start, int end = int.MaxValue)
    {
        if (start < 0)
            start += str.Length;
        if (end < 0)
            end += str.Length;

        start = Math.Min(Math.Max(start, 0), str.Length);
        end = Math.Min(Math.Max(end, 0), str.Length);
        if (end <= start)
            return string.Empty;

        return str[start..end];
    }

}
