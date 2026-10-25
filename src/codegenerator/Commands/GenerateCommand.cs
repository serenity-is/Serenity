using Serenity.Data.Schema;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public IArgumentReader Arguments { get; set; } = null!;

    /// <summary>
    /// Never prompts: missing values fall back to their defaults, and values
    /// without a default (table, connection key when there are several) make
    /// the command fail instead of asking. Existing files are backed up instead
    /// of asking for confirmation.
    /// </summary>
    public bool NonInteractive { get; set; }

    /// <summary>
    /// What to do with existing files that would change: <see cref="OverwriteMode.Ask"/>
    /// when the switch is omitted, otherwise the requested mode. Set from "--overwrite".
    /// </summary>
    public OverwriteMode? Overwrite { get; set; }

    public override ExitCodes Run()
    {
        ArgumentNullException.ThrowIfNull(Arguments);

        var projectDir = FileSystem.GetDirectoryName(ProjectFile);
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        var connectionStrings = ParseConnectionStrings(FileSystem, ProjectFile, config);
        if (connectionStrings.Count == 0)
        {
            Error("No connections in appsettings files or sergen.json!");
            return ExitCodes.NoConnectionString;
        }

        var argsConnectionKey = Arguments.GetString(
            ["cnk", "connkey", "connectionkey", "connection-key"]);
        var argsModule = Arguments.GetString(["mod", "module"], requiresValue: false);
        var argsIdentifier = Arguments.GetString(["cls", "idn", "identifier"]);
        var argsPermissionKey = Arguments.GetString(
            ["pms", "permission", "permissionkey", "permission-key"], requiresValue: false);
        var argsTable = Arguments.GetString(["tbl", "table"]);
        var argsWhat = Arguments.GetString(["wtg", "what", "whattogenerate"]);

        NonInteractive = Arguments.HasSwitch(
            ["nointeractive", "no-interactive", "noninteractive", "ni"]);

        var argsOverwrite = Arguments.GetOptionalString(["ovr", "overwrite"]);
        if (argsOverwrite != null)
        {
            if (!TryParseOverwriteMode(argsOverwrite, out var overwriteMode))
            {
                Error($"Invalid --overwrite value: {argsOverwrite}. " +
                    "Use yes (replace, keep a backup), no (fail), skip (leave as is) " +
                    "or force (replace, no backup).");
                return ExitCodes.InvalidArguments;
            }

            Overwrite = overwriteMode;
        }

        Arguments.ThrowIfRemaining();

        if (argsModule != null && !IsValidModuleName(argsModule))
        {
            Error(InvalidModuleNameMessage);
            return ExitCodes.InvalidArguments;
        }

        var templates = new Templates();

        if (!string.IsNullOrEmpty(config.CustomTemplates))
            templates.TemplatePath = FileSystem.Combine(projectDir, config.CustomTemplates);

        Console.WriteLine("Table Code Generation", ConsoleColor.DarkGreen);
        Console.WriteLine();

        var connectionKey = argsConnectionKey ?? SelectConnection(connectionStrings);

        if (string.IsNullOrEmpty(connectionKey))
        {
            Error(NonInteractive
                ? $"Multiple connections found, specify one with --connection-key: " +
                    string.Join(", ", connectionStrings.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                : "No connection selected!");
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
            allTableEntries = [.. schemaProvider.GetTableNames(connection)];
        }

        var allTableNames = allTableEntries.Select(x => x.Tablename).ToList();
        var confConnection = config.Connections?.FirstOrDefault(x =>
            string.Compare(x.Key, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

        List<string> selectedTableNames = !string.IsNullOrEmpty(argsTable) ? [argsTable] : SelectTables(allTableNames);
        if (selectedTableNames.Count == 0)
        {
            Error(NonInteractive
                ? "No table selected! Specify one with --table, e.g. --table dbo.Movie"
                : "No tables selected!");
            return ExitCodes.NoTablesSelected;
        }

        string? module = null;
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
                    EntityModelFactory.IdentifierForTable(connectionKey) : confTable.Module;

            module = argsModule ?? SelectModule(tableName, module);
            if ((argsModule == null && string.IsNullOrEmpty(module)) ||
                !IsValidModuleName(module))
            {
                Error(InvalidModuleNameMessage);
                return ExitCodes.InvalidArguments;
            }

            var defaultIdentifier = confTable?.Identifier?.IsTrimmedEmpty() != false ?
                EntityModelFactory.IdentifierForTable(tableEntry.Table) : confTable.Identifier;

            var identifier = (selectedTableNames.Count == 1 ? 
                argsIdentifier : null) ?? SelectIdentifier(tableName, defaultIdentifier);
            if (!IsValidCSharpIdentifier(identifier))
            {
                Error(InvalidIdentifierMessage);
                return ExitCodes.InvalidArguments;
            }

            permissionKey = argsPermissionKey ?? SelectPermissionKey(tableName, confTable?.PermissionKey?.TrimToNull() ?? permissionKey);

            inputsList.Add(new() 
            {
                ConnectionKey = connectionKey,
                Config = config,
                Schema = tableEntry.Schema!,
                Table = tableEntry.Table!,
                Module = module,
                Identifier = identifier,
                PermissionKey = permissionKey
            });
        }

        if (argsWhat != null)
        {
            config.GenerateRow = argsWhat == "*" || argsWhat.Contains('R', StringComparison.OrdinalIgnoreCase);
            config.GenerateService = argsWhat == "*" || argsWhat.Contains('S', StringComparison.OrdinalIgnoreCase);
            config.GenerateUI = argsWhat == "*" || argsWhat.Contains('U', StringComparison.OrdinalIgnoreCase);
            config.GenerateCustom = argsWhat == "*" || argsWhat.Contains('C', StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            var whatToGenerate = SelectWhatToGenerate();
            config.GenerateRow = whatToGenerate.Contains("Row", StringComparer.Ordinal);
            config.GenerateService = whatToGenerate.Contains("Services", StringComparer.Ordinal);
            config.GenerateUI = whatToGenerate.Contains("User Interface", StringComparer.Ordinal);
            config.GenerateCustom = whatToGenerate.Contains("Custom", StringComparer.Ordinal);
        }

        var modelFactory = new EntityModelFactory();

        EntityModel createModel(EntityModelInputs inputs)
        {
            using var connection = sqlConnections.NewByKey(inputs.ConnectionKey);
            connection.EnsureOpen();

            var csprojContent = FileSystem.ReadAllText(Project.ProjectFile);
            inputs.Net5Plus = !NetCoreAppRegex().IsMatch(csprojContent);
            inputs.Net8Plus = inputs.Net5Plus && !Net567Regex().IsMatch(csprojContent);

            inputs.SchemaIsDatabase = connection.GetDialect().ServerType.StartsWith("MySql",
                StringComparison.OrdinalIgnoreCase);

            inputs.DataSchema = new EntityDataSchema(connection);

            if (config.ParseGlobalUsings != false)
            {
                var globalUsings = Project.GetGlobalUsings();
                if (globalUsings != null)
                {
                    foreach (var gu in globalUsings)
                    {
                        // skip aliased usings
                        if (string.IsNullOrEmpty(gu.Value))
                            inputs.GlobalUsings.Add(gu.Key);
                    }
                }
            }

            if (config.IncludeGlobalUsings != null)
                inputs.GlobalUsings.AddRange(config.IncludeGlobalUsings);

            inputs.NullableRefTypes = Project.GetNullable() is "enable" or "annotations";

            return modelFactory.Create(inputs);
        }

        ApplicationMetadata? application = null;
        try
        {
            var assemblyFiles = Project.GetAssemblyList(config.ServerTypings?.Assemblies);
            if (assemblyFiles != null && assemblyFiles.Length > 0)
            {
                application = new ApplicationMetadata(FileSystem, assemblyFiles)
                {
                    DefaultSchema = schemaProvider.DefaultSchema
                };

                foreach (var inputs in inputsList)
                {
                    inputs.SkipForeignKeys = true;
                    try
                    {
                        var model = createModel(inputs);
                        application.EntityModels.Add(model);
                    }
                    finally
                    {
                        inputs.SkipForeignKeys = false;
                    }
                }
            }
        }
        catch { }

        // --no-interactive can't ask, so it refuses to replace anything unless
        // --overwrite said otherwise
        var writer = new GeneratedFileWriter(Project.FileSystem, Console)
        {
            Overwrite = Overwrite ?? (NonInteractive ? OverwriteMode.Fail : OverwriteMode.Ask)
        };

        try
        {
            foreach (var inputs in inputsList)
            {
                UpdateConfigTableFor(inputs, confConnection);
                inputs.Application = application!;
                var model = createModel(inputs);
                var generator = new EntityCodeGenerator(Project, model, inputs.Config, templates, writer);
                generator.Run();
            }
        }
        catch (GeneratedFileExistsException ex)
        {
            Error(ex.Message);
            return ExitCodes.OverwriteRefused;
        }

        if (config.SaveGeneratedTables != false)
            FileSystem.WriteAllText(FileSystem.Combine(projectDir, "sergen.json"), config.SaveToJson());

        return ExitCodes.Success;
    }
}