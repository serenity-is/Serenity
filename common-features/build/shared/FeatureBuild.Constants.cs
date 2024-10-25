#if IsFeatureBuild
using System.IO;

namespace Build;

public static partial class Shared
{
    public static string Src => Path.Combine(Root, "src");
    public static string PackageOutDir => Path.Combine(Root, "build", ".nupkg");
    public static string SerenityDir => Path.Combine(Root, "..", "Serenity");
    public static string NugetExePath => Path.Combine(SerenityDir, "build", "tools", "NuGet", "NuGet.exe");
    private static string SolutionFileBase => Path.Combine(Src, Path.GetFileName(Root));
    public static string SolutionFile => File.Exists(SolutionFileBase + ".slnf") ? SolutionFileBase + ".slnf" : SolutionFileBase + ".sln";
    public static string PackageBuildProps => Path.Combine(Root, "build", "Package.Build.props");
    public static string SerenityPackageBuildProps => Path.Combine(SerenityDir, "build", "Package.Build.props");
    public static string DirectoryBuildProps => Path.Combine(Src, "Directory.Build.props");
    public static bool IsPatch { get; set; } = false;
    public static string SerenityVersion { get; set; }
    public static string PackageVersion { get; set; }
    public static bool LocalPush { get; set; } = true;
    public const string SerenityNetWebPackage = "Serenity.Net.Web";

    public static string GetTarget(ArgumentReader arguments)
    {
        Shared.SerenityVersion = arguments.GetString(["serenity-version", "sv"]);
        Shared.PackageVersion = arguments.GetString(["version", "v"]);
        Shared.LocalPush = arguments.GetBoolean(["local-push", "localpush", "lp"]) ?? false;
        Shared.IsPatch = arguments.GetBoolean(["patch"]) ?? false;
        return arguments.GetCommand() ?? "pack";
    }
}
#endif