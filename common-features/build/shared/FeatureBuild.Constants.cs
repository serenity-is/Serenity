#if IsFeatureBuild
using System.IO;

namespace Build;

public static partial class Shared
{
    public static string Src => Path.Combine(Root, "src");
    public static string PackageOutDir => Path.Combine(Root, "build", ".nupkg");
    public static string NugetExePath => Path.Combine(Root, "..", "Serenity", "build", "tools", "NuGet", "NuGet.exe");
    public static string SolutionFile => Path.Combine(Src, Path.GetFileName(Root) + ".sln");
    public static string PackageBuildProps => Path.Combine(Root, "build", "Package.Build.props");
    public static string SerenityPackageBuildProps => Path.Combine(Root, "..", "Serenity", "build", "Package.Build.props");
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
        Shared.LocalPush = arguments.GetBoolean(["local-push", "localpush", "lp"]) ?? true;
        Shared.IsPatch = arguments.GetBoolean(["patch"]) ?? false;
        return arguments.GetCommand() ?? "pack";
    }
}
#endif