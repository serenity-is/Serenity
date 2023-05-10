using System.Xml.Linq;

namespace Serenity.CodeGenerator;

public class ProjectFileHelper
{
    private static string TargetFrameworkExtractor(XElement xe)
    {
        var xtarget = xe.Descendants("TargetFramework").FirstOrDefault();

        if (xtarget == null || string.IsNullOrEmpty(xtarget.Value))
        {
            xtarget = xe.Descendants("TargetFrameworks").FirstOrDefault();
            if (xtarget == null ||
                string.IsNullOrEmpty(xtarget.Value) &&
                xtarget.Value.Contains(';', StringComparison.OrdinalIgnoreCase))
                return null;
        }

        return xtarget?.Value.TrimToNull();
    }

    public static string ExtractTargetFrameworkFrom(IGeneratorFileSystem fileSystem, string csproj)
    {
        return ExtractPropertyFrom(fileSystem, csproj, TargetFrameworkExtractor);
    }

    public static string ExtractAssemblyNameFrom(IGeneratorFileSystem fileSystem, string csproj)
    {
        return ExtractPropertyFrom(fileSystem, csproj, xe => 
            xe.Descendants("AssemblyName")
                .FirstOrDefault()?.Value.TrimToNull());
    }

    public static string ExtractPropertyFrom(IGeneratorFileSystem fileSystem, string csproj, Func<XElement, string> extractor)
    {
        foreach (var xe in EnumerateProjectAndDirectoryBuildProps(fileSystem, csproj))
        {
            var value = extractor(xe);
            if (value != null)
                return value;
        }

        return null;
    }

    public static IEnumerable<XElement> EnumerateProjectAndDirectoryBuildProps(IGeneratorFileSystem fileSystem,
        string csproj)
    {
        if (fileSystem is null)
            throw new ArgumentNullException(nameof(fileSystem));

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
}