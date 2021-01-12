using Microsoft.Build.Locator;
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

            string csproj = null;
            if (command == "-p" && args.Length > 2)
            {
                csproj = args[1];
                command = args.Length > 2 ? args[1] : null;
            }

            if (command.IsEmptyOrNull() ||
                command == "-?" ||
                command == "--help")
            {
                WriteHelp();
                Environment.Exit(1);
            }

            if (csproj == null)
            {
                var csprojs = Directory.GetFiles(".", "*.csproj");
                if (csprojs.Length == 0)
                {
                    Console.Error.WriteLine("Can't find a project file in current directory!");
                    Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                    Environment.Exit(1);
                }

                if (csprojs.Length > 1)
                {
                    Console.Error.WriteLine("Multiple project files found in current directory!");
                    Console.Error.WriteLine("Please run Sergen in a folder that contains only one Asp.Net Core project.");
                    Environment.Exit(1);
                }

                csproj = csprojs[0];
            }

            if (!File.Exists(csproj))
            {
                Console.Error.WriteLine("Can't find project named: " + csproj);
                Environment.Exit(1);
            }

            var projectDir = Path.GetFullPath(Path.GetDirectoryName(csproj));

            if ("restore".StartsWith(command, StringComparison.Ordinal))
            {
                MSBuildLocator.RegisterDefaults();
                new RestoreCommand().Run(csproj);
            }
            else if (
                "transform".StartsWith(command, StringComparison.Ordinal) ||
                "servertypings".StartsWith(command, StringComparison.Ordinal) ||
                "clienttypes".StartsWith(command, StringComparison.Ordinal) ||
                "mvct".StartsWith(command, StringComparison.Ordinal))
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

                if ("transform".StartsWith(command, StringComparison.Ordinal) || 
                    "mvct".StartsWith(command, StringComparison.Ordinal))
                {
                    new MvcCommand().Run(csproj);
                }

                if ("transform".StartsWith(command, StringComparison.Ordinal) || 
                    "clienttypes".StartsWith(command, StringComparison.Ordinal) || command == "mvct")
                {
                    new ClientTypesCommand().Run(csproj, getTsTypes());
                }

                if ("transform".StartsWith(command, StringComparison.Ordinal) || 
                    "servertypings".StartsWith(command, StringComparison.Ordinal))
                {
                    new ServerTypingsCommand().Run(csproj, getTsTypes());
                }
            }
            else if ("generate".StartsWith(command, StringComparison.Ordinal))
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
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version);
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