using System.Diagnostics;
using System.Xml.Linq;

namespace Serenity.CodeGenerator;

public class ProjectFileInfo(IFileSystem fileSystem, string projectFile, 
    Func<string, string> getPropertyArgument = null,
    Action<string> onError = null) : IProjectFileInfo
{
    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly string projectFile = projectFile ?? throw new ArgumentNullException(nameof(projectFile));
    private string assemblyName;
    private ProjectProperties projectProperties;
    private string outDir;
    private string rootNamespace;
    private string targetFramework;

    public IFileSystem FileSystem => fileSystem;
    public string ProjectFile => projectFile;

    private static readonly char[] complexValueChars = [';', '$', '@'];

    /// <summary>
    /// Callback for tests to validate MSBuild execution arguments
    /// </summary>
    public Func<ProcessStartInfo, string> ExecuteMSBuild { get; set; }

    public string GetAssemblyName()
    {
        if (assemblyName is null)
        {
            if (getPropertyArgument?.Invoke("AssemblyName") is string s)
                assemblyName = s;
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.AssemblyName))
                    return assemblyName = projectProperties.AssemblyName;

                assemblyName ??= ExtractPropertyFrom(projectFile, g => 
                    g.Elements("AssemblyName").LastOrDefault());
            }
        }

        return string.IsNullOrEmpty(assemblyName) ? null : assemblyName;
    }

    public string GetOutDir()
    {
        if (outDir is null)
        {
            if ((getPropertyArgument?.Invoke("OutDir") ??
                 getPropertyArgument?.Invoke("OutputPath")) is string s)
                outDir = s;
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.OutDir))
                    return outDir = projectProperties.OutDir;

                outDir ??= ExtractPropertyFrom(projectFile, g => 
                    g.Elements("OutDir").LastOrDefault() ??
                    g.Elements("OutputPath").LastOrDefault());
            }

            if (!string.IsNullOrEmpty(outDir))
                outDir = fileSystem.Combine(fileSystem.GetDirectoryName(projectFile), outDir);
        }

        return string.IsNullOrEmpty(outDir) ? null : outDir;
    }

    public string GetRootNamespace()
    {
        if (rootNamespace is null)
        {
            if (getPropertyArgument?.Invoke("RootNamespace") is string s)
                rootNamespace = s;
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.RootNamespace))
                    return rootNamespace = projectProperties.RootNamespace;

                rootNamespace ??= ExtractPropertyFrom(projectFile, propertyGroups =>
                    propertyGroups.Elements("RootNamespace").LastOrDefault());
            }
        }

        return string.IsNullOrEmpty(rootNamespace) ? null : rootNamespace;
    }

    public string GetTargetFramework()
    {
        if (targetFramework is null)
        {
            if (getPropertyArgument?.Invoke("TargetFramework") is string s)
                targetFramework = s;
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.TargetFramework))
                    return targetFramework = projectProperties.TargetFramework;

                targetFramework ??= ExtractPropertyFrom(projectFile, groups =>
                    groups.Elements("TargetFramework").LastOrDefault() ??
                    groups.Descendants("TargetFrameworks").LastOrDefault()) ?? "";
            }
        }

        return string.IsNullOrEmpty(targetFramework) ? null : targetFramework;
    }

    private string ExtractPropertyFrom(string csproj, Func<IEnumerable<XElement>, XElement> extractor)
    {
        foreach (var root in EnumerateProjectAndDirectoryBuildProps(csproj))
        {
            var element = extractor(root.Elements("PropertyGroup"));

            if (element is null)
                continue;

            if (!string.IsNullOrEmpty(element.Attribute("Condition")?.Value) ||
                element.Value.IndexOfAny(complexValueChars) >= 0)
                return "";

            return element.Value.TrimToEmpty();
        }

        return null;
    }

    private IEnumerable<XElement> EnumerateProjectAndDirectoryBuildProps(string csproj)
    {
        ArgumentNullException.ThrowIfNull(fileSystem);

        var xe = XElement.Parse(fileSystem.ReadAllText(csproj));
        yield return xe;

        var dir = fileSystem.GetDirectoryName(csproj);
        while (!string.IsNullOrEmpty(dir) &&
            fileSystem.DirectoryExists(dir))
        {
            dir = fileSystem.GetFullPath(dir);
            var dirProps = fileSystem.Combine(dir, "Directory.Build.props");
            if (fileSystem.FileExists(dirProps))
                yield return XElement.Parse(fileSystem.ReadAllText(dirProps));

            dir = fileSystem.GetDirectoryName(dir);
        }
    }

    private class ProjectPropertiesJson
    {
        public ProjectProperties Properties { get; set; }
    }

    public class ProjectProperties
    {
        public string AssemblyName { get; set; }
        public string OutDir { get; set; }
        public string RootNamespace { get; set; }
        public string TargetFramework { get; set; }
    }

    private ProjectProperties GetProjectProperties()
    {
        // can't run dotnet in tests in an abstract file system
        // unless RunMSBuild callback is provided
        if ((ExecuteMSBuild is null && fileSystem is not PhysicalFileSystem) ||
            !fileSystem.FileExists(projectFile))
            return new();

        var configArg = getPropertyArgument?.Invoke("Configuration") is string configuration &&
            !string.IsNullOrEmpty(configuration) ? $"-property:Configuration={configuration} " : "";

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"msbuild \"{projectFile}\" {configArg}" +
                "-getProperty:AssemblyName " +
                "-getProperty:OutDir " +
                "-getProperty:RootNamespace " +
                "-getProperty:TargetFramework",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        try
        {
            string output;
            if (ExecuteMSBuild != null)
            {
                output = ExecuteMSBuild(startInfo);
            }
            else
            {
                var process = new Process() { StartInfo = startInfo };
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                if (!process.WaitForExit(10000))
                    output = null;
            }

            output = (output ?? "").Trim();

            if (output.StartsWith('{') &&
                output.EndsWith('}'))
                return JSON.ParseTolerant<ProjectPropertiesJson>(output)?.Properties ?? new();

            onError?.Invoke($"Unexpected output from MSBuild for project properties: {output}");
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error while executing MSBuild to get project properties: {ex.Message}");
        }

        return new();
    }

    public string[] GetAssemblyList(string[] configured)
    {
        ArgumentNullException.ThrowIfNull(onError);

        string projectFile = ProjectFile;

        if (configured == null || configured.Length == 0)
        {
            string outputDir = GetOutDir();
            string assemblyName = GetAssemblyName() ??
                FileSystem.ChangeExtension(fileSystem.GetFileName(projectFile), null);

            void couldNotFindError(string expectedPath)
            {
                onError(string.Format(CultureInfo.CurrentCulture,
                    "Couldn't find output file at {0}!" + Environment.NewLine +
                    "Make sure project is built successfully before running Sergen", expectedPath));
            }

            bool testCandidate(string path, out string outputPath)
            {
                outputPath = path + ".dll";
                if (fileSystem.FileExists(outputPath))
                    return true;

                if (fileSystem.FileExists(path + ".exe"))
                {
                    outputPath = path + ".exe";
                    return true;
                }

                return false;
            }

            if (!string.IsNullOrEmpty(outputDir) &&
                !string.IsNullOrEmpty(assemblyName))
            {
                if (!testCandidate(fileSystem.Combine(outputDir, assemblyName), out string outputPath))
                {
                    couldNotFindError(outputPath);
                    return null;
                }

                return [outputPath];
            }

            string targetFramework = GetTargetFramework();
            if (string.IsNullOrEmpty(targetFramework))
            {
                onError("Couldn't read TargetFramework from project file!");
                return null;
            }

            var debugExists = testCandidate(fileSystem.Combine(fileSystem.GetDirectoryName(ProjectFile),
                PathHelper.ToPath("bin/Debug/" + targetFramework + "/" + assemblyName)), out var debugPath);
            var releaseExists = testCandidate(fileSystem.Combine(fileSystem.GetDirectoryName(ProjectFile),
                PathHelper.ToPath("bin/Release/" + targetFramework + "/" + assemblyName)), out var releasePath);

            if (releaseExists &&
                (!debugExists || fileSystem.GetLastWriteTimeUtc(debugPath) < fileSystem.GetLastWriteTimeUtc(releasePath)))
                return [releasePath];

            if (debugExists)
                return [debugPath];

            couldNotFindError(debugPath);
            return null;
        }

        if (configured == null || configured.Length == 0)
        {
            onError("ServerTypings has no assemblies configured in sergen.json file!");
            return null;
        }

        var assemblyFiles = configured.ToArray();
        for (var i = 0; i < assemblyFiles.Length; i++)
        {
            var assemblyFile1 = PathHelper.ToUrl(fileSystem.GetFullPath(PathHelper.ToPath(assemblyFiles[i])));
            var binDebugIdx = assemblyFile1.IndexOf("/bin/Debug/", StringComparison.OrdinalIgnoreCase);
            string assemblyFile2 = assemblyFile1;
            if (binDebugIdx >= 0)
                assemblyFile2 = string.Concat(assemblyFile1[0..binDebugIdx], "/bin/Release/",
                    assemblyFile1[(binDebugIdx + "/bin/Release".Length)..]);

            assemblyFiles[i] = assemblyFile1;

            if (fileSystem.FileExists(assemblyFile1))
            {
                if (fileSystem.FileExists(assemblyFile2) &&
                    fileSystem.GetLastWriteTimeUtc(assemblyFile1) < fileSystem.GetLastWriteTimeUtc(assemblyFile2))
                    assemblyFiles[i] = assemblyFile2;
            }
            else if (fileSystem.FileExists(assemblyFile2))
                assemblyFiles[i] = assemblyFile2;
            else
            {
                onError(string.Format(CultureInfo.CurrentCulture, string.Format(CultureInfo.CurrentCulture,
                    "Assembly file '{0}' specified in sergen.json is not found! " +
                    "This might happen when project is not successfully built or file name doesn't match the output DLL." +
                    "Please check paths in sergen.json.", assemblyFile1)));
                return null;
            }
        }

        return assemblyFiles;
    }
}