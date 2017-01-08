using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

                string tsTypesJson = null;
                Func<List<ExternalType>> getTsTypes = () =>
                {
                    if (tsTypesJson == null)
                    {
                        var tsTypeLister = new TSTypeLister(Path.GetDirectoryName(projectJson));
                        var tsTypes = tsTypeLister.List();
                        tsTypesJson = JSON.Stringify(tsTypes);
                    }

                    return JSON.Parse<List<ExternalType>>(tsTypesJson);
                };

                if (type == null || type == "clienttypes" || type == "ct")
                {
                    new ClientTypesCommand().Run(projectJson, getTsTypes());
                }

                if (type == null || type == "servertypings" || type == "st")
                {
                    new ServerTypingsCommand().Run(projectJson, getTsTypes());
                }
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
    }
}