namespace Serenity.Testing;

public static class StringExtensions
{
    private static readonly string[] pluralizeIES = ["y", "ay", "ey", "iy", "oy", "uy"];
    private static readonly string[] pluralizeES = ["us", "ss", "x", "ch", "sh"];

    public static string SimplePluralize(this string text)
    {
        if (pluralizeIES.Any(x => text.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
            return text[..^1] + "ies";

        if (pluralizeES.Any(x => text.EndsWith(x, StringComparison.OrdinalIgnoreCase)))
            return text + "es";

        if (text.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return text;

        if (text.EndsWith("f", StringComparison.InvariantCultureIgnoreCase) && text.Length > 1)
            return text[..^1] + "ves";

        if (text.EndsWith("fe", StringComparison.InvariantCultureIgnoreCase) && text.Length > 2)
            return text[..^2] + "ves";

        return text + "s";
    }
}