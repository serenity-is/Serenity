using Microsoft.Data.Sqlite;

namespace Serenity.CodeGenerator;

public partial class GenerateCommandTests
{
    private const string projectDir = "/app";
    private const string projectFile = projectDir + "/My.Web.csproj";
    private const string csproj = "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>";

    private class MockProjectFileInfo(MockFileSystem fileSystem,
        IDictionary<string, string?>? globalUsings = null,
        string[]? assemblyList = null) : IProjectFileInfo
    {
        public string ProjectFile => GenerateCommandTests.projectFile;
        public IFileSystem FileSystem => fileSystem;
        public string[]? GetAssemblyList(string[]? configured) => assemblyList;
        public string? GetAssemblyName() => null;
        public string? GetEsmAssetBasePath() => null;
        public IDictionary<string, string?> GetGlobalUsings() => globalUsings ?? new Dictionary<string, string?>();
        public string? GetNullable() => null;
        public string? GetOutDir() => null;
        public string? GetRootNamespace() => "My";
        public string? GetTargetFramework() => "net8.0";
    }

    private static void AddFile(MockFileSystem fileSystem, string path, string content)
    {
        var directory = fileSystem.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            fileSystem.CreateDirectory(directory);
        fileSystem.WriteAllText(path, content);
    }

    private static GeneratorConfig AddConfig(MockFileSystem fileSystem, GeneratorConfig config)
    {
        AddFile(fileSystem, projectFile, csproj);
        AddFile(fileSystem, projectDir + "/sergen.json", config.SaveToJson());
        return config;
    }

    private static GenerateCommand CreateCommand(MockFileSystem fileSystem,
        MockGeneratorConsole console, string[] args,
        IDictionary<string, string?>? globalUsings = null,
        string[]? assemblyList = null)
    {
        return new GenerateCommand(new MockProjectFileInfo(fileSystem, globalUsings, assemblyList), console)
        {
            Arguments = new ArgumentReader(args)
        };
    }

    private sealed class MemorySqliteDb : IDisposable
    {
        private readonly SqliteConnection connection;
        private readonly ISqlDialect? originalDialect;

        public MemorySqliteDb(string table)
        {
            ConnectionString = "Data Source=SerenityCodeGen_" + Guid.NewGuid().ToString("N") +
                ";Mode=Memory;Cache=Shared";

            originalDialect = SqlSettings.SetLocalDialect(SqliteDialect.Instance);

            connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "CREATE TABLE [" + table + "] " +
                "(Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)";
            command.ExecuteNonQuery();
        }

        public string ConnectionString { get; }

        public void Dispose()
        {
            connection.Dispose();
            SqlSettings.SetLocalDialect(originalDialect);
        }
    }
}
