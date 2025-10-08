using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class MvcCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(FileSystem.GetFullPath(ProjectFile));
        var sergenConfig = FileSystem.LoadGeneratorConfig(projectDir);

        sergenConfig.MVC ??= new();

        var transformFor = FileSystem.GetFileNameWithoutExtension(ProjectFile);
        Console.WriteLine($"Transforming MVC for {transformFor}", ConsoleColor.Cyan);

        string[] stripViewPaths = sergenConfig.MVC.StripViewPaths ?? [
            "Modules/",
            "Views/",
            FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/",
            "Areas/" + FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/"
        ];

        var rootDir = projectDir + System.IO.Path.DirectorySeparatorChar;
        var searchViewPaths = (sergenConfig.MVC.SearchViewPaths ??
            [
                "Modules/",
                "Views/",
                FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/",
                "Areas/" + FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/"
            ])
            .Select(x => FileSystem.Combine(rootDir, PathHelper.ToPath(x)));

        IEnumerable<string> files = [];
        foreach (var path in searchViewPaths)
        {
            if (FileSystem.DirectoryExists(path))
                files = files.Concat(FileSystem.GetFiles(path, "*.cshtml", recursive: true));
        }

        var cw = new CodeWriter()
        {
            FileScopedNamespaces = sergenConfig.FileScopedNamespaces == true,
            IsCSharp = true
        };

        var rootNamespace = sergenConfig.GetRootNamespaceFor(Project);
        var asNamespace = sergenConfig.MVC.AsNamespace == true;
        var helperNamespace = asNamespace ? (rootNamespace + ".MVC") : rootNamespace;

        cw.AppendLine();
        var generator = new ViewPathsGenerator(FileSystem, stripViewPaths);

        var internalAccess = sergenConfig?.MVC?.InternalAccess == true;
        var modifiers = (internalAccess ? "internal" : "public");
        var relativeFiles = files.Select(x => x[rootDir.Length..]).ToArray();

        cw.InNamespace(helperNamespace, () =>
        {
            if (asNamespace)
            {
                generator.GenerateViews(cw, relativeFiles, modifiers);
            }
            else
            {
                cw.IndentedLine($"{modifiers} static partial class MVC");
                cw.InBrace(() =>
                {
                    generator.GenerateViews(cw, relativeFiles);
                });
            }
        });

        var outDir = FileSystem.Combine(projectDir,
            PathHelper.ToPath(sergenConfig.MVC.OutDir.TrimToNull() ?? "Imports/MVC"));

        var esmGenerator = new EsmEntryPointsGenerator();
        var esmAssetBasePath = Project.GetEsmAssetBasePath();
        if (!string.IsNullOrEmpty(esmAssetBasePath))
            esmGenerator.EsmAssetBasePath = esmAssetBasePath;

        if (sergenConfig.TSBuild?.EntryPoints is IEnumerable<string> globs)
        {
            if (globs.FirstOrDefault() != "+")
                esmGenerator.EntryPoints.Clear();
            else
                globs = globs.Skip(1);
            esmGenerator.EntryPoints.AddRange(globs);
        }

        var esmCode = esmGenerator.Generate(FileSystem, projectDir, helperNamespace,
            fileScopedNamespace: sergenConfig.FileScopedNamespaces == true,
            internalAccess: sergenConfig.MVC.InternalAccess == true);

        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir,
        [
            ("MVC.cs", cw.ToString()),
            ("ESM.cs", esmCode)
        ], deleteExtraPattern: ["ESM.cs", "MVC.cs"], endOfLine: sergenConfig.EndOfLine);

        return ExitCodes.Success;
    }
}