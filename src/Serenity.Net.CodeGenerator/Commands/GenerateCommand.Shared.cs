using System.Data.Common;

namespace Serenity.CodeGenerator;

public partial class GenerateCommand
{
    private static void UpdateConfigTableFor(EntityModelInputs inputs, 
        GeneratorConfig.Connection confConnection)
    {
        var tableName = string.IsNullOrEmpty(inputs.Schema) ? inputs.Table : (inputs.Schema + "." + inputs.Table);

        var confTable = confConnection?.Tables.FirstOrDefault(x => string.Compare(x.Tablename, tableName,
            StringComparison.OrdinalIgnoreCase) == 0);

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
                Tablename = string.IsNullOrEmpty(inputs.Schema) ? inputs.Table : (inputs.Schema + "." + inputs.Table)
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

    private static ConnectionStringOptions ParseConnectionStrings(IFileSystem fileSystem,
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

    private class AppSettingsFormat
    {
        public ConnectionStringOptions Data { get; set; }

        public AppSettingsFormat()
        {
            Data = [];
        }

        public class ConnectionInfo
        {
            public string ConnectionString { get; set; }
            public string ProviderName { get; set; }
        }
    }

    private void Error(string error)
    {
        Console.Error(error);
        Console.WriteLine();
    }

    [GeneratedRegex(@"\<TargetFramework\>.*netcoreapp.*\<\/TargetFramework\>", RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex Net5PlusRegex();
}
