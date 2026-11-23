using Serenity.CodeGenerator;

namespace Serenity.CodeGeneration;

/// <summary>
/// Writes the constants of the text keys of a project (TextKeys.g.cs), which is a part of the mvc command, as it runs
/// before the build and writes the C# that the build compiles. The keys, and their texts, come from the template of the
/// text resources of the project, e.g. "texts/resources/site.texts.__.json", which is what an application registers,
/// see <see cref="LocalTextResources"/> and <see cref="TextKeysWriter"/>, and is written by the json texts generator of
/// the application, e.g. <c>YourProject.Web json-texts</c>, see <see cref="Serenity.Localization.BaseJsonTextsGenerator"/>.
/// </summary>
/// <remarks>
/// It is written to the directory that the rest of the code of the mvc command is written to, e.g. "Imports/MVC", so
/// that the generated code of a project is in one place, which its project file leaves out of its sources as well.
/// A project that has no template of its texts, e.g. one that its json texts are not generated for, has no constants,
/// which is what it says, rather than an error: its texts are not written by this one.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="TextKeysGenerator"/> class.
/// </remarks>
/// <param name="fileSystem">The file system of the project.</param>
/// <param name="console">The console, which the step tells what it does, or could not do.</param>
public class TextKeysGenerator(IFileSystem fileSystem, IGeneratorConsole console)
{
    /// <summary>
    /// The name of the file that the constants are written to.
    /// </summary>
    public const string FileName = "TextKeys.g.cs";

    /// <summary>
    /// The name of the outermost class of the constants.
    /// </summary>
    public const string ClassName = "TextKeys";

    /// <summary>
    /// Gets or sets the directory of the project, which its text resources are under.
    /// </summary>
    public string? ProjectDir { get; set; }

    /// <summary>
    /// Gets or sets the directory that the file is written to, which is the one that the rest of the generated code of
    /// the mvc command is written to, i.e. "Imports/MVC" by default, or null for the directory of the project.
    /// </summary>
    public string? OutDir { get; set; }

    /// <summary>
    /// Gets or sets the namespace of the constants, which is the one that the rest of the generated code of the mvc
    /// command is in, e.g. the root namespace of the project or the "MVC" namespace under it.
    /// </summary>
    public string? RootNamespace { get; set; }

    /// <summary>
    /// Gets or sets whether the constants of the text keys are asked for, which is the TextKeys option of the mvc
    /// command: true is for a project that wants them, and gets a message when it has no template to write them from;
    /// null is for one that gets them if it has one, which is what the default is, and says nothing when it has none;
    /// and false is for one that does not want them at all.
    /// </summary>
    public bool? TextKeys { get; set; }

    /// <summary>
    /// Gets or sets the end of line of the project, e.g. "LF" or "CRLF", or null for the one of the platform.
    /// </summary>
    public string? EndOfLine { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the namespace of the file is written as a file scoped one, as the rest of
    /// the generated files of the project write it, unless the project says otherwise.
    /// </summary>
    public bool FileScopedNamespaces { get; set; } = true;

    /// <summary>
    /// Gets or sets the value of the nullable directive of the file, e.g. "enable", or null for none.
    /// </summary>
    public string? NullableDirective { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the class is made available as TextKeys in every file of the project,
    /// with a global using, which is the default, see the TextKeysAlias option of the mvc command.
    /// </summary>
    public bool UsingAlias { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the outermost class is written as an internal one, as the other classes
    /// that are generated for the project are, see the InternalAccess option of the mvc command.
    /// </summary>
    public bool InternalAccess { get; set; }

    /// <summary>
    /// Writes the constants of the text keys of the project, if it has any to write.
    /// </summary>
    public void Run()
    {
        ArgumentNullException.ThrowIfNull(fileSystem);
        ArgumentNullException.ThrowIfNull(console);

        if (string.IsNullOrEmpty(ProjectDir))
            throw new InvalidOperationException("The directory of the project is not set.");

        if (TextKeys == false)
        {
            // a project that turns the constants off does not keep the ones of a build that it had them in
            Write(null);
            return;
        }

        var resources = ReadResources(ProjectDir!);
        if (resources is null || resources.Count == 0)
        {
            // a project that has no template of its texts has no keys to write, which it is told when it asked for
            // them, while one that takes them if they are there, which is the default, says nothing
            if (TextKeys == true)
                console!.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.TextKeysSkipped, FileName));

            Write(null);
            return;
        }

        var result = TextKeysWriter.Write(resources.Keys, RootNamespace, ClassName, resources,
            FileScopedNamespaces, NullableDirective, InternalAccess, UsingAlias);
        foreach (var diagnostic in result.Diagnostics)
            console!.WriteLine(diagnostic.ToString(), ConsoleColor.Yellow);

        if (!Write(result.Source.Length == 0 ? null : result.Source))
            return;

        console!.WriteLine(string.Format(CultureInfo.CurrentCulture, Texts.TextKeysGenerated,
            result.ConstantCount, PathHelper.ToUrl(FileName)), ConsoleColor.Green);
    }

    /// <summary>
    /// Reads the templates of the text resources of the project, which it has one of for each of its text packages,
    /// e.g. "texts/resources/site.texts.__.json", or null when it has none.
    /// </summary>
    private IReadOnlyDictionary<string, string?>? ReadResources(string projectDir)
    {
        // the templates of the project are its own, under its texts/resources folder, and not the ones of the packages
        // that it uses, which are under node_modules as well, and are generated with the projects of those packages
        var dir = fileSystem.Combine(projectDir, "texts", "resources");
        if (!fileSystem.DirectoryExists(dir))
            return null;

        var files = fileSystem.GetFiles(dir, "*.texts.__.json");
        if (files is null || files.Length == 0)
            return null;

        return LocalTextResources.Merge(files
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(file => LocalTextResources.Read(fileSystem.ReadAllText(file))));
    }

    /// <summary>
    /// Writes the file of the constants, or removes it when there is nothing to write. It is written only when its
    /// content changed, as the command runs before every build, and a file that is written again, even with the same
    /// content, has a new timestamp, which makes the build compile every time. A file that is left behind when there is
    /// nothing to write is compiled as well, and carries the keys of a build that is gone, so it is removed, which is
    /// what the pattern of the stale files of the output folder is for.
    /// </summary>
    /// <param name="source">The source of the constants, or null when the project has none to write.</param>
    /// <returns>True when the file was written, i.e. it was not there, or its content changed.</returns>
    private bool Write(string? source)
    {
        var written = MultipleOutputHelper.WriteFiles(fileSystem, console!, OutDir ?? ProjectDir!,
            source is null ? [] : [(FileName, source)],
            deleteExtraPattern: [FileName], endOfLine: EndOfLine);

        return written.WrittenCount > 0;
    }
}
