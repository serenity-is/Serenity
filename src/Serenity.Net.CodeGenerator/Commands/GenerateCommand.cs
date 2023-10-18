using Serenity.Data.Schema;
using Spectre.Console;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand : BaseFileSystemCommand
{
    private readonly IAnsiConsole ansiConsole;

    public GenerateCommand(IGeneratorFileSystem fileSystem, IAnsiConsole ansiConsole) 
        : base(fileSystem)
    {
        this.ansiConsole = ansiConsole ?? throw new ArgumentNullException(nameof(ansiConsole));
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

    private void Error(string error)
    {
        ansiConsole.Write(new Markup($"[bold red]{error}[/]"));
        ansiConsole.WriteLine();
    }

    private void WriteHeading(string text)
    {
        ansiConsole.WriteLine();
        ansiConsole.Write(new Spectre.Console.Rule($"[bold springgreen3_1]{text}[/]")
        {
            Justification = Justify.Left
        });
    }

    public ExitCodes Run(string csproj, string[] args)
    {
        var projectDir = fileSystem.GetDirectoryName(csproj);
        var config = fileSystem.LoadGeneratorConfig(projectDir);

        var connectionStrings = ParseConnectionStrings(fileSystem, csproj, config);
        if (connectionStrings.Count == 0)
        {
            Error("No connections in appsettings files or sergen.json!");
            return ExitCodes.NoConnectionString;
        }

        var argsConnectionKey = GetOption(args, "c").TrimToNull();
        var argsModule = GetOption(args, "m").TrimToNull();
        var argsIdentifier = GetOption(args, "i").TrimToNull();
        var argsPermissionKey = GetOption(args, "p").TrimToNull();

        if (!string.IsNullOrEmpty(config.CustomTemplates))
            Templates.TemplatePath = fileSystem.Combine(projectDir, config.CustomTemplates);

        WriteHeading("Table Code Generation");

        var argsTable = GetOption(args, "t").TrimToNull();
        var what = GetOption(args, "w").TrimToNull();

        var connectionKey = argsConnectionKey ?? SelectConnection(connectionStrings);

        if (string.IsNullOrEmpty(connectionKey))
        {
            Error("No connection selected!");
            return ExitCodes.NoConnectionString;
        }

        if (!connectionStrings.ContainsKey(connectionKey))
        {
            Error($"Can't find connection with key: {connectionKey}!");
            return ExitCodes.InvalidConnectionKey;
        }

        RegisterSqlProviders();
        var sqlConnections = new DefaultSqlConnections(new DefaultConnectionStrings(connectionStrings));

        ISchemaProvider schemaProvider;
        List<TableName> allTableEntries;
        using (var connection = sqlConnections.NewByKey(connectionKey))
        {
            schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
            allTableEntries = schemaProvider.GetTableNames(connection).ToList();
        }

        var allTableNames = allTableEntries.Select(x => x.Tablename).ToList();
        var confConnection = config.Connections.FirstOrDefault(x =>
            string.Compare(x.Key, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

        var selectedTableNames = !string.IsNullOrEmpty(argsTable) ? new[] { argsTable } : SelectTables(allTableNames);
        if (!selectedTableNames.Any())
        {
            Error("No tables selected!");
            return ExitCodes.NoTablesSelected;
        }

        string module = null;
        var permissionKey = argsPermissionKey ?? "Administration:General";
        var generateData = new Dictionary<string, (string module, string identifier, string permissionKey, TableName table)>();

        foreach (var tableName in selectedTableNames)
        {
            var confTable = confConnection?.Tables?.FirstOrDefault(x => string.Compare(x.Tablename,
                tableName, StringComparison.OrdinalIgnoreCase) == 0);

            var tableEntry = allTableEntries.FirstOrDefault(x => x.Tablename == tableName) ??
                allTableEntries.FirstOrDefault(x => string.Equals(x.Tablename, tableName, 
                    StringComparison.OrdinalIgnoreCase));

            if (tableEntry is null)
            {
                Error($"Can't find table with name: {tableName}!");
                return ExitCodes.InvalidTable;
            }

            if (string.IsNullOrEmpty(module))
                module = confTable?.Module?.TrimToNull() is null ?
                    EntityModelGenerator.IdentifierForTable(connectionKey) : confTable.Module;

            module = argsModule ?? SelectModule(tableName, module);

            var defaultIdentifier = confTable?.Identifier?.IsTrimmedEmpty() != false ?
                EntityModelGenerator.IdentifierForTable(tableEntry.Table) : confTable.Identifier;

            var identifier = (selectedTableNames.Count() == 1 ? 
                argsIdentifier : null) ?? SelectIdentifier(tableName, defaultIdentifier);

            permissionKey = argsPermissionKey ?? SelectPermissionKey(tableName, confTable?.PermissionKey?.TrimToNull() ?? permissionKey);

            generateData.Add(tableName, (module, identifier, permissionKey, tableEntry));
        }

        if (what != null)
        {
            config.GenerateRow = what.Contains('R', StringComparison.OrdinalIgnoreCase);
            config.GenerateService = what.Contains('S', StringComparison.OrdinalIgnoreCase);
            config.GenerateUI = what.Contains('U', StringComparison.OrdinalIgnoreCase);
            config.GenerateCustom = what.Contains('C', StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var whatToGenerate = SelectWhatToGenerate();
            config.GenerateRow = whatToGenerate.Contains("Row");
            config.GenerateService = whatToGenerate.Contains("Services");
            config.GenerateUI = whatToGenerate.Contains("User Interface");
            config.GenerateCustom = whatToGenerate.Contains("Custom");
        }

        foreach (var data in generateData)
        {
            var confTable = confConnection?.Tables.FirstOrDefault(x => string.Compare(x.Tablename, data.Key, 
                StringComparison.OrdinalIgnoreCase) == 0);

            var inputs = new EntityModelInputs
            {
                Config = config,
                ConnectionKey = connectionKey,
                Identifier = data.Value.identifier,
                Module = data.Value.module,
                PermissionKey = data.Value.permissionKey,
                Table = data.Value.table.Table,
                Schema = data.Value.table.Schema
            };

            UpdateConfigTable(inputs, data.Value.table.Tablename, confConnection, confTable);

            var generator = CreateCodeGenerator(inputs, new EntityModelGenerator(),
                csproj, fileSystem, sqlConnections, interactive: argsIdentifier is null);

            generator.Run();
        }

        if (config.SaveGeneratedTables != false)
            fileSystem.WriteAllText(fileSystem.Combine(projectDir, "sergen.json"), config.SaveToJson());

        return ExitCodes.Success;
    }
}