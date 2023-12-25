namespace Serenity.CodeGenerator;

public class PackageHelper
{
    public static string DeterminePackagesPath(IFileSystem fileSystem, IGeneratorConsole console)
    {
        string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(userHomeDirectory))
            userHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");

        var packagesDir = fileSystem.Combine(userHomeDirectory, ".nuget", "packages");

        if (!fileSystem.DirectoryExists(packagesDir))
        {
            console.Error("Can't determine NuGet packages directory!");
            return null;
        }

        return packagesDir;
    }
}