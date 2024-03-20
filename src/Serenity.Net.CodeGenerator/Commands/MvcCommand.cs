namespace Serenity.CodeGenerator;

public class MvcCommand(IProjectFileInfo project, IGeneratorConsole console) 
    : BaseGeneratorCommand(project, console)
{
    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(FileSystem.GetFullPath(ProjectFile));
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        config.MVC ??= new();

        var transformFor = FileSystem.GetFileNameWithoutExtension(ProjectFile);
        Console.WriteLine($"Transforming MVC for {transformFor}", ConsoleColor.Cyan);

        string[] stripViewPaths = config.MVC.StripViewPaths ?? [
            "Modules/",
            "Views/",
            FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/",
            "Areas/" + FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/"
        ];

        var rootDir = projectDir + System.IO.Path.DirectorySeparatorChar;
        var searchViewPaths = (config.MVC.SearchViewPaths ??
            [
                "Modules/",
                "Views/",
                FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/",
                "Areas/" + FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/"
            ])
            .Select(x => FileSystem.Combine(rootDir, PathHelper.ToPath(x)));

        IEnumerable<string> files = [];
        foreach (var path in searchViewPaths)
        {
            if (FileSystem.DirectoryExists(path))
                files = files.Concat(FileSystem.GetFiles(path, "*.cshtml", recursive: true));
        }

        string getName(string s)
        {
            var path = s[rootDir.Length..];
            var name = FileSystem.ChangeExtension(path, null).Replace('\\', '/');
            foreach (var strip in stripViewPaths)
            {
                if (name.StartsWith(strip, StringComparison.OrdinalIgnoreCase))
                {
                    name = name[strip.Length..];

                    break;
                }
            }

            return name;
        }

        files = files.OrderBy(getName);

        var cw = new CodeWriter()
        {
            FileScopedNamespaces = config.FileScopedNamespaces == true,
            IsCSharp = true
        };
        cw.AppendLine("");
        var ns = (config.MVC.UseRootNamespace == true ||
            (config.MVC.UseRootNamespace == null &&
             FileSystem.ReadAllText(ProjectFile).Contains("Sdk=\"Microsoft.NET.Sdk.Razor\"", StringComparison.CurrentCulture))) ?
             config.GetRootNamespaceFor(Project) + ".MVC" : "MVC";

        cw.InNamespace(ns, () =>
        {
            cw.IndentedLine("public static class Views");
            cw.InBrace(() =>
            {
                var last = Array.Empty<string>();
                var processed = new HashSet<string>();

                foreach (var file in files)
                {
                    var path = file[rootDir.Length..];
                    var name = getName(file);
                    if (name.StartsWith("App_Code/", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith("_ViewStart", StringComparison.OrdinalIgnoreCase) ||
                        name.EndsWith("_ViewImports", StringComparison.OrdinalIgnoreCase) ||
                        processed.Contains(name))
                        continue;

                    processed.Add(name);
                    var parts = name.Split(['/']);

                    if (parts.Length == 0)
                        continue;

                    for (var i = last.Length; i > parts.Length; i--)
                    {
                        var close = (new string(' ', (i - 2) * 4) + "}");
                        cw.IndentedLine(close);
                    }

                    var x = Math.Min(last.Length, parts.Length) - 2;
                    while (x >= 0 && last[x] != parts[x])
                    {
                        var close = (new string(' ', x * 4) + "}");
                        x--;
                        cw.IndentedLine(close);
                        cw.AppendLine();
                    }

                    for (var i = Math.Max(x + 1, 0); i < parts.Length - 1; i++)
                    {
                        var indent = new string(' ', i * 4);
                        var u = parts[i];
                        if (i > 0 && parts[i - 1] == u ||
                            i == 0 && parts[i] == "Views")
                            u += "_";
                        cw.IndentedLine(indent + "public static class " + u.Replace(".", "_"));
                        cw.IndentedLine(indent + "{");
                    }

                    var n = parts[^1].Replace(".", "_", StringComparison.Ordinal);
                    if (parts.Length - 1 > 0 && parts[^2] == n)
                        n += "_";

                    cw.Indented(new string(' ', (parts.Length - 1) * 4));
                    cw.AppendLine("public const string " + n + " = \"~/" + path.Replace(@"\", "/", StringComparison.Ordinal) + "\";");

                    last = parts;
                }

                for (var i = last.Length - 1; i > 0; i--)
                {
                    cw.IndentedLine(new string(' ', (i - 1) * 4) + "}" + Environment.NewLine);
                }

                TrimEnd(cw.Builder);
                cw.AppendLine();
            });

        });

        var outDir = FileSystem.Combine(projectDir, 
            PathHelper.ToPath(config.MVC.OutDir.TrimToNull() ?? "Imports/MVC"));
        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir, new[]
        {
            ("MVC.cs", cw.ToString())
        }, deleteExtraPattern: null, endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }

    private static StringBuilder TrimEnd(StringBuilder sb)
    {
        if (sb == null || sb.Length == 0) return sb;

        int i = sb.Length - 1;

        for (; i >= 0; i--)
            if (!char.IsWhiteSpace(sb[i]))
                break;

        if (i < sb.Length - 1)
            sb.Length = i + 1;

        return sb;
    }
}