using Serenity.Data.Schema;

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
        if (!args.Any())
        {
            var exitCode = new Interactive(fileSystem, Spectre.Console.AnsiConsole.Console).Run(csproj);
            if (exitCode != ExitCodes.Success &&
                exitCode != ExitCodes.Help)
                Environment.Exit((int)exitCode);

            return;
        }

        var projectDir = fileSystem.GetDirectoryName(csproj);
        var config = fileSystem.LoadGeneratorConfig(projectDir);

        var inputs = new EntityModelInputs
        {
            ConnectionKey = GetOption(args, "c").TrimToNull(),
            Module = GetOption(args, "m").TrimToNull(),
            Identifier = GetOption(args, "i").TrimToNull(),
            PermissionKey = GetOption(args, "p").TrimToNull(),
            Config = config
        };

        var interactive = inputs.Identifier is null;

        var table = GetOption(args, "t").TrimToNull();
        var what = GetOption(args, "w").TrimToNull();
        var outFile = GetOption(args, "o").TrimToNull();

        if (!string.IsNullOrEmpty(inputs.Config.CustomTemplates))
            Templates.TemplatePath = fileSystem.Combine(projectDir, inputs.Config.CustomTemplates);

        var connectionStringOptions = ParseConnectionStringOptions(fileSystem, csproj, inputs.Config);
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
                    module = xct == null || string.IsNullOrEmpty(xct.Module) ? EntityModelGenerator.IdentifierForTable(inputs.ConnectionKey) : xct.Module,
                    permission = xct == null || string.IsNullOrWhiteSpace(xct.PermissionKey) ? "Administration:General" : xct.PermissionKey,
                    identifier = xct == null || string.IsNullOrEmpty(xct.Identifier) ? EntityModelGenerator.IdentifierForTable(x.Table) : xct.Identifier,
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
            userInput = confTable == null || string.IsNullOrEmpty(confTable.Module) ?
                EntityModelGenerator.IdentifierForTable(inputs.ConnectionKey) : confTable.Module;

            Console.WriteLine();

            while (string.IsNullOrWhiteSpace(inputs.Module))
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
            userInput = confTable == null || string.IsNullOrEmpty(confTable.Identifier) ?
                EntityModelGenerator.IdentifierForTable(tableName.Table) : confTable.Identifier;

            Console.WriteLine();

            while (string.IsNullOrWhiteSpace(inputs.Identifier))
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
            userInput = confTable == null || string.IsNullOrWhiteSpace(confTable.PermissionKey) ?
                "Administration:General" : confTable.PermissionKey;

            Console.WriteLine();

            while (string.IsNullOrWhiteSpace(inputs.PermissionKey))
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
            while (string.IsNullOrEmpty(what))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Choose What to Generate (R:Row, S:Services, U=UI, C=Custom)");
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
}