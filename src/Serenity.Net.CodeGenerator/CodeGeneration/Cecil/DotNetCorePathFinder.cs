using Mono.Cecil;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler;

public class DotNetCorePathFinder
{
    class DotNetCorePackageInfo
    {
        public readonly string Name;
        public readonly string Version;
        public readonly string Type;
        public readonly string Path;
        public readonly string[] RuntimeComponents;

        public DotNetCorePackageInfo(string fullName, string type, string path, string[] runtimeComponents)
        {
            var parts = fullName.Split('/');
            Name = parts[0];
            Version = parts[1];
            Type = type;
            Path = path;
            RuntimeComponents = runtimeComponents ?? Array.Empty<string>();
        }
    }

    static readonly string[] LookupPaths = new string[] {
         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"),
         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "sdk", "NuGetFallbackFolder"),
         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "sdk", "NuGetFallbackFolder")
    };

    static readonly string[] RuntimePacks = new[] {
        "Microsoft.NETCore.App",
        "Microsoft.WindowsDesktop.App",
        "Microsoft.AspNetCore.App",
        "Microsoft.AspNetCore.All"
    };

    readonly Dictionary<string, DotNetCorePackageInfo> packages;
    readonly ISet<string> packageBasePaths = new HashSet<string>(StringComparer.Ordinal);
    readonly string assemblyName;
    readonly string basePath;
    readonly Version version;
    readonly string dotnetBasePath = FindDotNetExeDirectory();
    readonly List<string> searchPaths = new();
    ILookup<string, string> packageLocationByName;

    public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkId, UniversalAssemblyResolver.TargetFrameworkIdentifier targetFramework, 
        Version version)
    {
        assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
        basePath = Path.GetDirectoryName(parentAssemblyFileName);

        searchPaths.Add(basePath);

        this.version = version;

        if (targetFramework == UniversalAssemblyResolver.TargetFrameworkIdentifier.NETStandard)
        {
            // .NET Standard 2.1 is implemented by .NET Core 3.0 or higher
            if (version.Major == 2 && version.Minor == 1)
            {
                this.version = new Version(3, 0, 0);
            }
        }

        var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
        if (File.Exists(depsJsonFileName))
            packages = LoadPackageInfos(depsJsonFileName, targetFrameworkId).ToDictionary(i => i.Name);

        var lookupPaths = LookupPaths.Where(x => Directory.Exists(x)).ToList();
        if (Directory.Exists("/usr/share/dotnet/sdk/NuGetFallbackFolder"))
            lookupPaths.Add("/usr/share/dotnet/sdk/NuGetFallbackFolder");

        if (Directory.Exists("/usr/local/share/dotnet/sdk/NuGetFallbackFolder"))
            lookupPaths.Add("/usr/local/share/dotnet/sdk/NuGetFallbackFolder");

        var runtimeConfigFileName = Path.Combine(basePath, $"{assemblyName}.runtimeconfig.dev.json");
        if (File.Exists(runtimeConfigFileName))
        {
            var runtimeConfig = JObject.Parse(File.ReadAllText(runtimeConfigFileName));
            var probingPaths = runtimeConfig["runtimeOptions"]?["additionalProbingPaths"];
            if (probingPaths != null)
            {
                foreach (var x in probingPaths.ToArray().OfType<JValue>().Select(x => x.ToString(CultureInfo.InvariantCulture)))
                    if (x.IndexOf('|', StringComparison.Ordinal) < 0 && lookupPaths.IndexOf(x) < 0 && Directory.Exists(x))
                        lookupPaths.Add(x);
            }
        }

        foreach (var path in lookupPaths)
        {
            foreach (var pk in packages)
            {
                foreach (var item in pk.Value.RuntimeComponents)
                {
                    var itemPath = Path.GetDirectoryName(item);
                    var fullPath = Path.Combine(path, pk.Value.Name, pk.Value.Version, itemPath);
                    if (Directory.Exists(fullPath))
                        packageBasePaths.Add(fullPath);
                    else
                    {
                        fullPath = Path.Combine(path, pk.Value.Name?.ToLowerInvariant(), pk.Value.Version, itemPath);
                        if (Directory.Exists(fullPath))
                            packageBasePaths.Add(fullPath);
                    }
                }
            }
        }
    }

    public void AddSearchDirectory(string path)
    {
        searchPaths.Add(path);
        packageLocationByName = null;
    }

    public void RemoveSearchDirectory(string path)
    {
        searchPaths.Remove(path);
        packageLocationByName = null;
    }

    private ILookup<string, string> PackageLocationByName
    {
        get
        {
            if (packageLocationByName != null)
                return packageLocationByName;

            var runtimeDirs = new List<string>();
            if (dotnetBasePath != null)
            {
                var basePaths = RuntimePacks.Select(pack => Path.Combine(dotnetBasePath, "shared", pack));
                foreach (var basePath in basePaths)
                {
                    if (!Directory.Exists(basePath))
                        continue;

                    var closestVersion = GetClosestVersionFolder(basePath, version);
                    var path = Path.Combine(basePath, closestVersion);

                    if (Directory.Exists(path))
                        runtimeDirs.Add(path);
                }
            }

            packageLocationByName = packageBasePaths.Concat(searchPaths).Concat(runtimeDirs).SelectMany(x =>
                Directory.GetFiles(x, "*.dll")
                    .Concat(Directory.GetFiles(x, "*.exe")))
                .ToLookup(x => Path.GetFileNameWithoutExtension(x), StringComparer.OrdinalIgnoreCase);

            return packageLocationByName;
        }
    }


    public string TryResolveDotNetCore(AssemblyNameReference name)
    {
        return PackageLocationByName[name.Name].FirstOrDefault();
    }

    internal string GetReferenceAssemblyPath(string targetFramework)
    {
        var tfi = UniversalAssemblyResolver.ParseTargetFramework(targetFramework, out var version);
        string identifier, identifierExt;
        switch (tfi)
        {
            case UniversalAssemblyResolver.TargetFrameworkIdentifier.NETCoreApp:
                identifier = "Microsoft.NETCore.App";
                identifierExt = "netcoreapp" + version.Major + "." + version.Minor;
                break;
            case UniversalAssemblyResolver.TargetFrameworkIdentifier.NETStandard:
                identifier = "NETStandard.Library";
                identifierExt = "netstandard" + version.Major + "." + version.Minor;
                break;
            default:
                throw new NotSupportedException();
        }
        return Path.Combine(dotnetBasePath, "packs", identifier + ".Ref", version.ToString(), "ref", identifierExt);
    }

    static IEnumerable<DotNetCorePackageInfo> LoadPackageInfos(string depsJsonFileName, string targetFramework)
    {
        var dependencies = JObject.Parse(File.ReadAllText(depsJsonFileName));
        var runtimeInfos = dependencies["targets"][targetFramework].Children().OfType<JProperty>().ToArray();
        var libraries = dependencies["libraries"].Children().OfType<JProperty>().ToArray();

        foreach (var library in libraries)
        {
            var type = library.First()["type"].ToString();
            var path = library.First()["path"]?.ToString();
            var rti = runtimeInfos.FirstOrDefault(r => r.Name == library.Name)?.First();
            var runtimeInfo = (rti?["runtime"]?.Children().OfType<JProperty>().Select(i => i.Name) ?? Array.Empty<string>())
                .Concat(rti?["compile"]?.Children().OfType<JProperty>().Select(i => i.Name) ?? Array.Empty<string>())
                .ToArray();

            yield return new DotNetCorePackageInfo(library.Name, type, path, runtimeInfo);
        }
    }

    static string GetClosestVersionFolder(string basePath, Version version)
    {
        string result = null;
        foreach (var folder in new DirectoryInfo(basePath).GetDirectories()
            .Select(d => ConvertToVersion(d.Name)).Where(v => v.Item1 != null).OrderByDescending(v => v.Item1))
        {
            if (folder.Item1 >= version)
                result = folder.Item2;
        }
        return result ?? version.ToString();
    }

    internal static Tuple<Version, string> ConvertToVersion(string name)
    {
        try
        {
            return new Tuple<Version, string>(new Version(RemoveTrailingVersionInfo(name)), name);
        }
        catch (Exception ex)
        {
            Trace.TraceWarning(ex.ToString());
            return new Tuple<Version, string>(null, null);
        }
    }

    static string RemoveTrailingVersionInfo(string name)
    {
        string shortName = name;
        int dashIndex = shortName.IndexOf('-', StringComparison.Ordinal);
        if (dashIndex > 0)
        {
            shortName = shortName.Remove(dashIndex);
        }
        return shortName;
    }

    static string FindDotNetExeDirectory()
    {
        string dotnetExeName = (Environment.OSVersion.Platform == PlatformID.Unix) ? "dotnet" : "dotnet.exe";
        foreach (var item in Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator))
        {
            try
            {
                string fileName = Path.Combine(item, dotnetExeName);
                if (!File.Exists(fileName))
                    continue;
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    if ((new FileInfo(fileName).Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        var sb = new StringBuilder();
                        realpath(fileName, sb);
                        fileName = sb.ToString();
                        if (!File.Exists(fileName))
                            continue;
                    }
                }
                return Path.GetDirectoryName(fileName);
            }
            catch (ArgumentException) { }
        }
        return null;
    }

