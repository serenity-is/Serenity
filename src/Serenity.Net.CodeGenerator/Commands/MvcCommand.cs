using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator;

public class MvcCommand(IProjectFileInfo project, IGeneratorConsole console)
    : BaseGeneratorCommand(project, console)
{
    public override ExitCodes Run()
    {
        var projectDir = FileSystem.GetDirectoryName(FileSystem.GetFullPath(ProjectFile));
        var config = FileSystem.LoadGeneratorConfig(projectDir);

        config.MVC ??= new();

        var transformFor = FileSystem.GetFileNameWithoutExtension(ProjectFile);
        Console.WriteLine($"Transforming MVC for {transformFor}", ConsoleColor.Cyan);

        string[] stripViewPaths = config.MVC.StripViewPaths ?? [
            "Modules/",
            "Views/",
            FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/",
            "Areas/" + FileSystem.GetFileNameWithoutExtension(ProjectFile) + "/"
        ];

        var rootDir = projectDir + System.IO.Path.DirectorySeparatorChar;
        var searchViewPaths = (config.MVC.SearchViewPaths ??
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
            FileScopedNamespaces = config.FileScopedNamespaces == true,
            IsCSharp = true
        };

        var rootNamespace = config.GetRootNamespaceFor(Project);

        cw.AppendLine();
        var generator = new ViewPathsGenerator(FileSystem, stripViewPaths);

        var internalAccess = config?.MVC?.InternalAccess == true;

        cw.InNamespace(rootNamespace, () =>
        {
            cw.IndentedLine($"{(internalAccess ? "internal" : "public")} static partial class MVC");
            cw.InBrace(() =>
            {
                generator.GenerateViews(cw, files.Select(x => x[rootDir.Length..]).ToArray());
            });
        });

        var outDir = FileSystem.Combine(projectDir,
            PathHelper.ToPath(config.MVC.OutDir.TrimToNull() ?? "Imports/MVC"));

        var esmGenerator = new EsmEntryPointsGenerator();
        var esmAssetBasePath = Project.GetEsmAssetBasePath();
        if (!string.IsNullOrEmpty(esmAssetBasePath))
            esmGenerator.EsmAssetBasePath = esmAssetBasePath;

        if (config.TSBuild?.EntryPoints is IEnumerable<string> globs)
        {
            if (globs.FirstOrDefault() != "+")
                esmGenerator.EntryPoints.Clear();
            else
                globs = globs.Skip(1);
            esmGenerator.EntryPoints.AddRange(globs);
        }

        var esmCode = esmGenerator.Generate(FileSystem, projectDir, rootNamespace,
            fileScopedNamespace: config.FileScopedNamespaces == true,
            internalAccess: config.MVC.InternalAccess == true);

        MultipleOutputHelper.WriteFiles(FileSystem, Console, outDir,
        [
            ("MVC.cs", cw.ToString()),
            ("ESM.cs", esmCode)
        ], deleteExtraPattern: ["ESM.cs", "MVC.cs"], endOfLine: config.EndOfLine);

        return ExitCodes.Success;
    }
}