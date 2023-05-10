namespace Serenity.CodeGenerator;

public class PackageHelper
{
    public static string DeterminePackagesPath(IGeneratorFileSystem fileSystem)
    {
        string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
        if (string.IsNullOrEmpty(userHomeDirectory))
            userHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");

        var packagesDir = fileSystem.Combine(userHomeDirectory, ".nuget", "packages");

        if (!fileSystem.DirectoryExists(packagesDir))
        {
            Console.Error.WriteLine("Can't determine NuGet packages directory!");
            return null;
        }

        return packagesDir;
    }
}