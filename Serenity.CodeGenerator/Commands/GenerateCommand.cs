using Serenity.Data;
using Serenity.Data.Schema;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public class GenerateCommand
    {
        private class AppSettingsFormat
        {
            public Dictionary<string, ConnectionInfo> Data { get; }

            public AppSettingsFormat()
            {
                Data = new Dictionary<string, ConnectionInfo>(StringComparer.OrdinalIgnoreCase);
            }

            public class ConnectionInfo
            {
                public string ConnectionString { get; set; }
                public string ProviderName { get; set; }
            }
        }

        private string GetOption(string[] args, string opt)
        {
            var dash = "-" + opt;
            var val = args.FirstOrDefault(x => x.StartsWith(dash + ":"));
            if (val != null)
                return val.Substring(dash.Length + 1);

            var idx = Array.IndexOf(args, dash);
            if (idx >= 0 && idx < args.Length - 1)
            {
                val = args[idx + 1];
                if (val.StartsWith("\"") && val.EndsWith("\""))
                    return val.Substring(1, val.Length - 2);
                else
                    return val;
            }

            return null;
        }

        public void Run(string csproj, string[] args)
        {
            var projectDir = Path.GetDirectoryName(csproj);

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

            if (!string.IsNullOrEmpty(config.TemplatePath))
                Templates.TemplatePath = Path.Combine(projectDir, config.TemplatePath);

            var connectionKeys = config.Connections
                .Where(x => !x.ConnectionString.IsEmptyOrNull())
                .Select(x => x.Key).ToList();

            var appSettingsFile = Path.Combine(projectDir, "appsettings.json");
            AppSettingsFormat appSettings;
            if (File.Exists(appSettingsFile))
                appSettings = JSON.ParseTolerant<AppSettingsFormat>(File.ReadAllText(appSettingsFile).TrimToNull() ?? "{}");
            else
                appSettings = new AppSettingsFormat();

            connectionKeys.AddRange(appSettings.Data.Keys);

            var appSettingsFile2 = Path.Combine(projectDir, "appsettings.machine.json");
            if (File.Exists(appSettingsFile2))
            {
                var appSettings2 = JSON.ParseTolerant<AppSettingsFormat>(File.ReadAllText(appSettingsFile2).TrimToNull() ?? "{}");
                foreach (var pair in appSettings2.Data)
                    appSettings.Data[pair.Key] = pair.Value;
            }

            connectionKeys.AddRange(appSettings.Data.Keys);

            connectionKeys = connectionKeys.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();

            if (connectionKeys.Count == 0)
            {
                Console.Error.WriteLine("No connections in appsettings.json or sergen.json!");
                Environment.Exit(1);
            }

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
                File.WriteAllText(outFile, JSON.Stringify(connectionKeys));
                Environment.Exit(0);
            }

            string userInput = null;

            if (outFile == null && connectionKey == null)
            {
                userInput = connectionKeys.Count == 1 ? connectionKeys[0] : null;
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
            connectionKey = connectionKeys.Find(x => string.Compare(x, userInput, StringComparison.OrdinalIgnoreCase) == 0);
            if (connectionKey == null)
            {
                Console.Error.WriteLine("Can't find connection with key: " + userInput + "!");
                Environment.Exit(1);
            }

            if (outFile == null)
            {
                Console.ResetColor();
                Console.WriteLine();
            }

            var dataConnection = appSettings.Data.ContainsKey(connectionKey) ?
                appSettings.Data[connectionKey] : null;

            var confConnection = config.Connections.FirstOrDefault(x =>
                string.Compare(x.Key, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

            var connectionString = dataConnection != null ? dataConnection.ConnectionString.TrimToNull() : null;
            if (connectionString == null && confConnection != null)
                connectionString = confConnection.ConnectionString.TrimToNull();

            var providerName = dataConnection != null ? dataConnection.ProviderName.TrimToNull() : null;
            if (providerName == null && confConnection != null)
                providerName = confConnection.ProviderName.TrimToNull();
            providerName = providerName ?? "System.Data.SqlClient";

#if COREFX
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);
            DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);
            DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySql.Data.MySqlClient.MySqlClientFactory.Instance);
#endif

            if (connectionString.IndexOf("../../..") >= 0)
                connectionString = connectionString.Replace("../../..", Path.GetDirectoryName(csproj));
            else if (connectionString.IndexOf(@"..\..\..\") >= 0)
                connectionString = connectionString.Replace(@"..\..\..\", Path.GetDirectoryName(csproj));

            ISchemaProvider schemaProvider;
            List<TableName> tableNames;
            using (var connection = SqlConnections.New(connectionString, providerName))
            {
                schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
                tableNames = schemaProvider.GetTableNames(connection).ToList();
            }

            var tables = tableNames.Select(x => x.Tablename).ToList();

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
                    var xct = confConnection == null ? null : confConnection.Tables.FirstOrDefault(z => string.Compare(z.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);
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
                tables.Any(x => x.StartsWith(schemaProvider.DefaultSchema + ".")))
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

            var confTable = confConnection == null ? null : confConnection.Tables.FirstOrDefault(x =>
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
                    module = Hinter.ReadHintedLine(new string[0], userInput: userInput);
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
                    identifier = Hinter.ReadHintedLine(new string[0], userInput: userInput);
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
                    permissionKey = Hinter.ReadHintedLine(new string[0], userInput: userInput);
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

                userInput = "RSU";
                while (what.IsEmptyOrNull())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Choose What to Generate (R:Row, S:Repo+Svc, U=Cols+Form+Page+Grid+Dlg+Css)");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    what = Hinter.ReadHintedLine(new string[0], userInput: userInput);
                    userInput = what;

                    if (what == "!")
                    {
                        Console.ResetColor();
                        return;
                    }
                }
            }

            config.GenerateRow = what.IndexOf("R", StringComparison.OrdinalIgnoreCase) >= 0;
            config.GenerateService = what.IndexOf("S", StringComparison.OrdinalIgnoreCase) >= 0;
            config.GenerateUI = what.IndexOf("U", StringComparison.OrdinalIgnoreCase) >= 0;

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

            using (var connection = SqlConnections.New(connectionString, providerName))
            {
                connection.Open();

                var rowModel = RowGenerator.GenerateModel(connection, tableName.Schema, tableName.Table,
                    module, connectionKey, identifier, permissionKey, config);

                rowModel.AspNetCore = true;

                new EntityCodeGenerator(rowModel, config, csproj).Run();
            }
        }
    }
}