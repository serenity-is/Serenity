using Serenity.Data;
using Serenity.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Serenity.CodeGenerator
{
    public partial class GenerateCommand
    {
        private class AppSettingsFormat
        {
            public ConnectionStringOptions Data { get; }

            public AppSettingsFormat()
            {
                Data = new ConnectionStringOptions();
            }

            public class ConnectionInfo
            {
                public string ConnectionString { get; set; }
                public string ProviderName { get; set; }
            }
        }

        private static string GetOption(string[] args, string opt)
        {
            var dash = "-" + opt;
            var val = args.FirstOrDefault(x => x.StartsWith(dash + ":", StringComparison.Ordinal));
            if (val != null)
                return val[(dash.Length + 1)..];

            var idx = Array.IndexOf(args, dash);
            if (idx >= 0 && idx < args.Length - 1)
            {
                val = args[idx + 1];
                if (val.StartsWith("\"", StringComparison.Ordinal) && 
                    val.EndsWith("\"", StringComparison.Ordinal))
                    return val[1..^1];
                else
                    return val;
            }

            return null;
        }

        public static void Run(string csproj, string[] args, IFileSystem fileSystem)
        {
            var projectDir = Path.GetDirectoryName(csproj);
            if (!args.Any())
            {
                var exitCode = new Interactive(fileSystem).Run();
                if (exitCode != ExitCodes.Success &&
                    exitCode != ExitCodes.Help)
                    Environment.Exit((int)exitCode);

                return;
            }

            var outFile = GetOption(args, "o").TrimToNull();
            var connectionKey = GetOption(args, "c").TrimToNull();
            var table = GetOption(args, "t").TrimToNull();
            var what = GetOption(args, "w").TrimToNull();
            var module = GetOption(args, "m").TrimToNull();
            var identifier = GetOption(args, "i").TrimToNull();
            var permissionKey = GetOption(args, "p").TrimToNull();
            if (identifier != null)
                CodeFileHelper.Overwrite = true;

            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));

            var connectionStringOptions = new ConnectionStringOptions();

            if (!string.IsNullOrEmpty(config.CustomTemplates))
                Templates.TemplatePath = Path.Combine(projectDir, config.CustomTemplates);

            foreach (var x in config.Connections.Where(x => !x.ConnectionString.IsEmptyOrNull()))
            {
                connectionStringOptions[x.Key] = new ConnectionStringEntry
                {
                    ConnectionString = x.ConnectionString,
                    ProviderName = x.ProviderName,
                    Dialect = x.Dialect
                };
            }

            foreach (var name in config.GetAppSettingsFiles())
            {
                var path = Path.Combine(projectDir, name);
                if (File.Exists(name))
                {
                    var appSettings = JSON.ParseTolerant<AppSettingsFormat>(File.ReadAllText(path).TrimToNull() ?? "{}");
                    if (appSettings.Data != null)
                        foreach (var data in appSettings.Data)
                        {
                            // not so nice fix for relative paths, e.g. sqlite etc.
                            if (data.Value.ConnectionString.Contains("../../..", StringComparison.Ordinal))
                                data.Value.ConnectionString = data.Value
                                    .ConnectionString.Replace("../../..", Path.GetDirectoryName(csproj), StringComparison.Ordinal);
                            else if (data.Value.ConnectionString.Contains(@"..\..\..\", StringComparison.Ordinal))
                                data.Value.ConnectionString = data.Value.ConnectionString.Replace(@"..\..\..\", 
                                    Path.GetDirectoryName(csproj), StringComparison.Ordinal);

                            connectionStringOptions[data.Key] = data.Value;
                        }
                }
            }

            if (connectionStringOptions.Count == 0)
            {
                Console.Error.WriteLine("No connections in appsettings files or sergen.json!");
                Environment.Exit(1);
            }

            var connectionKeys = connectionStringOptions.Keys.OrderBy(x => x).ToArray();

            if (outFile == null && connectionKey == null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== Table Code Generation ===");
                Console.WriteLine("");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Available Connections:");
                Console.ResetColor();
                foreach (var x in connectionKeys)
                    Console.WriteLine(x);
                Console.ResetColor();
                Console.WriteLine();
            }
            else if (connectionKey == null)
            {
                File.WriteAllText(outFile, JSON.Stringify(connectionStringOptions.Keys.OrderBy(x => x)));
                Environment.Exit(0);
            }

            string userInput = null;

            if (outFile == null && connectionKey == null)
            {
                userInput = connectionKeys.Length == 1 ? connectionKeys[0] : null;
                while (connectionKey == null ||
                    !connectionKeys.Contains(connectionKey, StringComparer.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter a Connection: ('!' to abort)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    connectionKey = Hinter.ReadHintedLine(connectionKeys, userInput: userInput);
                    userInput = connectionKey;

                    if (connectionKey == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            userInput = connectionKey;
            if (!connectionStringOptions.ContainsKey(userInput))
            {
                Console.Error.WriteLine("Can't find connection with key: " + userInput + "!");
                Environment.Exit(1);
            }

            if (outFile == null)
            {
                Console.ResetColor();
                Console.WriteLine();
            }

            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", 
                Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("System.Data.SqlClient",
                Microsoft.Data.SqlClient.SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", 
                Microsoft.Data.Sqlite.SqliteFactory.Instance);
            DbProviderFactories.RegisterFactory("Npgsql", 
                Npgsql.NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", 
                FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", 
                MySqlConnector.MySqlConnectorFactory.Instance);

            var sqlConnections = new DefaultSqlConnections(
                new DefaultConnectionStrings(connectionStringOptions));

            ISchemaProvider schemaProvider;
            List<TableName> tableNames;
            using (var connection = sqlConnections.NewByKey(connectionKey))
            {
                schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
                tableNames = schemaProvider.GetTableNames(connection).ToList();
            }

            var tables = tableNames.Select(x => x.Tablename).ToList();
            var confConnection = config.Connections.FirstOrDefault(x =>
                string.Compare(x.Key, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

            if (outFile == null && table == null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Available Tables:");
                Console.ResetColor();

                foreach (var x in tables)
                    Console.WriteLine(x);
            }
            else if (table == null)
            {
                File.WriteAllText(outFile, JSON.Stringify(tableNames.Select(x =>
                {
                    var xct = confConnection?.Tables.FirstOrDefault(z => string.Compare(z.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);
                    return new
                    {
                        name = x.Tablename,
                        module = xct == null || xct.Module.IsEmptyOrNull() ? RowGenerator.ClassNameFromTableName(connectionKey) : xct.Module,
                        permission = xct == null || xct.PermissionKey.IsTrimmedEmpty() ? "Administration:General" : xct.PermissionKey,
                        identifier = xct == null || xct.Identifier.IsEmptyOrNull() ? RowGenerator.ClassNameFromTableName(x.Table) : xct.Identifier,
                    };
                })));
                
                Environment.Exit(0);
            }

            userInput = tables.Count == 1 ? tables[0] : null;
            if (userInput == null && schemaProvider.DefaultSchema != null && 
                tables.Any(x => x.StartsWith(schemaProvider.DefaultSchema + ".", StringComparison.Ordinal)))
                userInput = schemaProvider.DefaultSchema + ".";

            if (outFile == null)
            {
                Console.WriteLine();

                while (table == null ||
                    !tables.Contains(table, StringComparer.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter a Table: ('!' to abort)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    table = Hinter.ReadHintedLine(tables, userInput: userInput);
                    userInput = table;

                    if (table == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            userInput = table;
            var tableName = tableNames.First(x => string.Compare(x.Tablename, userInput, StringComparison.OrdinalIgnoreCase) == 0);
            if (tableName == null)
            {
                Console.Error.WriteLine("Can't find table with name: " + userInput + "!");
                Environment.Exit(1);
            }

            var confTable = confConnection?.Tables.FirstOrDefault(x =>
                string.Compare(x.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);

            if (module == null)
            {
                userInput = confTable == null || confTable.Module.IsEmptyOrNull() ?
                    RowGenerator.ClassNameFromTableName(connectionKey) : confTable.Module;

                Console.WriteLine();

                while (module.IsTrimmedEmpty())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter a Module name for table: ('!' to abort)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    module = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                    userInput = module;

                    if (module == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            if (identifier == null)
            {
                userInput = confTable == null || confTable.Identifier.IsEmptyOrNull() ?
                    RowGenerator.ClassNameFromTableName(tableName.Table) : confTable.Identifier;

                Console.WriteLine();

                while (identifier.IsTrimmedEmpty())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter a class Identifier for table: ('!' to abort)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    identifier = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                    userInput = identifier;

                    if (identifier == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            if (permissionKey == null)
            {
                userInput = confTable == null || confTable.PermissionKey.IsTrimmedEmpty() ?
                    "Administration:General" : confTable.PermissionKey;

                Console.WriteLine();

                while (permissionKey.IsTrimmedEmpty())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter a Permission Key for table: ('!' to abort)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    permissionKey = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                    userInput = permissionKey;

                    if (permissionKey == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }


            if (what == null)
            { 
                Console.WriteLine();

                userInput = "RSUC";
                while (what.IsEmptyOrNull())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Choose What to Generate (R:Row, S:Repo+Svc, U=UI, C=Custom)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    what = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                    userInput = what;

                    if (what == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            config.GenerateRow = what.Contains("R", StringComparison.OrdinalIgnoreCase);
            config.GenerateService = what.Contains("S", StringComparison.OrdinalIgnoreCase);
            config.GenerateUI = what.Contains("U", StringComparison.OrdinalIgnoreCase);
            config.GenerateCustom = what.Contains("C", StringComparison.OrdinalIgnoreCase);

            Console.ResetColor();
            Console.WriteLine();

            if (confConnection == null)
            {
                confConnection = new GeneratorConfig.Connection
                {
                    Key = connectionKey
                };
                config.Connections.Add(confConnection);
            }

            if (confTable == null)
            {
                confTable = new GeneratorConfig.Table
                {
                    Identifier = identifier,
                    Module = module,
                    PermissionKey = permissionKey,
                    Tablename = tableName.Tablename
                };

                confConnection.Tables.Add(confTable);
            }
            else
            {
                confTable.Identifier = identifier;
                confTable.Module = module;
                confTable.PermissionKey = permissionKey;
            }

            File.WriteAllText(Path.Combine(projectDir, "sergen.json"), config.SaveToJson());

            using (var connection = sqlConnections.NewByKey(connectionKey))
            {
                connection.Open();

                var csprojContent = File.ReadAllText(csproj);
                var net5Plus = !new Regex(@"\<TargetFramework\>.*netcoreapp.*\<\/TargetFramework\>", RegexOptions.Multiline | RegexOptions.Compiled)
                    .IsMatch(csprojContent);

                var rowModel = RowGenerator.GenerateModel(connection, tableName.Schema, tableName.Table,
                    module, connectionKey, identifier, permissionKey, config, net5Plus);

                rowModel.AspNetCore = true;
                rowModel.NET5Plus = net5Plus;

                var kdiff3Paths = new[]
                {
                    config.KDiff3Path
                };

                CodeFileHelper.Kdiff3Path = kdiff3Paths.FirstOrDefault(File.Exists);
                CodeFileHelper.TSCPath = config.TSCPath ?? "tsc";

                new EntityCodeGenerator(rowModel, config, csproj).Run();
            }
        }
    }
}