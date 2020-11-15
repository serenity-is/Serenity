using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class MvcCommand
    {
        private static Encoding utf8 = new System.Text.UTF8Encoding(true);

        public void Run(string csproj)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            config.MVC = config.MVC ?? new GeneratorConfig.MVCConfig();

            var outDir = Path.Combine(projectDir, (config.MVC.OutDir.TrimToNull() ?? "Imports/MVC")
                .Replace('/', Path.DirectorySeparatorChar));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Transforming MVC at: ");
            Console.ResetColor();
            Console.WriteLine(outDir);

            string[] stripViewPaths = config.MVC.StripViewPaths ?? new string[] {
                "Modules/",
                "Views/"
            };

            var rootDir = projectDir + Path.DirectorySeparatorChar;
            var searchViewPaths = (config.MVC.SearchViewPaths ?? new string[] { "Modules/", "Views/" })
                .Select(x => Path.Combine(rootDir, x.Replace('/', Path.DirectorySeparatorChar)));

            IEnumerable<string> files = new List<string>();
            foreach (var path in searchViewPaths)
                files = files.Concat(Directory.GetFiles(path, "*.cshtml", SearchOption.AllDirectories));

            Func<string, string> getName = s => {
                var path = s.Substring(rootDir.Length);
                var name = Path.ChangeExtension(path, null).Replace('\\', '/');
                foreach (var strip in stripViewPaths)
                {
                    if (name.StartsWith(strip, StringComparison.OrdinalIgnoreCase))
                    {
                        name = name.Substring(strip.Length);
                    }
                }

                return name;
            };

            files = files.OrderBy(x => getName(x));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("");
            sb.AppendLine("namespace MVC");
            sb.AppendLine("{");
            sb.AppendLine("    public static class Views");
            sb.AppendLine("    {");

            var last = new string[0];
            var processed = new HashSet<string>();

            foreach (var file in files)
            {
                var path = file.Substring(rootDir.Length);
                var name = getName(file);
                if (name.StartsWith("App_Code/", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith("_ViewStart", StringComparison.OrdinalIgnoreCase) ||
                    name.EndsWith("_ViewImports", StringComparison.OrdinalIgnoreCase) ||
                    processed.Contains(name))
                    continue;

                processed.Add(name);
                var parts = name.Split(new char[] { '/' });

                if (parts.Length <= 1)
                    continue;

                for (var i = last.Length; i > parts.Length; i--)
                {
                    var close = (new String(' ', (i * 4)) + "}");
                    sb.AppendLine(close);
                }

                var x = Math.Min(last.Length, parts.Length) - 2;
                while (x >= 0 && last[x] != parts[x])
                {
                    var close = (new String(' ', ((x + 2) * 4)) + "}");
                    x--;
                    sb.AppendLine(close);
                    sb.AppendLine();
                }

                for (var i = Math.Max(x + 1, 0); i < parts.Length - 1; i++)
                {
                    var indent = new String(' ', (i + 2) * 4);
                    var u = parts[i];
                    if (i > 0 && parts[i - 1] == u)
                        u = u + "_";
                    sb.AppendLine(indent + "public static class " + u);
                    sb.AppendLine(indent + "{");
                }

                var n = parts[parts.Length - 1].Replace(".", "_");
                if (parts.Length - 1 > 0 && parts[parts.Length - 2] == n)
                    n += "_";

                sb.Append(new String(' ', (parts.Length + 1) * 4));
                sb.AppendLine("public const string " + n + " = \"~/" + path.Replace(@"\", "/") + "\";");

                last = parts;
            }

            for (var i = last.Length - 1; i > 0; i--)
            {
                sb.AppendLine(new String(' ', ((i + 1) * 4)) + "}" + Environment.NewLine);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            new MultipleOutputHelper().WriteFiles(outDir, new SortedDictionary<string, string>
            {
                { "MVC.cs", sb.ToString() }
            });
        }
    }
}