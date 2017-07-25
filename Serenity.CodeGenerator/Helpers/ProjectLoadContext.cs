using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
#if !NET46
using System.Runtime.Loader;
#endif

namespace Serenity.CodeGenerator
{
    public class ProjectLoadContext : AssemblyLoadContext
    {

        private static readonly string[] NativeLibraryExtensions;
        private static readonly string[] ManagedAssemblyExtensions = new[]
        {
            ".dll",
            ".ni.dll",
            ".exe",
            ".ni.exe"
        };

        static ProjectLoadContext()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                NativeLibraryExtensions = new[] { ".dll" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                NativeLibraryExtensions = new[] { ".dylib" };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                NativeLibraryExtensions = new[] { ".so" };
            }
            else
            {
                NativeLibraryExtensions = new string[0];
            }
        }

        private string csproj;
        private string assemblyPath;
        private string packagesPath;
        
        public ProjectLoadContext(string csproj, string assemblyPath)
        {
            this.csproj = csproj;
            this.assemblyPath = assemblyPath;
            this.packagesPath = new PackageHelper().DeterminePackagesPath();
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            try
            {
                var assembly = Assembly.Load(new AssemblyName(assemblyName.Name));
                if (assembly != null)
                {
                    return assembly;
                }
            }
            catch (FileNotFoundException)
            {
            }

            string path;
            if (SearchForLibrary(assemblyPath, ManagedAssemblyExtensions, assemblyName.Name, out path))
            {
                return LoadFromAssemblyPath(path);
            }

            var packageDir = Path.Combine(this.packagesPath, assemblyName.Name);
            if (!Directory.Exists(packageDir))
                return null;

            var version = assemblyName.Version.Major + "." + assemblyName.Version.Minor + "." + assemblyName.Version.Build;
            packageDir = Path.Combine(Path.Combine(packageDir, version), "lib");
            if (!Directory.Exists(packageDir))
                return null;

            var dirs = Directory.GetDirectories(packageDir).OrderByDescending(x => x);
            var best = dirs.FirstOrDefault(x => x.IndexOf("netstandard", StringComparison.OrdinalIgnoreCase) >= 0) ??
                dirs.FirstOrDefault(x => x.IndexOf("netcore", StringComparison.OrdinalIgnoreCase) >= 0) ??
                dirs.FirstOrDefault();

            if (best != null && SearchForLibrary(best, ManagedAssemblyExtensions, assemblyName.Name, out path))
                return LoadFromAssemblyPath(path);

            return null;
        }

        private bool SearchForLibrary(string where, string[] extensions, string name, out string path)
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(where, name + extension);
                if (File.Exists(candidate))
                {
                    path = candidate;
                    return true;
                }
            }

            path = null;
            return false;
        }
    }
}