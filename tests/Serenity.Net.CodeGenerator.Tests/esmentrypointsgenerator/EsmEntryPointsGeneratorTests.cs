using Serenity.Reflection;

namespace Serenity.CodeGeneration;

public partial class EsmEntryPointsGeneratorTests
{
    private sealed record Options
    {
        public string ProjectDir { get; init; } = "/Repos/MyTest.Web/";
        public string RootNamespace { get; init; } = "MyTest";
        public string EsmAssetBasePath { get; init; } = "/esm";
        public bool FileScopedNamespaces { get; init; }
        public bool InternalAccess { get; init; }
        public bool OmitComments { get; init; } = true;
        public string[] EntryPoints { get; init; }
    }

    private static string Generate(params string[] files)
    {
        return Generate(new Options(), files);
    }

    private static string Generate(Options options, params string[] files)
    {
        var fileSystem = new MockFileSystem();
        foreach (var file in files)
            fileSystem.AddFile(options.ProjectDir + file, "");

        var generator = new EsmEntryPointsGenerator(fileSystem)
        {
            ProjectDir = options.ProjectDir,
            RootNamespace = options.RootNamespace,
            EsmAssetBasePath = options.EsmAssetBasePath,
            FileScopedNamespaces = options.FileScopedNamespaces,
            InternalAccess = options.InternalAccess,
            OmitComments = options.OmitComments
        };

        if (options.EntryPoints != null)
        {
            generator.EntryPoints.Clear();
            generator.EntryPoints.AddRange(options.EntryPoints);
        }

        return generator.Generate().ReplaceLineEndings();
    }
}
