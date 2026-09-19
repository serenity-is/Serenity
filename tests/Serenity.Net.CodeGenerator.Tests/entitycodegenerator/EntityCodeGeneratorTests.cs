namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    private class MockProjectFileInfo(MockFileSystem fileSystem, string? nullable = null) : IProjectFileInfo
    {
        public string ProjectFile => "/app/My.Web.csproj";
        public IFileSystem FileSystem => fileSystem;

        public string[]? GetAssemblyList(string[]? configured) => throw new NotSupportedException();
        public string? GetAssemblyName() => throw new NotSupportedException();
        public string? GetEsmAssetBasePath() => throw new NotSupportedException();
        public IDictionary<string, string?> GetGlobalUsings() => throw new NotSupportedException();
        public string? GetNullable() => nullable;
        public string? GetOutDir() => throw new NotSupportedException();
        public string? GetRootNamespace() => throw new NotSupportedException();
        public string? GetTargetFramework() => throw new NotSupportedException();
    }

    private class MockGeneratedFileWriter : IGeneratedFileWriter
    {
        public MockGeneratedFileWriter(MockFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public MockFileSystem FileSystem { get; }
        public List<string> Files { get; } = [];

        public void WriteAllText(string targetFile, string contents)
        {
            FileSystem.CreateDirectory(FileSystem.GetDirectoryName(targetFile));
            Files.Add(targetFile.Replace('\\', '/'));
            FileSystem.WriteAllText(targetFile, contents);
        }
    }

    private static void AddFile(MockFileSystem fileSystem, string path, string content = "")
    {
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(PathHelper.ToPath(path)));
        fileSystem.WriteAllText(path, content);
    }

    private static EntityCodeGenerator Create(
        out MockFileSystem fileSystem,
        out Templates templates,
        out MockGeneratedFileWriter writer,
        out EntityModel model,
        out GeneratorConfig config)
    {
        fileSystem = new MockFileSystem();
        writer = new MockGeneratedFileWriter(fileSystem);
        model = new EntityModelFactory().Create(new CustomerEntityInputs());
        templates = new Templates();
        config = new GeneratorConfig()
        {
            RootNamespace = CustomerEntityInputs.TestNamespace,
            EndOfLine = "lf",
            GenerateRow = false,
            GenerateService = false,
            GenerateUI = false,
            GenerateCustom = false
        };
        return new EntityCodeGenerator(new MockProjectFileInfo(fileSystem), model, config, templates, writer);
    }

    public static readonly string[] SerenityNetWebGlobalUsings =
    [
        "Microsoft.AspNetCore.Mvc",
        "Microsoft.Extensions.Logging",
        "Microsoft.Extensions.Options",
        "Serenity",
        "Serenity.Abstractions",
        "Serenity.ComponentModel",
        "Serenity.Data",
        "Serenity.Data.Mapping",
        "Serenity.Extensions",
        "Serenity.Navigation",
        "Serenity.Services",
        "Serenity.Web",
        "System",
        "System.Collections.Generic",
        "System.ComponentModel",
        "System.Linq",
        "System.Text",
        "System.Text.RegularExpressions",
        "System.Threading",
        "System.Threading.Tasks"
    ];

    private static EntityCodeGenerator CreateDefaults(
        out MockFileSystem fileSystem,
        out Templates templates,
        out MockGeneratedFileWriter writer,
        out EntityModel model,
        string sergenJson,
        bool nullableRefTypes = false)
    {
        fileSystem = new MockFileSystem();
        AddFile(fileSystem, "/app/sergen.json", sergenJson);
        var config = fileSystem.LoadGeneratorConfig("/app");
        writer = new MockGeneratedFileWriter(fileSystem);
        templates = new Templates();

        var inputs = new CustomerEntityInputs
        {
            Config = config,
            NullableRefTypes = nullableRefTypes
        };
        foreach (var ns in SerenityNetWebGlobalUsings)
            inputs.GlobalUsings.Add(ns);

        model = new EntityModelFactory().Create(inputs);
        return new EntityCodeGenerator(new MockProjectFileInfo(fileSystem, nullableRefTypes ? "enable" : null),
            model, config, templates, writer);
    }
}
