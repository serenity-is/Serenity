namespace Serenity.CodeGeneration;

public class ViewPathsGenerator(IFileSystem fileSystem,
    string[] stripViewPaths)
{
    string GetStrippedName(string s)
    {
        var name = fileSystem.ChangeExtension(s, null).Replace('\\', '/');
        foreach (var strip in stripViewPaths)
        {
            if (name.StartsWith(strip, StringComparison.OrdinalIgnoreCase))
                return name[strip.Length..];
        }

        return name;
    }

    static string NormalizeParts(string s)
    {
        foreach (var character in s)
        {
            if (!char.IsLetterOrDigit(character) && character != '_')
                s = s.Replace(character, '_');
        }

        if (!char.IsLetter(s[0]) && s[0] != '_')
            s = $"_{s}";

        return s;
    }


    public void GenerateViews(CodeWriter cw, IEnumerable<string> files)
    {
        files = [.. files.OrderBy(GetStrippedName)];

        cw.IndentedLine("public static partial class Views");
        cw.InBrace(() =>
        {
            var last = Array.Empty<string>();
            var processed = new HashSet<string>();

            foreach (var file in files)
            {
                var name = GetStrippedName(file);
                if (name.StartsWith("App_Code/", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith("_ViewStart", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith("_ViewImports", StringComparison.OrdinalIgnoreCase) ||
                    processed.Contains(name))
                    continue;

                processed.Add(name);
                var parts = name.Split(['/']);

                if (parts.Length == 0)
                    continue;

                for (var i = 0; i < parts.Length; i++)
                {
                    parts[i] = NormalizeParts(parts[i]);
                    if (i > 0 && parts[i - 1] == parts[i])
                        parts[i] += "_";
                }

                bool needLine = false;

                for (var i = last.Length; i > parts.Length; i--)
                {
                    var close = (i > 2 ? new string(' ', (i - 2) * 4) : "") + "}";
                    cw.IndentedLine(close);
                    needLine = true;
                }

                var x = Math.Min(last.Length, parts.Length) - 2;

                for (var i = 0; i <= x; i++)
                {
                    if (last[i] == parts[i])
                        continue;

                    while (x >= i)
                    {
                        x--;
                        var close = new string(' ', (x + 1) * 4) + "}";
                        cw.IndentedLine(close);
                        needLine = true;
                    }

                    break;
                }

                for (var i = Math.Max(x + 1, 0); i < parts.Length - 1; i++)
                {
                    if (needLine)
                    {
                        cw.AppendLine();
                        needLine = false;
                    }

                    var indent = new string(' ', i * 4);
                    var u = parts[i];
                    cw.IndentedLine(indent + "public static partial class " + u.Replace(".", "_"));
                    cw.IndentedLine(indent + "{");
                }

                var n = parts[^1];

                if (needLine)
                {
                    cw.AppendLine();
                    needLine = false;
                }

                cw.Indented(parts.Length > 1 ? new string(' ', (parts.Length - 1) * 4) : "");
                cw.AppendLine("public const string " + n + " = \"~/" +
                    file.Replace('\\', '/') + "\";");

                last = parts;
            }

            for (var i = last.Length - 1; i > 0; i--)
            {
                cw.IndentedLine(i > 1 ? new string(' ', (i - 1) * 4) + "}" : "}");
            }
        });
    }
}