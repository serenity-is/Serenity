using System;
using System.IO.Abstractions;

namespace Serenity.CodeGenerator
{
    public class PackageHelper
    {
        public static string DeterminePackagesPath(IFileSystem fileSystem)
        {
            string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrEmpty(userHomeDirectory))
                userHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");

            var packagesDir = fileSystem.Path.Combine(userHomeDirectory, ".nuget", "packages");

            if (!fileSystem.Directory.Exists(packagesDir))
            {
                Console.Error.WriteLine("Can't determine NuGet packages directory!");
                return null;
            }

            return packagesDir;
        }
    }
}