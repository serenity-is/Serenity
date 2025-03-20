using Serenity.CodeGeneration;
using System.Diagnostics;

namespace Serenity.CodeGenerator;

public partial class DoctorCommand(IProjectFileInfo project, IGeneratorConsole console) 
    : BaseGeneratorCommand(project, console)
{
    static readonly Version RecommendedNodeVersion = new(20, 11, 0);
    static readonly Version RecommendedNpmVersion = new(10, 8, 2);

    static readonly (Version, Version)[] RecommendedTSBuildVersion = [
        (new(0, 0, 0), new(8, 6, 0)),
        (new(8, 7, 1), new(8, 7, 4)),
        (new(8, 8, 4), new(8, 8, 4))
    ];

    static readonly (Version, Version)[] RecommendedJsxDomVersion = [
        (new(0, 0, 0), new(8, 1, 4)),
        (new(8, 6, 4), new(8, 1, 5)),
        (new(8, 8, 4), new(8, 1, 6))
    ];

    public IArgumentReader Arguments { get; set; }
    public List<ExternalType> TsTypes { get; set; }

    void Info(string label, string text)
    { 
        Console.Write(label + ": ", ConsoleColor.Cyan);
        Console.WriteLine(text);
    }

    void Warning(string message)
    {
        Console.Write("WARNING: ", ConsoleColor.Yellow);
        Console.WriteLine(message, ConsoleColor.Yellow);
    }

    void Error(string message)
    {
        Console.Write("ERROR: ", ConsoleColor.Red);
        Console.WriteLine(message, ConsoleColor.Red);
    }

    public override ExitCodes Run()
    {
        var projectFile = System.IO.Path.GetFullPath(ProjectFile);

        Info("Project File", projectFile);

        CheckProjectFilename(System.IO.Path.GetFileName(projectFile));
        var projectDir = FileSystem.GetDirectoryName(projectFile);
        CheckProjectDirectory(projectDir);

        var config = FileSystem.LoadGeneratorConfig(projectDir);

        var sergenJson = FileSystem.Combine(projectDir, "sergen.json");
        if (!FileSystem.FileExists(sergenJson))
        {
            Warning($"sergen.json file not found at {projectDir}!");
        }
        else
        {
            Info("sergen.json Location", sergenJson);
            Info("sergen.json Contents", FileSystem.ReadAllText(sergenJson));
        }

        Info("Using Generator Config", JSON.StringifyIndented(config));

        if (string.IsNullOrEmpty(config.RootNamespace))
        {
            config.RootNamespace = config.GetRootNamespaceFor(Project);
            Info("Using Root Namespace (auto generated)", config.RootNamespace);
        }
        else
        {
            Info("Using Root Namespace (from sergen.json)", config.RootNamespace);
        }

        CheckRootNamespace(config.RootNamespace);
        var sergenVersion = typeof(DoctorCommand).Assembly.GetName().Version;
        Info("Sergen Version", sergenVersion.ToString());
        CheckSerenityVersion(projectFile, sergenVersion, out Version serenityVersion);

        if (serenityVersion.Major != sergenVersion.Major ||
            serenityVersion.Minor != sergenVersion.Minor ||
            serenityVersion.Build != sergenVersion.Build)
        {
            Error($"Serenity.Net.Web version ({serenityVersion}) does not match Sergen version ({sergenVersion})!" +
                $"Sergen and Serenity.Web versions should exactly match!");
        }

        CheckNodeAndNpmVersions();
        CheckPackageJson(projectDir, serenityVersion);
        
        return ExitCodes.Success;
    }

    void CheckProjectDirectory(string projectDirectory)
    {
        if (ContainsSpaceRegex().IsMatch(projectDirectory))
        {
            Warning("It is not recommended to have the project in a directory that contains spaces!");
        }

        if (projectDirectory.Contains('#'))
        {
            Error("Project directory must not include '#' (hash) character!");
        }

        if (projectDirectory.Contains(';'))
        {
            Error("Project directory must not include ';' (semicolon) character!");
        }
    }

    void CheckProjectFilename(string projectFilename)
    {
        if (!StartsWithCapitalRegex().IsMatch(projectFilename))
        {
            Error("Project filename should start with a capital letter!");
        }

        if (ContainsSpaceRegex().IsMatch(projectFilename))
        {
            Warning("Project filename should not contain spaces!");
        }

        if (!ValidProjectNameCharsRegex().IsMatch(projectFilename))
        {
            Warning("Project filename should only include letters, digits, underscore and dot characters!");
        }

        if (projectFilename.EndsWith('.'))
        {
            Warning("Project filename should not end with a DOT!");
        }

        if (projectFilename.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            Error("Project filename should not be 'Serenity'");
        }

        if (projectFilename.StartsWith("Serenity.", StringComparison.Ordinal))
        {
            Warning("Project filename should not start with 'Serenity.' prefix");
        }
    }

    void CheckRootNamespace(string rootNamespace)
    {
        if (!StartsWithCapitalRegex().IsMatch(rootNamespace))
        {
            Error("Root namespace should start with a capital letter!");
        }

        if (ContainsSpaceRegex().IsMatch(rootNamespace))
        {
            Error("Root namespace should not contain spaces!");
        }

        if (!ValidProjectNameCharsRegex().IsMatch(rootNamespace))
        {
            Error("Root namespace should only include letters, digits, underscore and dot characters!");
        }

        if (rootNamespace.EndsWith('.'))
        {
            Error("Root namespace should not end with a DOT!");
        }

        if (rootNamespace.Equals("Serenity", StringComparison.OrdinalIgnoreCase))
        {
            Warning("Root namespace not be 'Serenity'");
        }

        if (rootNamespace.StartsWith("Serenity.", StringComparison.Ordinal))
        {
            Warning("Root namespace should not start with 'Serenity.' prefix");
        }
    }

    void CheckSerenityVersion(string projectFile, Version sergenVersion, out Version serenityVersion)
    {
        serenityVersion = sergenVersion;
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"msbuild \"{projectFile}\" " +
                        "-getItem:PackageReference " +
                        "-getProperty:AssemblyName " +
                        "-getProperty:OutDir " +
                        "-getProperty:RootNamespace " +
                        "-getProperty:TargetFramework",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        ProjectMetadataJson metadata;
        try
        {
            string output;
            var process = new Process() { StartInfo = startInfo };
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            if (!process.WaitForExit(10000))
                output = null;

            output = (output ?? "").Trim();

            if (!output.StartsWith('{') ||
                !output.EndsWith('}'))
            {
                Error($"Unexpected output from MSBuild for project metadata: {output}");
                return;
            }

            metadata = JSON.ParseTolerant<ProjectMetadataJson>(output) ?? new();
        }
        catch (Exception ex)
        {
            Error($"Error while executing MSBuild to get project metadata: {ex.Message}");
            return;
        }

        if (metadata.Items?.PackageReference == null)
        {
            Error("Can't read package references from project file!");
            return;
        }

        var serenityWeb = metadata.Items.PackageReference.FirstOrDefault(x => x.Identity == "Serenity.Net.Web");
        if (serenityWeb == null)
        {
            Error($"Can't read Serenity.Net.Web package reference from project file, assuming Sergen version ({sergenVersion})!");
            return;
        }

        if (serenityWeb.Version == null || !Version.TryParse(serenityWeb.Version, out Version serenityWebVersion))
        {
            Error($"Can't parse Serenity.Net.Web package version ({serenityWeb.Version}) from project file, assuming Sergen version ({sergenVersion})!");
            return;
        }

        Info("Serenity.Net.Web Version", serenityWebVersion.ToString());

        foreach (var packageRef in metadata.Items.PackageReference)
        {
            if (packageRef.Identity.StartsWith("Serenity.", StringComparison.Ordinal) &&
                packageRef.Version != null &&
                Version.TryParse(packageRef.Version, out Version packageVersion) &&
                (packageVersion.Major != serenityVersion.Major ||
                 packageVersion.Minor != serenityVersion.Minor ||
                 packageVersion.Build != serenityVersion.Build))
            {
                Warning($"Serenity.Net.Web version ({serenityVersion}) " +
                    $"does not match version of the package {packageRef.Identity} ({packageVersion})!");
            }
        }

        serenityVersion = serenityWebVersion;
    }

    private static Version GetNodeOrNpmVersion(bool npm)
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "cmd",
                Arguments = "/c " + (npm ? "npm.cmd" : "node") + " --version",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        string output;
        try
        {
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            if (!process.WaitForExit(5000))
                output = null;
        }
        catch (Exception)
        {
            output = null;
        }

        output = (output ?? "").Trim();

        if (output.StartsWith("v", StringComparison.OrdinalIgnoreCase))
            output = output.Substring(1);

        if (Version.TryParse(output, out var version))
            return version;

        return new Version(0, 0);
    }

    private void CheckNodeAndNpmVersions()
    {
        var nodeVersion = GetNodeOrNpmVersion(npm: false);
        if (nodeVersion.Major == 0)
        {
            Error("NodeJS is not installed or not in PATH. Please install NodeJS.");
        } else if (nodeVersion < RecommendedNodeVersion)
        {
            Warning($"Your NodeJS version ({nodeVersion}) is not up to date." +
                $"Please install latest Node LTS version (at least {RecommendedNodeVersion}+).");
        }

        var npmVersion = GetNodeOrNpmVersion(npm: true);
        if (npmVersion.Major == 0)
        {
            Error("NPM is not installed or not in PATH. Please install NPM.");
        }
        else if (npmVersion < RecommendedNpmVersion)
        {
            Warning($"Your NPM version ({npmVersion}) is not up to date." +
                $"Please install latest NPM version (at least {RecommendedNpmVersion}+).");
        }
    }

    private void CheckPackageJson(string projectDir, Version serenityVersion)
    {
        var packageJsonPath = FileSystem.Combine(projectDir, "package.json");
        if (!FileSystem.FileExists(packageJsonPath))
        {
            Warning("package.json file not found at project directory!");
            return;
        }

        PackageJson packageJson;
        try
        {
            packageJson = JSON.ParseTolerant<PackageJson>(System.IO.File.ReadAllText(packageJsonPath).Trim());
        }
        catch (Exception ex)
        {
            Error($"Error reading package.json: {ex.Message}");
            return;
        }

        CheckTSBuildVersion(packageJson, serenityVersion);
        CheckJsxDomVersion(packageJson, serenityVersion);
    }

    void CheckTSBuildVersion(PackageJson packageJson, Version serenityVersion)
    { 
        if (packageJson.devDependencies?.TryGetValue("@serenity-is/tsbuild", out var versionStr) != true &&
            packageJson.dependencies?.TryGetValue("@serenity-is/tsbuild", out versionStr) != true)
        {
            Warning($"@serenity-is/tsbuild is not found in package.json devDependencies!");
            return;
        }
        
        if (!Version.TryParse(versionStr, out Version version))
        {
            Warning($"Can't parse @serenity-is/tsbuild dependency version from package.json!");
            return;
        }

        var recommendedVersion = RecommendedTSBuildVersion.LastOrDefault(x =>
            serenityVersion >= x.Item1).Item2;

        if (version != null && version < recommendedVersion)
        {
            Error($"@serenity-is/tsbuild version in package.json is {version}, " +
                $"please update to {recommendedVersion} for better support.");
        }
        else if (version != null && version > recommendedVersion)
        {
            Warning($"@serenity-is/tsbuild version in package.json is {version}, " +
                $"which is newer than the recommended version {recommendedVersion} for " +
                $"Serenity {serenityVersion}. Please check docs as it may include breaking changes.");
        }
        else
        {
            Info("@serenity-is/tsbuild Version", version.ToString());
        }
    }

    void CheckJsxDomVersion(PackageJson packageJson, Version serenityVersion)
    {
        if (packageJson.dependencies?.TryGetValue("jsx-dom", out var versionStr) != true &&
            packageJson.devDependencies?.TryGetValue("jsx-dom", out versionStr) != true)
        {
            Warning($"jsx-dom package not found in package.json dependencies!");
            return;
        }

        if (!Version.TryParse(versionStr, out Version version))
        {
            Warning($"Can't parse jsx-dom dependency version from package.json!");
            return;
        }

        var recommendedVersion = RecommendedJsxDomVersion.LastOrDefault(x =>
            serenityVersion >= x.Item1).Item2;

        if (version != null && version < recommendedVersion)
        {
            Error($"jsx-dom version in package.json is {version}, " +
                $"please update to {RecommendedJsxDomVersion} for better support.");
        }
        else if (version != null && version > recommendedVersion)
        {
            Warning($"The jsx-dom version in package.json is {version}, " +
                $"which is newer than the recommended version {recommendedVersion} " +
                $"for Serenity {serenityVersion}. Please check docs as it may include breaking changes.");
        }
        else
        {
            Info("jsx-dom Version", version.ToString());
        }
    }

    private class PackageJson
    {
#pragma warning disable IDE1006 // Naming Styles
        public Dictionary<string, string> dependencies { get; set; }
        public Dictionary<string, string> devDependencies { get; set; }
#pragma warning restore IDE1006 // Naming Styles

    }

    private class ProjectMetadataJson
    {
        public ProjectFileInfo.ProjectProperties Properties { get; set; }
        public ProjectItems Items { get; set; }
    }

    private class ProjectItems
    {
        public PackageReferenceItem[] PackageReference { get; set; }
    }

    private class PackageReferenceItem
    {
        public string Identity { get; set; }
        public string Version { get; set; }
    }

    [GeneratedRegex("^[A-Z]")]
    private static partial Regex StartsWithCapitalRegex();
    [GeneratedRegex(@"\s")]
    private static partial Regex ContainsSpaceRegex();
    [GeneratedRegex(@"^[A-Za-z0-9._]+$")]
    private static partial Regex ValidProjectNameCharsRegex();
}