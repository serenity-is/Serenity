using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class Program
    {
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

            var sergenJson = "sergen.json";

            if (!File.Exists(sergenJson))
            {
                System.Console.Error.WriteLine("Can't find sergen.json in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                Environment.Exit(1);
            }

            sergenJson = Path.GetFullPath(sergenJson);
            var projectDir = Path.GetDirectoryName(sergenJson);

            if (!Directory.Exists(Path.Combine(projectDir, "wwwroot")))
            {
                System.Console.Error.WriteLine("Can't find wwwroot folder in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                Environment.Exit(1);
            }

            var csprojs = Directory.GetFiles(projectDir, "*.csproj");
            if (csprojs.Length > 1)
                csprojs = csprojs.Where(x => !x.StartsWith("Dev.", StringComparison.OrdinalIgnoreCase)).ToArray();

            if (csprojs.Length == 0)
            {
                System.Console.Error.WriteLine("Can't find a project file in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                Environment.Exit(1);
            }

            if (csprojs.Length > 1)
            {
                System.Console.Error.WriteLine("Multiple project files found in current directory!");
                System.Console.Error.WriteLine("Please run Sergen in a folder that contains only one Asp.Net Core project.");
                Environment.Exit(1);
            }

            var csproj = csprojs.First();

            if ("restore".StartsWith(command))
            {
                new RestoreCommand().Run(csproj);
            }
            else if (
                "transform".StartsWith(command) ||
                "servertypings".StartsWith(command) ||
                "clienttypes".StartsWith(command) ||
                "mvct".StartsWith(command))
            {
                string tsTypesJson = null;
                Func<List<ExternalType>> getTsTypes = () =>
                {
                    if (tsTypesJson == null)
                    {
                        var tsTypeLister = new TSTypeLister(projectDir);
                        var tsTypes = tsTypeLister.List();
                        tsTypesJson = JSON.Stringify(tsTypes);
                    }

                    return JSON.Parse<List<ExternalType>>(tsTypesJson);
                };

                if ("transform".StartsWith(command) || "mvct".StartsWith(command))
                {
                    new MvcCommand().Run(csproj);
                }

                if ("transform".StartsWith(command) || "clienttypes".StartsWith(command) || command == "mvct")
                {
                    new ClientTypesCommand().Run(csproj, getTsTypes());
                }

                if ("transform".StartsWith(command) || "servertypings".StartsWith(command))
                {
                    new ServerTypingsCommand().Run(csproj, getTsTypes());
                }
            }
            else if ("generate".StartsWith(command))
            {
                new GenerateCommand().Run(csproj, args.Skip(1).ToArray());
            }
            else
            {
                WriteHelp();
            }
        }

        private static void WriteHelp()
        {
            Console.WriteLine("Serenity Code Generator " +
#if COREFX
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
#else
                typeof(Program).GetType().Assembly.GetName().Version.ToString());
#endif
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