#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
    [DllImport("libc")]
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments
    static extern void realpath(string path, StringBuilder resolvedPath);

    public static string DetectTargetFrameworkId(AssemblyDefinition assembly)
    {
        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";

        foreach (var attribute in assembly.CustomAttributes)
        {
            if (attribute.AttributeType.FullName != TargetFrameworkAttributeName)
                continue;
            var buffer = attribute.GetBlob();
            if (buffer.Length > 2 &&
                buffer[0] == 1 &&
                buffer[1] == 0)
                return ReadSerString(buffer, 2);
        }

        return string.Empty;
    }

    public static string ReadSerString(byte[] buffer, int position)
    {
        if (buffer[position] == 0xff)
            return null;

        int length = (int)ReadCompressedUInt32(buffer, ref position);
        if (length == 0)
            return string.Empty;

        string str = Encoding.UTF8.GetString(
            buffer, position,
            buffer[position + length - 1] == 0 ? length - 1 : length);

        return str;
    }

    public static uint ReadCompressedUInt32(byte[] buffer, ref int position)
    {
        unchecked
        {
            byte first = buffer[position++];
            if ((first & 0x80) == 0)
                return first;

            if ((first & 0x40) == 0)
                return ((uint)(first & ~0x80) << 8)
                    | buffer[position++];

            return ((uint)(first & ~0xc0) << 24)
                | (uint)buffer[position++] << 16
                | (uint)buffer[position++] << 8
                | buffer[position++];
        }
    }
}