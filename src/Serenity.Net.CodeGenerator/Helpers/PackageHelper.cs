using System;
using System.IO;

namespace Serenity.CodeGenerator
{
    public class PackageHelper
    {
        public string DeterminePackagesPath()
        {
            var packagesFolder = "/packages/".Replace('/', Path.DirectorySeparatorChar);
            var packagesDir = AppContext.BaseDirectory;
            var packagesIdx = packagesDir.IndexOf(packagesFolder, StringComparison.OrdinalIgnoreCase);

            if (packagesIdx < 0)
            {
                string userHomeDirectory = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(userHomeDirectory))
                    userHomeDirectory = Environment.GetEnvironmentVariable("USERPROFILE");

                packagesDir = Path.Combine(userHomeDirectory, ".nuget/packages/"
                    .Replace('/', Path.DirectorySeparatorChar));

                packagesIdx = packagesDir.IndexOf(packagesFolder, StringComparison.OrdinalIgnoreCase);
            }

            if (packagesIdx < 0)
            {
                Console.Error.WriteLine("Can't determine NuGet packages directory!");
                Environment.Exit(1);
            }

            packagesDir = packagesDir.Substring(0, packagesIdx + packagesFolder.Length);
            if (!Directory.Exists(packagesDir))
            {
                Console.Error.WriteLine("Can't determine NuGet packages directory!");
                Environment.Exit(1);
            }

            return packagesDir;
        }
    }
}