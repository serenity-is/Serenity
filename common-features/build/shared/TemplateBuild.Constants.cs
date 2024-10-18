#if IsTemplateBuild
using System.IO;

namespace Build;

public static partial class Shared
{
    public static string NugetExePath => Path.Combine(Root, "Serenity", "build", "tools", "NuGet", "NuGet.exe");
    public static string ProjectId => IsStartSharp ? "StartSharp" : "Serene";
    public static string ProjectName => ProjectId + ".Web";
    public static string ProjectFolder => Path.Combine(Root, "src", ProjectName);
    public static string ProjectFile => Path.Combine(ProjectFolder, ProjectName + ".csproj");
    public static string PackageJsonFile => Path.Combine(ProjectFolder, "package.json");
    public static string SergenJsonFile => Path.Combine(ProjectFolder, "sergen.json");
    public static string TemplateId => IsStartSharp ? "StartCore" : "SereneCore";
    public static string VSIXTemplateFolder => Path.Combine(Root, "vsix");
    public static string VSIXTemplateProject => Path.Combine(VSIXTemplateProject, ProjectId + ".VSIX.csproj");
    public static string VSIXOutputFolder => Path.Combine(VSIXTemplateFolder, "bin");
    public static string VSIXManifestFile => Path.Combine(VSIXTemplateFolder, "source.extension.vsixmanifest");
    public static string VSIXProjectTemplates => Path.Combine(VSIXTemplateFolder, "ProjectTemplates");
    public static string TemplatesPackage => IsStartSharp ? "Serenity.Pro.Templates" : "Serene.Templates";
    public static string TemplatesProject => Path.Combine(VSIXTemplateFolder, TemplatesPackage, TemplatesPackage + ".csproj");
    public static string TemporaryFilesRoot => Path.Combine(VSIXTemplateFolder, "obj");
    public static string PackagePatchFolder => Path.Combine(Root, ".vs", "package-json-patch");
    public static string PackageJsonCopy => Path.Combine(PackagePatchFolder, "package.json");
    public static string PackageJsonCopyLock => Path.Combine(PackagePatchFolder, "package-lock.json");
    public static string TemplateVersion { get; set; }
    public static string TemplateZipFolder => Path.Combine(TemporaryFilesRoot, ProjectId);
    public static string TemplateZipWebFolder => Path.Combine(TemporaryFilesRoot, ProjectId, ProjectName);
    public static string SerenityPackageBuildProps => Path.Combine(Root, "Serenity", "build", "Package.Build.props");
    public static bool IsPatch { get; set; } = false;
    public static bool LocalPush { get; set; } = false;

    public static string GetTarget(ArgumentReader arguments)
    {
        Shared.TemplateVersion = arguments.GetString(["version", "v"]);
        Shared.LocalPush = arguments.GetBoolean(["local-push", "localpush", "lp"]) ?? true;
        Shared.IsPatch = arguments.GetBoolean(["patch"]) ?? false;
        return arguments.GetCommand() ?? "vsix";
    }
}
#endif