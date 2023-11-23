using System.Diagnostics;
using System.Xml.Linq;

namespace Serenity.CodeGenerator;

public class ProjectFileInfo(IFileSystem fileSystem, string projectFile, Dictionary<string, string> props) : IProjectFileInfo
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
    private static readonly char[] propertySeps = [',', ';'];

    public string GetAssemblyName()
    {
        if (assemblyName is null)
        {
            if (props != null &&
                props.TryGetValue("AssemblyName", out string s))
            {
                assemblyName = s ?? "";
            }
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.AssemblyName))
                    return assemblyName = projectProperties.AssemblyName;

                assemblyName ??= ExtractPropertyFrom(projectFile, g => g.Elements("AssemblyName").LastOrDefault());
            }
        }

        return string.IsNullOrEmpty(assemblyName) ? null : assemblyName;
    }

    public string GetOutDir()
    {
        if (outDir is null)
        {
            if (props != null &&
                (props.TryGetValue("OutDir", out string s) ||
                 props.TryGetValue("OutputPath", out s)))
            {
                outDir = s ?? "";
            }
            else
            {
                projectProperties ??= GetProjectProperties();
                if (!string.IsNullOrEmpty(projectProperties.OutDir))
                    return outDir = projectProperties.OutDir;

                outDir ??= ExtractPropertyFrom(projectFile, groups => groups.Elements("OutDir").LastOrDefault() ??
                    groups.Elements("OutputPath").LastOrDefault());
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
            if (props != null &&
                props.TryGetValue("RootNamespace", out string s))
            {
                rootNamespace = s ?? "";
            }
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
            if (props != null &&
                props.TryGetValue("TargetFramework", out string s))
            {
                targetFramework = s ?? "";
            }
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
        if (fileSystem is not PhysicalFileSystem ||
            !fileSystem.FileExists(projectFile))
            return new();

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"msbuild \"{projectFile}\" " +
                    (props.TryGetValue("Configuration", out string configuration) &&
                     !string.IsNullOrEmpty(configuration) ? $"-property:Configuration={configuration} " : "") +
                    $"-getProperty:AssemblyName -getProperty:OutDir -getProperty:RootNamespace -getProperty:TargetFramework",
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        string output;
        try
        {
            process.Start();
            output = process.StandardOutput.ReadToEnd();
            if (!process.WaitForExit(10000))
                output = null;
        }
        catch (Exception)
        {
            return new();
        }

        output = (output ?? "").Trim();

        if (output.StartsWith('{') &&
            output.EndsWith('}'))
            return JSON.ParseTolerant<ProjectPropertiesJson>(output)?.Properties ?? new();

        return new();
    }

    public static IEnumerable<string> FilterPropertyParams(IEnumerable<string> args,
        out Dictionary<string, string> props)
    {
        var result = props = [];
        return args.Where((x, i) =>
        {
            if (x.StartsWith("-prop:", StringComparison.Ordinal))
            {
                foreach (var assignment in x[6..].Split(propertySeps, StringSplitOptions.RemoveEmptyEntries))
                {
                    var eq = assignment.IndexOf('=');
                    if (eq <= 0)
                    {
                        result[x] = null;
                        continue;
                    }

                    var propName = assignment[0..eq];
                    var propValue = assignment[(eq + 1)..];
                    result[propName] = propValue;
                }

                return false;
            }

            return true;
        });
    }
}