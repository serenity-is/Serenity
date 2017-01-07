using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Loader;

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

            if (!File.Exists(projectJson))
            {
                System.Console.Error.WriteLine("Can't find project.json in current directory!");
                Environment.Exit(1);
            }

            projectJson = Path.GetFullPath(projectJson);

            if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(projectJson), "wwwroot")))
                throw new Exception("Can't locate wwwroot folder!");

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

            if (args.Length > 0 && (args[0] == "restore"))
            {
                new RestoreCommand().Run(projectJson);
            }
            else if (args.Length > 0 && (args[0] == "transform" || args[0] == "t"))
            {
                var type = args.Length < 2 ? null : args[1].ToLowerInvariant();
                if (type != null && 
                    type != "servertypings" && 
                    type != "st" &&
                    type != "clienttypes" && 
                    type != "ct" &&
                    type != "mvc")
                {
                    Console.Error.WriteLine("Invalid transform type: " + type + "!");
                    Environment.Exit(process.ExitCode);
                }

                if (type == null || type == "servertypings" || type == "st")
                    new ServerTypingsCommand().Run(projectJson);
            }
            else if (args.Length == 0)
            {
                System.Console.Error.WriteLine("Default action...");
                Environment.Exit(1);
            }
            else
            {
                System.Console.Error.WriteLine("Use one of 'restore', 'transform' as parameter!");
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