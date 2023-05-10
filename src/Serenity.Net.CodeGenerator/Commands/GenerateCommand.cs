using Serenity.Data.Schema;
using System.Data.Common;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand : BaseFileSystemCommand
{
    public GenerateCommand(IGeneratorFileSystem fileSystem) 
        : base(fileSystem)
    {
    }

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

    public void Run(string csproj, string[] args)
    {
        var projectDir = fileSystem.GetDirectoryName(csproj);
        if (!args.Any())
        {
            var exitCode = new Interactive(fileSystem, Spectre.Console.AnsiConsole.Console).Run();
            if (exitCode != ExitCodes.Success &&
                exitCode != ExitCodes.Help)
                Environment.Exit((int)exitCode);

            return;
        }

        var inputs = new EntityModelInputs
        {
            ConnectionKey = GetOption(args, "c").TrimToNull(),
            Module = GetOption(args, "m").TrimToNull(),
            Identifier = GetOption(args, "i").TrimToNull(),
            PermissionKey = GetOption(args, "p").TrimToNull(),
            Config = fileSystem.LoadGeneratorConfig(projectDir)
        };

        var interactive = inputs.Identifier is null;

        var table = GetOption(args, "t").TrimToNull();
        var what = GetOption(args, "w").TrimToNull();
        var outFile = GetOption(args, "o").TrimToNull();

        var connectionStringOptions = new ConnectionStringOptions();

        if (!string.IsNullOrEmpty(inputs.Config.CustomTemplates))
            Templates.TemplatePath = fileSystem.Combine(projectDir, inputs.Config.CustomTemplates);

        foreach (var x in inputs.Config.Connections.Where(x => !x.ConnectionString.IsEmptyOrNull()))
        {
            connectionStringOptions[x.Key] = new ConnectionStringEntry
            {
                ConnectionString = x.ConnectionString,
                ProviderName = x.ProviderName,
                Dialect = x.Dialect
            };
        }

        foreach (var name in inputs.Config.GetAppSettingsFiles())
        {
            var path = fileSystem.Combine(projectDir, name);
            if (fileSystem.FileExists(name))
            {
                var appSettings = JSON.ParseTolerant<AppSettingsFormat>(fileSystem.ReadAllText(path).TrimToNull() ?? "{}");
                if (appSettings.Data != null)
                    foreach (var data in appSettings.Data)
                    {
                        // not so nice fix for relative paths, e.g. sqlite etc.
                        if (data.Value.ConnectionString.Contains("../../..", StringComparison.Ordinal))
                            data.Value.ConnectionString = data.Value
                                .ConnectionString.Replace("../../..", fileSystem.GetDirectoryName(csproj), StringComparison.Ordinal);
                        else if (data.Value.ConnectionString.Contains(@"..\..\..\", StringComparison.Ordinal))
                            data.Value.ConnectionString = data.Value.ConnectionString.Replace(@"..\..\..\",
                                fileSystem.GetDirectoryName(csproj), StringComparison.Ordinal);

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

        if (outFile == null && inputs.ConnectionKey == null)
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
        else if (inputs.ConnectionKey == null)
        {
            fileSystem.WriteAllText(outFile, System.Text.Json.JsonSerializer.Serialize(connectionStringOptions.Keys.OrderBy(x => x)));
            Environment.Exit(0);
        }

        string userInput = null;

        if (outFile == null && inputs.ConnectionKey == null)
        {
            userInput = connectionKeys.Length == 1 ? connectionKeys[0] : null;
            while (inputs.ConnectionKey == null ||
                !connectionKeys.Contains(inputs.ConnectionKey, StringComparer.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a Connection: ('!' to abort)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                inputs.ConnectionKey = Hinter.ReadHintedLine(connectionKeys, userInput: userInput);
                userInput = inputs.ConnectionKey;

                if (inputs.ConnectionKey == "!")
                {
                    Console.ResetColor();
                    return;
                }
            }
        }

        userInput = inputs.ConnectionKey;
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

        RegisterSqlProviders();

        var sqlConnections = new DefaultSqlConnections(
            new DefaultConnectionStrings(connectionStringOptions));

        ISchemaProvider schemaProvider;
        List<TableName> tableNames;
        using (var connection = sqlConnections.NewByKey(inputs.ConnectionKey))
        {
            schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
            tableNames = schemaProvider.GetTableNames(connection).ToList();
        }

        var tables = tableNames.Select(x => x.Tablename).ToList();
        var confConnection = inputs.Config.Connections.FirstOrDefault(x =>
            string.Compare(x.Key, inputs.ConnectionKey, StringComparison.OrdinalIgnoreCase) == 0);

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
            fileSystem.WriteAllText(outFile, JSON.Stringify(tableNames.Select(x =>
            {
                var xct = confConnection?.Tables.FirstOrDefault(z => string.Compare(z.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);
                return new
                {
                    name = x.Tablename,
                    module = xct == null || xct.Module.IsEmptyOrNull() ? EntityModelGenerator.IdentifierForTable(inputs.ConnectionKey) : xct.Module,
                    permission = xct == null || xct.PermissionKey.IsTrimmedEmpty() ? "Administration:General" : xct.PermissionKey,
                    identifier = xct == null || xct.Identifier.IsEmptyOrNull() ? EntityModelGenerator.IdentifierForTable(x.Table) : xct.Identifier,
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
        var tableName = tableNames.First(x => string.Compare(x.Tablename, 
            userInput, StringComparison.OrdinalIgnoreCase) == 0);
        if (tableName == null)
        {
            Console.Error.WriteLine("Can't find table with name: " + userInput + "!");
            Environment.Exit(1);
        }

        inputs.Schema = tableName.Schema;
        inputs.Table = tableName.Table;

        var confTable = confConnection?.Tables.FirstOrDefault(x =>
            string.Compare(x.Tablename, table, StringComparison.OrdinalIgnoreCase) == 0);

        if (inputs.Module == null)
        {
            userInput = confTable == null || confTable.Module.IsEmptyOrNull() ?
                EntityModelGenerator.IdentifierForTable(inputs.ConnectionKey) : confTable.Module;

            Console.WriteLine();

            while (inputs.Module.IsTrimmedEmpty())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a Module name for table: ('!' to abort)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                inputs.Module = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                userInput = inputs.Module;

                if (inputs.Module == "!")
                {
                    Console.ResetColor();
                    return;
                }
            }
        }

        if (inputs.Identifier == null)
        {
            userInput = confTable == null || confTable.Identifier.IsEmptyOrNull() ?
                EntityModelGenerator.IdentifierForTable(tableName.Table) : confTable.Identifier;

            Console.WriteLine();

            while (inputs.Identifier.IsTrimmedEmpty())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a class Identifier for table: ('!' to abort)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                inputs.Identifier = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                userInput = inputs.Identifier;

                if (inputs.Identifier == "!")
                {
                    Console.ResetColor();
                    return;
                }
            }
        }

        if (inputs.PermissionKey == null)
        {
            userInput = confTable == null || confTable.PermissionKey.IsTrimmedEmpty() ?
                "Administration:General" : confTable.PermissionKey;

            Console.WriteLine();

            while (inputs.PermissionKey.IsTrimmedEmpty())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a Permission Key for table: ('!' to abort)");
                Console.ForegroundColor = ConsoleColor.Yellow;
                inputs.PermissionKey = Hinter.ReadHintedLine(Array.Empty<string>(), userInput: userInput);
                userInput = inputs.PermissionKey;

                if (inputs.PermissionKey == "!")
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

        Console.ResetColor();
        Console.WriteLine();

        inputs.Config.GenerateRow = what.Contains('R', StringComparison.OrdinalIgnoreCase);
        inputs.Config.GenerateService = what.Contains('S', StringComparison.OrdinalIgnoreCase);
        inputs.Config.GenerateUI = what.Contains('U', StringComparison.OrdinalIgnoreCase);
        inputs.Config.GenerateCustom = what.Contains('C', StringComparison.OrdinalIgnoreCase);

        UpdateConfigTable(inputs, tableName.Tablename, confConnection, confTable);

        if (inputs.Config.SaveGeneratedTables != false)
            fileSystem.WriteAllText(fileSystem.Combine(projectDir, "sergen.json"), inputs.Config.SaveToJson());

        using (var connection = sqlConnections.NewByKey(inputs.ConnectionKey))
        {
            connection.Open();
            var generator = CreateCodeGenerator(inputs, new EntityModelGenerator(),
                csproj, fileSystem, sqlConnections, interactive);
            generator.Run();
        }
    }

    private static void UpdateConfigTable(EntityModelInputs inputs, string tableName,
        GeneratorConfig.Connection confConnection,
        GeneratorConfig.Table confTable)
    {
        if (confConnection == null)
        {
            confConnection = new GeneratorConfig.Connection
            {
                Key = inputs.ConnectionKey
            };
            inputs.Config.Connections.Add(confConnection);
        }

        if (confTable == null)
        {
            confTable = new GeneratorConfig.Table
            {
                Identifier = inputs.Identifier,
                Module = inputs.Module,
                PermissionKey = inputs.PermissionKey,
                Tablename = tableName
            };

            confConnection.Tables.Add(confTable);
        }
        else
        {
            confTable.Identifier = inputs.Identifier;
            confTable.Module = inputs.Module;
            confTable.PermissionKey = inputs.PermissionKey;
        }
    }

    private static EntityCodeGenerator CreateCodeGenerator(
        EntityModelInputs inputs, IEntityModelGenerator modelGenerator, 
        string csproj, IGeneratorFileSystem fileSystem,
        ISqlConnections sqlConnections, bool interactive = true)
    {
        using var connection = sqlConnections.NewByKey(inputs.ConnectionKey);
        connection.EnsureOpen();

        var csprojContent = fileSystem.ReadAllText(csproj);
        inputs.Net5Plus = !new Regex(@"\<TargetFramework\>.*netcoreapp.*\<\/TargetFramework\>",
            RegexOptions.Multiline | RegexOptions.Compiled).IsMatch(csprojContent);

        inputs.SchemaIsDatabase = connection.GetDialect().ServerType.StartsWith("MySql",
            StringComparison.OrdinalIgnoreCase);

        inputs.DataSchema = new EntityDataSchema(connection);
        var rowModel = modelGenerator.GenerateModel(inputs);

        var codeFileHelper = new CodeFileHelper(fileSystem)
        {
            NoUserInteraction = !interactive,
            Kdiff3Path = new[] { inputs.Config.KDiff3Path }.FirstOrDefault(fileSystem.FileExists),
            TSCPath = inputs.Config.TSCPath ?? "tsc"
        };

        return new EntityCodeGenerator(fileSystem, codeFileHelper, rowModel, inputs.Config, csproj);
    }

    private static void RegisterSqlProviders()
    {
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
    }
}