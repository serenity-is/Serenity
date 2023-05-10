using Serenity.Data.Schema;
using Spectre.Console;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand
{
    private class Interactive
    {
        private readonly IGeneratorFileSystem fileSystem;
        private readonly IAnsiConsole ansiConsole;

        public Interactive(IGeneratorFileSystem fileSystem,
            IAnsiConsole ansiConsole)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            this.ansiConsole = ansiConsole ?? throw new ArgumentNullException(nameof(ansiConsole));
        }

        public ExitCodes Run()
        {
            ansiConsole.WriteLine();
            ansiConsole.Write(new Spectre.Console.Rule($"[bold springgreen3_1]Table Code Generation[/]")
            {
                Justification = Justify.Left
            });

            var csproj = SelectCsProj();
            if (csproj is null)
                return ExitCodes.NoProjectFiles;

            var projectDir = fileSystem.GetDirectoryName(csproj);
            var config = fileSystem.LoadGeneratorConfig(projectDir);

            if (!string.IsNullOrEmpty(config.CustomTemplates))
                Templates.TemplatePath = fileSystem.Combine(projectDir, config.CustomTemplates);

            var (connectionKey, sqlConnections) = SelectConnectionString(config, projectDir);

            if (connectionKey is null)
                return ExitCodes.NoConnectionString;
            var confConnection = config.Connections?.FirstOrDefault(x =>
                string.Compare(x.Key, connectionKey, StringComparison.OrdinalIgnoreCase) == 0);

            var (selectedTables, tableNames) = SelectedTables(sqlConnections, connectionKey);

            var module = string.Empty;
            var permissionKey = "Administration:General";

            var generateData = new Dictionary<string, (string module, string identifier, string permissionKey, TableName table)>();

            foreach (var table in selectedTables)
            {
                var confTable = confConnection?.Tables.FirstOrDefault(x => string.Compare(x.Tablename, table.Tablename, StringComparison.OrdinalIgnoreCase) == 0);

                if (string.IsNullOrEmpty(module))
                    module = confTable?.Module?.TrimToNull() is null ?
                        EntityModelGenerator.IdentifierForTable(connectionKey) : confTable.Module;

                module = SelectModule(table.Tablename, module);

                var defaultIdentifier = confTable?.Identifier?.TrimToNull() is null ?
                    EntityModelGenerator.IdentifierForTable(table.Table) : confTable.Identifier;
                var identifier = SelectIdentifier(table.Tablename, defaultIdentifier);

                permissionKey = SelectPermissionKey(table.Tablename, confTable?.PermissionKey?.TrimToNull() ?? permissionKey);
                generateData.Add(table.Tablename, (module, identifier, permissionKey, table));
            }

            var whatToGenerate = SelectWhatToGenerate();

            config.GenerateRow = whatToGenerate.Contains("Row");
            config.GenerateService = whatToGenerate.Contains("Repository & Service");
            config.GenerateUI = whatToGenerate.Contains("User Interface");
            config.GenerateCustom = whatToGenerate.Contains("Custom");

            foreach (var data in generateData)
            {
                var confTable = confConnection?.Tables.FirstOrDefault(x => string.Compare(x.Tablename, data.Key, StringComparison.OrdinalIgnoreCase) == 0);

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
                    csproj, fileSystem, sqlConnections, interactive: true);

                generator.Run();
            }

            if (config.SaveGeneratedTables != false)
                fileSystem.WriteAllText(fileSystem.Combine(projectDir, "sergen.json"), config.SaveToJson());

            return ExitCodes.Success;
        }

        private IEnumerable<string> SelectWhatToGenerate()
        {
            var whatToGenerate = new List<string>
            {
                "Row",
                "Repository & Service",
                "User Interface",
                "Custom"
            };

            return ansiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[steelblue1]Choose What to Generate[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to select/unselect, " +
                        "[grey]up and down to navigate[/], " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoiceGroup<string>("All", whatToGenerate));
        }

        private string SelectPermissionKey(string table, string defaultPermissionKey)
        {
            ansiConsole.WriteLine();
            return ansiConsole.Prompt(
                new TextPrompt<string>($"Enter a Permission Key for table [springgreen3_1]{table}[/]")
                    .DefaultValue(defaultPermissionKey)
                    .Validate(module =>
                    {
                        if (string.IsNullOrEmpty(module))
                            return ValidationResult.Error("[red]Can not be empty[/]");

                        if (module.IndexOf(' ') > -1)
                            return ValidationResult.Error("[red]Can not contains space[/]");

                        return ValidationResult.Success();
                    }));
        }

        private string SelectIdentifier(string table, string defaultIdentifier)
        {
            ansiConsole.WriteLine();
            return ansiConsole.Prompt(
                new TextPrompt<string>($"Enter a class Identifier for table [springgreen3_1]{table}[/]")
                    .Validate(module =>
                    {
                        if (string.IsNullOrEmpty(module))
                            return ValidationResult.Error("[red]Can not be empty[/]");

                        if (module.IndexOf(' ') > -1)
                            return ValidationResult.Error("[red]Can not contains space[/]");

                        return ValidationResult.Success();
                    })
                    .DefaultValue(defaultIdentifier));
        }

        private string SelectModule(string table, string defaultModule)
        {
            ansiConsole.WriteLine();
            return ansiConsole.Prompt(
                new TextPrompt<string>($"Enter a Module name for table [springgreen3_1]{table}[/]")
                    .Validate(module =>
                    {
                        if (string.IsNullOrEmpty(module))
                            return ValidationResult.Error("[red]Can not be empty[/]");

                        if (module.IndexOf(' ') > -1)
                            return ValidationResult.Error("[red]Can not contains space[/]");

                        return ValidationResult.Success();
                    })
                    .DefaultValue(defaultModule));
        }

        private (IEnumerable<TableName> selectedTables, List<TableName> tableNames) SelectedTables(
            ISqlConnections sqlConnections, string connectionKey)
        {
            ISchemaProvider schemaProvider;
            List<TableName> tableNames;
            using (var connection = sqlConnections.NewByKey(connectionKey))
            {
                schemaProvider = SchemaHelper.GetSchemaProvider(connection.GetDialect().ServerType);
                tableNames = schemaProvider.GetTableNames(connection).ToList();
            }

            var tables = tableNames.Select(x => x.Tablename).ToList();

            var selectedTableNames = ansiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title("[steelblue1]Select tables for code generation (single/multiple)[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more tables)[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to select/unselect, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(tables));

            var selectedTables = tableNames.Where(s => selectedTableNames.Contains(s.Tablename));
            return (selectedTables, tableNames);
        }

        private (string connectionKey, ISqlConnections sqlConnections) SelectConnectionString(GeneratorConfig config, string csproj)
        {
            var projectDir = fileSystem.GetDirectoryName(csproj);
            var connectionStringOptions = new ConnectionStringOptions();
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
                ansiConsole.Write(new Markup($"[bold red]No connections in appsettings files or sergen.json![/]"));
                ansiConsole.WriteLine();
                return (null, null);
            }

            var connectionKeys = connectionStringOptions.Keys.OrderBy(x => x).ToArray();

            RegisterSqlProviders();

            var sqlConnections = new DefaultSqlConnections(
                new DefaultConnectionStrings(connectionStringOptions));

            ansiConsole.WriteLine();
            var selections = new SelectionPrompt<string>()
                    .Title("[steelblue1]Available Connections[/]")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more connections)[/]")
                    .AddChoices(connectionKeys);

            return (ansiConsole.Prompt(selections), sqlConnections);
        }

        private string SelectCsProj()
        {
            var csprojFiles = fileSystem.GetFiles(".", "*.csproj");
            if (csprojFiles.Length == 1)
                return csprojFiles.First();

            if (csprojFiles.Length == 0)
            {
                ansiConsole.Write(new Markup($"[bold red]Can't find a project file in current directory![/]"));
                ansiConsole.WriteLine();
                ansiConsole.Write(new Markup($"[bold red]Please run Sergen in a folder that contains the Asp.Net Core project.[/]"));
                ansiConsole.WriteLine();
                return null;
            }

            ansiConsole.WriteLine();
            ansiConsole.Write(new Spectre.Console.Rule($"[bold orange1]Please select an Asp.Net Core project file[/]")
            {
                Justification = Justify.Left
            });
            ansiConsole.WriteLine();
            var selections = new SelectionPrompt<string>()
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more project files)[/]")
                    .AddChoices(csprojFiles);

            return ansiConsole.Prompt(selections);
        }
    }
}
