using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var exitCode = Run(args, new FileSystem(), () => new MSBuild.MSBuildProjectSystem());
            if (exitCode != ExitCodes.Success &&
                exitCode != ExitCodes.Help)
                Environment.Exit((int)exitCode);
        }

        public static ExitCodes Run(string[] args, IFileSystem fileSystem, 
            Func<IBuildProjectSystem> projectSystemFactory)
        {
            if (fileSystem is null)
                throw new ArgumentNullException(nameof(fileSystem));

            string command = null;
            if (args.Length > 0)
                command = args[0].ToLowerInvariant().TrimToEmpty();

            string csproj = null;
            if (command == "-p" && args.Length > 2)
            {
                csproj = args[1];
                command = args.Length > 2 ? args[1] : null;
            }

            if (command.IsEmptyOrNull())
            {
                WriteHelp();
                return ExitCodes.NoCommand;
            }

            if (command == "-?" ||
                command == "--help")
            {
                WriteHelp();
                return ExitCodes.Help;
            }

            if (csproj == null)
            {
                var csprojs = fileSystem.Directory.GetFiles(".", "*.csproj");
                if (csprojs.Length == 0)
                {
                    Console.Error.WriteLine("Can't find a project file in current directory!");
                    Console.Error.WriteLine("Please run Sergen in a folder that contains the Asp.Net Core project.");
                    return ExitCodes.NoProjectFiles;
                }

                if (csprojs.Length > 1)
                {
                    Console.Error.WriteLine("Multiple project files found in current directory!");
                    Console.Error.WriteLine("Please run Sergen in a folder that contains only one Asp.Net Core project.");
                    return ExitCodes.MultipleProjectFiles;
                }

                csproj = csprojs[0];
            }

            if (!fileSystem.File.Exists(csproj))
                return ExitCodes.ProjectNotFound;

            var projectDir = fileSystem.Path.GetFullPath(fileSystem.Path.GetDirectoryName(csproj));

            try
            {
                if ("restore".StartsWith(command, StringComparison.Ordinal))
                    return new RestoreCommand(fileSystem, projectSystemFactory()).Run(csproj);

                if ("transform".StartsWith(command, StringComparison.Ordinal) ||
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

                    bool transformAll = "transform".StartsWith(command, StringComparison.Ordinal);

                    if (transformAll ||
                        "mvct".StartsWith(command, StringComparison.Ordinal))
                    {
                        new MvcCommand().Run(csproj);
                    }

                    if (transformAll ||
                        "clienttypes".StartsWith(command, StringComparison.Ordinal) || command == "mvct")
                    {
                        new ClientTypesCommand().Run(csproj, getTsTypes());
                    }

                    if (transformAll ||
                        "servertypings".StartsWith(command, StringComparison.Ordinal))
                    {
                        new ServerTypingsCommand().Run(csproj, getTsTypes());
                    }

                    return ExitCodes.Success;
                }

                if ("generate".StartsWith(command, StringComparison.Ordinal))
                {
                    new GenerateCommand().Run(csproj, args.Skip(1).ToArray());
                    return ExitCodes.Success;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return ExitCodes.Exception;
            }

            WriteHelp();
            return ExitCodes.InvalidCommand;
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
        }
    }
}