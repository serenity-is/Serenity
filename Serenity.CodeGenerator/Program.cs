using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
    public class Program
    {
        private static string[] skipPackages = new[]
        {
            "Microsoft.",
            "System.",
            "Newtonsoft.",
            "EPPlus",
            "FastMember",
            "MailKit"
        };

        public static void Main(string[] args)
        {
            var projectJson = "project.json";

#if DEBUG
            var testJson = "P:/Sandbox/Serene/Serene/Serene.AspNetCore/project.json";
            if (!File.Exists(projectJson))
            {
                if (File.Exists(testJson))
                    projectJson = testJson;
                else if (File.Exists(testJson.Replace("P:/", "C:/")))
                    projectJson = testJson.Replace("P:/", "C:/");
                else if (File.Exists(testJson.Replace("P:/Sandbox/", "C:/Projects/")))
                    projectJson = testJson.Replace("P:/Sandbox/", "C:/Projects/");
            }
#endif
            if (!File.Exists(projectJson))
            {
                Console.Error.WriteLine("Can't find project.json in current directory!");
                Environment.Exit(1);
            }

            projectJson = Path.GetFullPath(projectJson);

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = Path.GetDirectoryName(projectJson),
                CreateNoWindow = true,
                Arguments = "restore project.json"
            });

            process.WaitForExit();
            if (process.ExitCode > 0)
            {
                Console.Error.WriteLine("Error executing dotnet restore!");
                Environment.Exit(process.ExitCode);
            }

            var packagesFolder = "/packages/".Replace('/', Path.DirectorySeparatorChar);

            if (args.Length > 0 && args[0] == "restore")
            {
                var packagesDir = AppContext.BaseDirectory;
                var packagesIdx = packagesDir.IndexOf(packagesFolder);
#if DEBUG
                if (packagesIdx < 0)
                {
                    string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
                    if (string.IsNullOrEmpty(userHomeDirectory))
                        userHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");

                    packagesDir = Path.Combine(userHomeDirectory, ".nuget/packages/"
                        .Replace('/', Path.DirectorySeparatorChar));

                    packagesIdx = packagesDir.IndexOf(packagesFolder);
                }
#endif

                if (packagesIdx < 0)
                {
                    Console.Error.WriteLine("Can't determine NuGet packages directory!");
                    Environment.Exit(1);
                }

                packagesDir = packagesDir.Substring(0, packagesIdx + packagesFolder.Length);
                if (!Directory.Exists(packagesDir))
                {
                    Console.Error.WriteLine("Can't determine NuGet packages directory!");
                    Environment.Exit(1);
                }

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

                var proj = JObject.Parse(File.ReadAllText(projectJson));
                EnumerateProjectJsonDeps(proj, (fw, id, ver) =>
                {
                    if (!skipPackage(id))
                        queue.Enqueue(new Tuple<string, string, string>(fw, id, ver));
                });

                while (queue.Count > 0)
                {
                    var dep = queue.Dequeue();
                    var id = dep.Item2;

                    var ver = dep.Item3.Trim();
                    if (ver.EndsWith("-*"))
                        ver = ver.Substring(0, ver.Length - 2);
                    else if (ver.StartsWith("[") && ver.EndsWith("]"))
                    {
                        ver = ver.Substring(1, ver.Length - 2).Trim();
                    }

                    var packageFolder = Path.Combine(Path.Combine(packagesDir, id), ver);
                    var nuspecFile = Path.Combine(packageFolder, id + ".nuspec");
                    if (!File.Exists(nuspecFile))
                        continue;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Processing: " + id);
                    Console.ResetColor();

                    var contentRoot = Path.Combine(packageFolder, "content/".Replace('/', Path.DirectorySeparatorChar));
                    if (Directory.Exists(contentRoot))
                    {
                        var targetRoot = Path.GetDirectoryName(projectJson);

                        foreach (var file in Directory.GetFiles(contentRoot, "*.*", SearchOption.AllDirectories))
                        {
                            var extension = Path.GetExtension(file);
                            if (String.Compare(extension, ".transform", StringComparison.OrdinalIgnoreCase) == 0)
                                continue;

                            var relative = file.Substring(contentRoot.Length);
                            if (relative.StartsWith("scripts/typings/".Replace('/', Path.DirectorySeparatorChar),
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                relative = relative.Substring("scripts/".Length);
                            }
                            else
                            {
                                relative = Path.Combine("wwwroot", relative);
                            }

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

                    var nuspec = XElement.Parse(File.ReadAllText(nuspecFile));
                    XNamespace ns = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";
                    var meta = nuspec.Element(ns + "metadata");
                    if (meta == null)
                        continue;

                    var deps = meta.Element(ns + "dependencies");
                    if (deps == null)
                        continue;

                    var fw = dep.Item1;

                    var groups = deps.Elements(ns + "group");
                    if (groups.Any())
                    {
                        foreach (var group in groups)
                        {
                            var target = group.Attribute("targetFramework").Value;
                            if (string.IsNullOrEmpty(target) ||
                                String.Compare(target, fw, StringComparison.OrdinalIgnoreCase) == 0 ||
                                target.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) ||
                                target.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
                                target.StartsWith("netcore", StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (var dep2 in group.Elements(ns + "dependency"))
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
                        foreach (var dep2 in deps.Elements(ns + "dependency"))
                        {
                            var id2 = dep2.Attribute("id").Value;
                            if (!skipPackage(id2))
                                queue.Enqueue(new Tuple<string, string, string>(fw, id2, dep2.Attribute("version").Value));
                        }
                    }
                }
            }
            else if (args.Length == 0)
            {
                Console.Error.WriteLine("Default action...");
                Environment.Exit(1);
            }
            else
            {
                Console.Error.WriteLine("Use one of 'restore', 'generate' as parameter!");
                Environment.Exit(1);
            }
        }

        private static void EnumerateProjectJsonDeps(JObject proj, Action<string, string, string> dependency)
        {
            Action<string, JObject> enumDeps = (fwkey, deps) => {
                if (deps == null)
                    return;

                foreach (var pair in deps)
                {
                    var v = pair.Value as JObject;
                    if (v != null)
                    {
                        var o = v["version"] as JValue;
                        if (o != null && o.Value != null)
                            dependency(fwkey, pair.Key, o.Value.ToString());
                    }
                    else if (pair.Value is JValue && (pair.Value as JValue).Value != null)
                    {
                        dependency(fwkey, pair.Key, (pair.Value as JValue).Value.ToString());
                    }
                }
            };

            var frameworks = proj["frameworks"] as JObject;
            if (frameworks == null)
                return;

            foreach (var pair in frameworks)
            {
                var val = pair.Value as JObject;
                if (val == null)
                    continue;

                enumDeps(pair.Key, val["dependencies"] as JObject);
                enumDeps(pair.Key, proj["dependencies"] as JObject);
            }
        }
    }
}