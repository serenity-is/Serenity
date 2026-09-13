using Serenity.Reflection;

namespace Serenity.CodeGeneration;

public partial class ViewPathsGeneratorTests
{
    private sealed record Options
    {
        public bool AsNamespace { get; init; }
        public bool FileScopedNamespaces { get; init; }
        public string Modifiers { get; init; } = "public";
        public bool OmitComments { get; init; } = true;
        public string[] StripViewPaths { get; init; }
        public string RootNamespace { get; init; } = "MyTest";
    }

    private static string GenerateViews(params string[] files)
    {
        return GenerateViews(new Options(), files);
    }

    private static string GenerateViews(Options options, params string[] files)
    {
        var fileSystem = new MockFileSystem();
        foreach (var file in files)
            fileSystem.AddFile(file.Replace('\\', '/'), "");

        var cw = new CodeWriter
        {
            FileScopedNamespaces = options.FileScopedNamespaces,
            IsCSharp = true
        };

        var generator = new ViewPathsGenerator(fileSystem,
            options.StripViewPaths ?? ["Modules/", "Views/", "MyTest.Web/", "Areas/MyTest.Web/"])
        {
            OmitComments = options.OmitComments
        };

        cw.InNamespace(options.RootNamespace, () =>
        {
            if (options.AsNamespace)
            {
                generator.GenerateViews(cw, files, options.Modifiers);
            }
            else
            {
                cw.IndentedLine($"{options.Modifiers} static partial class MVC");
                cw.InBrace(() => generator.GenerateViews(cw, files));
            }
        });

        return cw.ToString().ReplaceLineEndings();
    }
}
