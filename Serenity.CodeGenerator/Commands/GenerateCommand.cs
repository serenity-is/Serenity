using Serenity.Data;
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

        public void Run(string projectJson)
        {
            var projectDir = Path.GetDirectoryName(projectJson);

            var config = GeneratorConfig.LoadFromFile(Path.Combine(projectDir, "sergen.json"));
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


            string userInput = connectionKeys.Count == 1 ? connectionKeys[0] : null;
            string connectionKey = null;
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

            connectionKey = connectionKeys.Find(x => string.Compare(x, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

            Console.ResetColor();
            Console.WriteLine();

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

            // TODO: register other factories from config file?
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);

            List<TableName> tableNames;
            using (var connection = SqlConnections.New(connectionString, providerName))
                tableNames = SqlSchemaInfo.GetTableNames(connection);

            var tables = tableNames.Select(x => x.Tablename).ToList();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Available Tables:");
            Console.ResetColor();

            foreach (var x in tables)
                Console.WriteLine(x);

            userInput = tables.Count == 1 ? tables[0] : null;
            if (userInput == null && tables.Any(x => x.StartsWith("dbo.")))
                userInput = "dbo.";

            Console.WriteLine();

            string table = null;
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

            var tableName = tableNames.First(x => string.Compare(x.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);
            var confTable = confConnection == null ? null : confConnection.Tables.FirstOrDefault(x =>
                string.Compare(x.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);

            userInput = confTable == null || confTable.Module.IsEmptyOrNull() ?
                RowGenerator.ClassNameFromTableName(connectionKey) : confTable.Module;

            Console.WriteLine();

            string module = null;
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

            userInput = confTable == null || confTable.Identifier.IsEmptyOrNull() ?
                RowGenerator.ClassNameFromTableName(tableName.Table) : confTable.Identifier;

            Console.WriteLine();

            string identifier = null;
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

            userInput = confTable == null || confTable.PermissionKey.IsTrimmedEmpty() ?
                "Administration:General" : confTable.PermissionKey;

            Console.WriteLine();

            string permissionKey = null;
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

            Console.WriteLine();

            userInput = "RSU";
            string generate = null;
            while (generate.IsEmptyOrNull())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Choose What to Generate (R:Row, S:Repo+Svc, U=Cols+Form+Page+Grid+Dlg+Css)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                generate = Hinter.ReadHintedLine(new string[0], userInput: userInput);
                userInput = generate;

                if (generate == "!")
                {
                    Console.ResetColor();
                    return;
                }
            }

            config.GenerateRow = generate.IndexOf("R", StringComparison.OrdinalIgnoreCase) >= 0;
            config.GenerateService = generate.IndexOf("S", StringComparison.OrdinalIgnoreCase) >= 0;
            config.GenerateUI = generate.IndexOf("U", StringComparison.OrdinalIgnoreCase) >= 0;

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

                new EntityCodeGenerator(rowModel, config, projectJson).Run();
            }

            Console.ReadLine();
        }
    }
}