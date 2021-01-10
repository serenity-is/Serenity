using Newtonsoft.Json.Linq;
using Serenity.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class RestoreCommand
    {
        private static string[] skipPackages = new[]
        {
            "Dapper",
            "EPPlus",
            "FastMember",
            "FluentMigrator.",
            "FirebirdSql.",
            "MailKit",
            "Mapster",
            "MySql",
            "Microsoft.",
            "Newtonsoft.",
            "NetStandard.",
            "Npgsql",
            "Nuglify.",
            "StackExchange.",
            "System.",
            "Serenity.Net."
        };

        public void Run(string csproj)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = Path.GetDirectoryName(csproj),
                CreateNoWindow = true,
                Arguments = "restore \"" + csproj + "\""
            });

            process.WaitForExit();
            if (process.ExitCode > 0)
            {
                Console.Error.WriteLine("Error executing dotnet restore!");
                Environment.Exit(process.ExitCode);
            }

            var packagesDir = new PackageHelper().DeterminePackagesPath();

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<Tuple<string, string, string>>();

            Func<string, bool> skipPackage = id =>
            {
                if (visited.Contains(id))
                    return true;

                foreach (var skip in skipPackages)
                    if (id.StartsWith(skip, StringComparison.OrdinalIgnoreCase))
                        return true;

                visited.Add(id);

                return false;
            };

            var projectDir = Path.GetDirectoryName(csproj);
            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            EnumerateProjectDeps(csproj, (fw, id, ver) =>
            {
                if (!skipPackage(id) && !string.IsNullOrEmpty(ver))
                    queue.Enqueue(new Tuple<string, string, string>(fw, id, ver));
            });

            GlobFilter include = null;
            if (config.Restore?.Include.IsEmptyOrNull() == false)
                include = new GlobFilter(config.Restore.Include);

            GlobFilter exclude = null;
            if (config.Restore?.Exclude.IsEmptyOrNull() == false)
                exclude = new GlobFilter(config.Restore.Exclude);

            while (queue.Count > 0)
            {
                var dep = queue.Dequeue();
                var id = dep.Item2;

                var ver = dep.Item3.Trim();
                if (ver.EndsWith("-*", StringComparison.Ordinal))
                    ver = ver.Substring(0, ver.Length - 2);
                else if (ver.StartsWith("[", StringComparison.Ordinal) && ver.EndsWith("]", StringComparison.Ordinal))
                {
                    ver = ver.Substring(1, ver.Length - 2).Trim();
                }

                var packageFolder = Path.Combine(Path.Combine(packagesDir, id), ver);
                if (!Directory.Exists(packageFolder))
                {
                    packageFolder = Path.Combine(Path.Combine(packagesDir, id.ToLowerInvariant()), ver);
                    if (!Directory.Exists(packageFolder))
                    {
                        var myPackagesDir = Path.Combine(packagesDir, "..", "my-packages");
                        packageFolder = Path.Combine(myPackagesDir, id, ver);
                        if (!Directory.Exists(packageFolder))
                            packageFolder = Path.Combine(myPackagesDir, id.ToLowerInvariant(), ver);
                    }
                }

                var nuspecFile = Path.Combine(packageFolder, id + ".nuspec");
                if (!File.Exists(nuspecFile))
                {
                    nuspecFile = Path.Combine(packageFolder, id.ToLowerInvariant() + ".nuspec");
                    if (!File.Exists(nuspecFile))
                        continue;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Processing: " + id);
                Console.ResetColor();

                var contentRoot = Path.Combine(packageFolder, "Content/".Replace('/', Path.DirectorySeparatorChar));
                if (!Directory.Exists(contentRoot))
                    contentRoot = Path.Combine(packageFolder, "content/".Replace('/', Path.DirectorySeparatorChar));


                if (Directory.Exists(contentRoot))
                {
                    var targetRoot = Path.GetDirectoryName(csproj);

                    foreach (var file in Directory.GetFiles(contentRoot, "*.*", SearchOption.AllDirectories))
                    {
                        var extension = Path.GetExtension(file);
                        if (string.Compare(extension, ".transform", StringComparison.OrdinalIgnoreCase) == 0)
                            continue;

                        var relative = file.Substring(contentRoot.Length);

                        // toastr!
                        if (relative.StartsWith("content/".Replace('/', Path.DirectorySeparatorChar), StringComparison.Ordinal))
                            relative = "Content/".Replace('/', Path.DirectorySeparatorChar) + relative.Substring("content/".Length);
                        else if (relative.StartsWith("scripts/", StringComparison.Ordinal))
                            relative = "Scripts/".Replace('/', Path.DirectorySeparatorChar) + relative.Substring("content/".Length);
                        else if (relative.StartsWith("Fonts/", StringComparison.Ordinal))
                            relative = "fonts/".Replace('/', Path.DirectorySeparatorChar) + relative.Substring("fonts/".Length);

                        if (relative.StartsWith("scripts/typings/".Replace('/', Path.DirectorySeparatorChar),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            relative = relative.Substring("scripts/".Length);
                        }
                        else
                        {
                            relative = Path.Combine("wwwroot", relative);
                        }

                        if (include != null &&
                            !include.IsMatch(relative))
                            continue;

                        if (exclude != null &&
                            exclude.IsMatch(relative))
                            continue;

                        var target = Path.Combine(targetRoot, relative);
                        if (File.Exists(target))
                        {
                            if (!File.ReadAllBytes(target)
                                    .SequenceEqual(File.ReadAllBytes(file)))
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Restoring: " + relative);
                                Console.ResetColor();
                                File.Copy(file, target, true);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(target))
                                Directory.CreateDirectory(Path.GetDirectoryName(target));

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Restoring: " + relative);
                            Console.ResetColor();
                            File.Copy(file, target, false);
                        }
                    }
                }

                var nuspecContent = File.ReadAllText(nuspecFile);
                var nuspec = XElement.Parse(nuspecContent);
                var meta = nuspec.Elements().Where(x => x.Name?.LocalName == "metadata").FirstOrDefault();
                if (meta == null)
                    continue;

                var deps = meta.Elements().Where(x => x.Name?.LocalName == "dependencies").FirstOrDefault(); ;
                if (deps == null)
                    continue;

                var fw = dep.Item1;

                var groups = deps.Elements().Where(x => x.Name?.LocalName == "group");
                if (groups.Any())
                {
                    foreach (var group in groups)
                    {
                        var target = group.Attribute("targetFramework").Value;
                        if (string.IsNullOrEmpty(target) ||
                            string.Compare(target, fw, StringComparison.OrdinalIgnoreCase) == 0 ||
                            target.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("netcore", StringComparison.OrdinalIgnoreCase) ||
                            target.StartsWith("net", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (var dep2 in group.Elements().Where(x => x.Name?.LocalName == "dependency"))
                            {
                                var id2 = dep2.Attribute("id").Value;
                                if (!skipPackage(id2))
                                    queue.Enqueue(new Tuple<string, string, string>(fw, id2, dep2.Attribute("version").Value));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var dep2 in deps.Elements().Where(x => x.Name?.LocalName == "dependency"))
                    {
                        var id2 = dep2.Attribute("id").Value;
                        if (!skipPackage(id2))
                            queue.Enqueue(new Tuple<string, string, string>(fw, id2, dep2.Attribute("version").Value));
                    }
                }
            }
        }

        private static void EnumerateProjectDeps(string csproj, Action<string, string, string> dependency)
        {
            var csprojElement = XElement.Parse(File.ReadAllText(csproj));

            foreach (var itemGroup in csprojElement.Descendants("ItemGroup"))
            {
                var condition = itemGroup.Attribute("Condition");
                var target = "";
                if (condition != null && !string.IsNullOrEmpty(condition.Value))
                {
                    const string tf = "'$(TargetFramework)' == '";
                    var idx = condition.Value.IndexOf(tf, StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        var end = condition.Value.IndexOf("'", idx + tf.Length, StringComparison.Ordinal);
                        if (end >= 0)
                            target = condition.Value.Substring(idx + tf.Length, end - idx - tf.Length);
                    }
                }

                foreach (var packageReference in itemGroup.Descendants("PackageReference"))
                {
                    var ver = packageReference.Attribute("Version")?.Value?.Trim();
                    if (!string.IsNullOrEmpty(ver) &&
                        !Version.TryParse(ver, out _) &&
                        ver.StartsWith("$(", StringComparison.Ordinal) &&
                        ver.EndsWith(")", StringComparison.Ordinal))
                    {
                        var prop = ProjectFileHelper.ExtractPropertyFrom(csproj, xel => 
                            xel.Descendants(ver.Substring(2, ver.Length - 3))?
                                .LastOrDefault(x => Version.TryParse(x.Value?.TrimToEmpty(), out _))
                                .Value?.TrimToNull());
                        if (prop != null)
                            ver = prop;
                    }

                    dependency(target, packageReference.Attribute("Include").Value, ver);
                }
            }

            if (!Path.GetFileName(csproj).Equals("Directory.Build.props", StringComparison.OrdinalIgnoreCase))
            { 
                var dir = Path.GetDirectoryName(Path.GetFullPath(csproj));
                while (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                {
                    var dirProps = Path.Combine(dir, "Directory.Build.props");
                    if (File.Exists(dirProps))
                        EnumerateProjectDeps(dirProps, dependency);
                    dir = Path.GetDirectoryName(dir);
                }
            }
        }
    }
}