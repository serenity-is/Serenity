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
        var inputsList = new List<EntityModelInputs>();

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

            inputsList.Add(new() 
            {
                ConnectionKey = connectionKey,
                Config = config,
                Schema = tableEntry.Schema,
                Table = tableEntry.Table,
                Module = module,
                Identifier = identifier,
                PermissionKey = permissionKey
            });
        }

        if (what != null)
        {
            config.GenerateRow = what == "*" || what.Contains('R', StringComparison.OrdinalIgnoreCase);
            config.GenerateService = what == "*" || what.Contains('S', StringComparison.OrdinalIgnoreCase);
            config.GenerateUI = what == "*" || what.Contains('U', StringComparison.OrdinalIgnoreCase);
            config.GenerateCustom = what == "*" || what.Contains('C', StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var whatToGenerate = SelectWhatToGenerate();
            config.GenerateRow = whatToGenerate.Contains("Row", StringComparer.Ordinal);
            config.GenerateService = whatToGenerate.Contains("Services", StringComparer.Ordinal);
            config.GenerateUI = whatToGenerate.Contains("User Interface", StringComparer.Ordinal);
            config.GenerateCustom = whatToGenerate.Contains("Custom", StringComparer.Ordinal);
        }

        ApplicationMetadata application = null;
        try
        {
            var assemblyFiles = ServerTypingsCommand.DetermineAssemblyFiles(fileSystem, csproj, config, (error) => { });
            if (assemblyFiles != null && assemblyFiles.Length > 0)
            {
                application = new ApplicationMetadata(fileSystem, assemblyFiles)
                {
                    DefaultSchema = schemaProvider.DefaultSchema
                };

                foreach (var inputs in inputsList)
                {
                    inputs.SkipForeignKeys = true;
                    try
                    {
                        var entityModel = CreateEntityModel(inputs, new EntityModelGenerator(), csproj, fileSystem, sqlConnections);
                        application.EntityModels.Add(entityModel);
                    }
                    finally
                    {
                        inputs.SkipForeignKeys = false;
                    }
                }
            }
        }
        catch { }

        foreach (var inputs in inputsList)
        {
            UpdateConfigTableFor(inputs, confConnection);

            inputs.Application = application;

            var generator = CreateCodeGenerator(inputs, new EntityModelGenerator(),
                csproj, fileSystem, sqlConnections, interactive: true);

            generator.Run();
        }

        if (config.SaveGeneratedTables != false)
            fileSystem.WriteAllText(fileSystem.Combine(projectDir, "sergen.json"), config.SaveToJson());

        return ExitCodes.Success;
    }
}