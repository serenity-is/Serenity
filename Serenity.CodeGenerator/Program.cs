using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            string command = null;
            if (args.Length > 0)
                command = args[0].ToLowerInvariant().TrimToEmpty();

            if (command.IsEmptyOrNull() ||
                command == "-?" ||
                command == "--help")
            {
                WriteHelp();
                Environment.Exit(1);
            }

            var projectJson = "project.json";

            if (!File.Exists(projectJson))
            {
                System.Console.Error.WriteLine("Can't find project.json in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                Environment.Exit(1);
            }

            projectJson = Path.GetFullPath(projectJson);

            if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(projectJson), "wwwroot")))
            {
                System.Console.Error.WriteLine("Can't find wwwroot folder in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                Environment.Exit(1);
            }

            if ("restore".StartsWith(command))
            {
                new RestoreCommand().Run(projectJson);
            }
            else if (
                "transform".StartsWith(command) ||
                "servertypings".StartsWith(command) ||
                "clienttypes".StartsWith(command) ||
                "mvc".StartsWith(command))
            {
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

                if ("transform".StartsWith(command) || "mvc".StartsWith("command"))
                {
                    new MvcCommand().Run(projectJson);
                }

                if ("transform".StartsWith(command) || "clienttypes".StartsWith(command))
                {
                    new ClientTypesCommand().Run(projectJson, getTsTypes());
                }

                if ("transform".StartsWith(command) || "servertypings".StartsWith(command))
                {
                    new ServerTypingsCommand().Run(projectJson, getTsTypes());
                }
            }
            else if ("generate".StartsWith(command))
            {
                new GenerateCommand().Run(projectJson, args.Skip(1).ToArray());
            }
            else
            {
                WriteHelp();
            }
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Serenity Code Generator " +
                Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion);

            Console.WriteLine();
            Console.WriteLine("Usage: sergen [command]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("    g[enerate]        Launches table code generator");
            Console.WriteLine("    r[estore]         Restores content, e.g. scripts, fonts and css from .nupkg");
            Console.WriteLine();
            Console.WriteLine("    c[lienttypes]     Imports editor, formatter types from TypeScript to CS");
            Console.WriteLine("    m[vc]             Generates intellisense helpers for view locations");
            Console.WriteLine("    s[ervertypings]   Imports row, form, service types from CS to TypeScript");
            Console.WriteLine("    t[ransform]       Runs clienttypes, mvc and servertypings commands at once");
            Environment.Exit(1);
        }
    }
}