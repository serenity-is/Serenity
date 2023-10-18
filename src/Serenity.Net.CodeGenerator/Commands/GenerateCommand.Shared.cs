using System.Data.Common;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand
{
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

    private static ConnectionStringOptions ParseConnectionStringOptions(IFileSystem fileSystem,
        string csproj, GeneratorConfig config)
    {
        var projectDir = fileSystem.GetDirectoryName(csproj);
        var options = new ConnectionStringOptions();
        foreach (var x in config.Connections.Where(x => !string.IsNullOrEmpty(x.ConnectionString)))
        {
            options[x.Key] = new ConnectionStringEntry
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

                        options[data.Key] = data.Value;
                    }
            }
        }

        return options;
    }
}
