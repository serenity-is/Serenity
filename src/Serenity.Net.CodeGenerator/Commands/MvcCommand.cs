using System.IO;

namespace Serenity.CodeGenerator
{
    public class MvcCommand
    {
        public static void Run(string csproj)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            config.MVC ??= new GeneratorConfig.MVCConfig();

            var outDir = Path.Combine(projectDir, PathHelper.ToPath(config.MVC.OutDir.TrimToNull() ?? "Imports/MVC"));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming MVC at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            string[] stripViewPaths = config.MVC.StripViewPaths ?? new string[] {
                "Modules/",
                "Views/",
                Path.GetFileNameWithoutExtension(csproj) + "/"
            };

            var rootDir = projectDir + Path.DirectorySeparatorChar;
            var searchViewPaths = (config.MVC.SearchViewPaths ?? 
                new string[] { 
                    "Modules/", 
                    "Views/",
                    Path.GetFileNameWithoutExtension(csproj) + "/"
                })
                .Select(x => Path.Combine(rootDir, PathHelper.ToPath(x)));

            IEnumerable<string> files = new List<string>();
            foreach (var path in searchViewPaths)
            {
                if (Directory.Exists(path))
                    files = files.Concat(Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories));
            }

            string getName(string s)
            {
                var path = s[rootDir.Length..];
                var name = Path.ChangeExtension(path, null).Replace('\\', '/');
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

            files = files.OrderBy(x => getName(x));

            StringBuilder sb = new();
            sb.AppendLine("");
            sb.Append("namespace ");
            if (config.MVC.UseRootNamespace == true ||
                (config.MVC.UseRootNamespace == null &&
                 File.ReadAllText(csproj).Contains("Sdk=\"Microsoft.NET.Sdk.Razor\"", StringComparison.CurrentCulture)))
            {
                sb.Append(config.GetRootNamespaceFor(csproj));
                sb.Append('.');
            }
            sb.AppendLine("MVC");
            sb.AppendLine("{");
            sb.AppendLine("    public static class Views");
            sb.AppendLine("    {");

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
                var parts = name.Split(new char[] { '/' });

                if (parts.Length == 0)
                    continue;

                for (var i = last.Length; i > parts.Length; i--)
                {
                    var close = (new string(' ', (i * 4)) + "}");
                    sb.AppendLine(close);
                }

                var x = Math.Min(last.Length, parts.Length) - 2;
                while (x >= 0 && last[x] != parts[x])
                {
                    var close = (new string(' ', ((x + 2) * 4)) + "}");
                    x--;
                    sb.AppendLine(close);
                    sb.AppendLine();
                }

                for (var i = Math.Max(x + 1, 0); i < parts.Length - 1; i++)
                {
                    var indent = new string(' ', (i + 2) * 4);
                    var u = parts[i];
                    if (i > 0 && parts[i - 1] == u ||
                        i == 0 && parts[i] == "Views")
                        u += "_";
                    sb.AppendLine(indent + "public static class " + u.Replace(".", "_"));
                    sb.AppendLine(indent + "{");
                }

                var n = parts[^1].Replace(".", "_", StringComparison.Ordinal);
                if (parts.Length - 1 > 0 && parts[^2] == n)
                    n += "_";

                sb.Append(new string(' ', (parts.Length + 1) * 4));
                sb.AppendLine("public const string " + n + " = \"~/" + path.Replace(@"\", "/", StringComparison.Ordinal) + "\";");

                last = parts;
            }

            for (var i = last.Length - 1; i > 0; i--)
            {
                sb.AppendLine(new string(' ', ((i + 1) * 4)) + "}" + Environment.NewLine);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            MultipleOutputHelper.WriteFiles(outDir, new SortedDictionary<string, string>
            {
                { "MVC.cs", sb.ToString() }
            });
        }
    }
}