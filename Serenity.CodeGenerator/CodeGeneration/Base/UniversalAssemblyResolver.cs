﻿// Copyright (c) 2018 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ICSharpCode.Decompiler
{
    // This inspired by Mono.Cecil's BaseAssemblyResolver/DefaultAssemblyResolver.
    public class UniversalAssemblyResolver : IAssemblyResolver
    {
        DotNetCorePathFinder dotNetCorePathFinder;
        readonly bool throwOnError;
        readonly string mainAssemblyFileName;
        readonly string baseDirectory;
        readonly List<string> directories = new List<string>();
        readonly List<string> gac_paths = GetGacPaths();
        readonly Dictionary<AssemblyNameReference, AssemblyDefinition> cache = new Dictionary<AssemblyNameReference, AssemblyDefinition>();
        HashSet<string> targetFrameworkSearchPaths;

        /// <summary>
        /// Detect whether we're in a Mono environment.
        /// </summary>
        /// <remarks>This is used whenever we're trying to decompile a plain old .NET framework assembly on Unix.</remarks>
        static bool DetectMono()
        {
            // TODO : test whether this works with Mono on *Windows*, not sure if we'll
            // ever need this...
            if (Type.GetType("Mono.Runtime") != null)
                return true;
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                return true;
            return false;
        }

        public void AddSearchDirectory(string directory)
        {
            directories.Add(directory);
        }

        public void RemoveSearchDirectory(string directory)
        {
            directories.Remove(directory);
        }

        public string[] GetSearchDirectories()
        {
            return directories.ToArray();
        }

        enum TargetFrameworkIdentifier
        {
            NETFramework,
            NETCoreApp,
            NETStandard,
            Silverlight
        }

        string targetFramework;
        TargetFrameworkIdentifier targetFrameworkIdentifier;
        Version targetFrameworkVersion;

        public UniversalAssemblyResolver(string mainAssemblyFileName, bool throwOnError, string targetFramework)
        {
            this.targetFramework = targetFramework ?? string.Empty;
            targetFrameworkIdentifier = ParseTargetFramework(this.targetFramework, out targetFrameworkVersion);
            this.mainAssemblyFileName = mainAssemblyFileName;
            this.baseDirectory = Path.GetDirectoryName(mainAssemblyFileName);
            this.throwOnError = throwOnError;
            if (string.IsNullOrWhiteSpace(this.baseDirectory))
                this.baseDirectory = Environment.CurrentDirectory;
            AddSearchDirectory(baseDirectory);
        }

        TargetFrameworkIdentifier ParseTargetFramework(string targetFramework, out Version version)
        {
            string[] tokens = targetFramework.Split(',');
            TargetFrameworkIdentifier identifier;

            switch (tokens[0].Trim().ToUpperInvariant())
            {
                case ".NETCOREAPP":
                    identifier = TargetFrameworkIdentifier.NETCoreApp;
                    break;
                case ".NETSTANDARD":
                    identifier = TargetFrameworkIdentifier.NETStandard;
                    break;
                case "SILVERLIGHT":
                    identifier = TargetFrameworkIdentifier.Silverlight;
                    break;
                default:
                    identifier = TargetFrameworkIdentifier.NETFramework;
                    break;
            }

            version = null;

            for (int i = 1; i < tokens.Length; i++)
            {
                var pair = tokens[i].Trim().Split('=');

                if (pair.Length != 2)
                    continue;

                switch (pair[0].Trim().ToUpperInvariant())
                {
                    case "VERSION":
                        var versionString = pair[1].TrimStart('v');
                        if (identifier == TargetFrameworkIdentifier.NETCoreApp ||
                            identifier == TargetFrameworkIdentifier.NETStandard)
                        {
                            if (versionString.Length == 3)
                                versionString += ".0";
                        }
                        if (!Version.TryParse(versionString, out version))
                            version = null;
                        break;
                }
            }
            version = version ?? ZeroVersion;
            return identifier;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            AssemblyDefinition ad;
            if (cache.TryGetValue(name, out ad))
                return ad;

            var result = Resolve(name, new ReaderParameters());
            cache[name] = result;
            return result;
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var file = FindAssemblyFile(name);
            if (file == null)
            {
                if (throwOnError)
                    throw new AssemblyResolutionException(name);
                return null;
            }

            return GetAssembly(file, parameters);
        }

        AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
        {
            if (parameters.AssemblyResolver == null)
                parameters.AssemblyResolver = this;

            return ModuleDefinition.ReadModule(file, parameters).Assembly;
        }

        public string FindAssemblyFile(AssemblyNameReference name)
        {
            if (name.IsWindowsRuntime)
            {
                return FindWindowsMetadataFile(name);
            }

            string file = null;
            switch (targetFrameworkIdentifier)
            {
                case TargetFrameworkIdentifier.NETCoreApp:
                case TargetFrameworkIdentifier.NETStandard:
                    if (IsZeroOrAllOnes(targetFrameworkVersion))
                        goto default;
                    if (dotNetCorePathFinder == null)
                    {
                        dotNetCorePathFinder = new DotNetCorePathFinder(mainAssemblyFileName, targetFramework, targetFrameworkVersion);
                    }
                    file = dotNetCorePathFinder.TryResolveDotNetCore(name);
                    if (file != null)
                        return file;
                    goto default;
                case TargetFrameworkIdentifier.Silverlight:
                    if (IsZeroOrAllOnes(targetFrameworkVersion))
                        goto default;
                    file = ResolveSilverlight(name, targetFrameworkVersion);
                    if (file != null)
                        return file;
                    goto default;
                default:
                    return ResolveInternal(name);
            }
        }

        string FindWindowsMetadataFile(AssemblyNameReference name)
        {
            // Finding Windows Metadata (winmd) is currently only supported on Windows.
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return null;

            // TODO : Find a way to detect the base directory for the required Windows SDK.
            string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Windows Kits", "10", "References");

            if (!Directory.Exists(basePath))
                return FindWindowsMetadataInSystemDirectory(name);

            // TODO : Find a way to detect the required Windows SDK version.
            var di = new DirectoryInfo(basePath);
            basePath = null;
            foreach (var versionFolder in di.EnumerateDirectories())
            {
                basePath = versionFolder.FullName;
            }

            if (basePath == null)
                return FindWindowsMetadataInSystemDirectory(name);

            basePath = Path.Combine(basePath, name.Name);

            if (!Directory.Exists(basePath))
                return FindWindowsMetadataInSystemDirectory(name);

            basePath = Path.Combine(basePath, FindClosestVersionDirectory(basePath, name.Version));

            if (!Directory.Exists(basePath))
                return FindWindowsMetadataInSystemDirectory(name);

            string file = Path.Combine(basePath, name.Name + ".winmd");

            if (!File.Exists(file))
                return FindWindowsMetadataInSystemDirectory(name);

            return file;
        }

        string FindWindowsMetadataInSystemDirectory(AssemblyNameReference name)
        {
            string file = Path.Combine(Environment.SystemDirectory, "WinMetadata", name.Name + ".winmd");
            if (File.Exists(file))
                return file;
            return null;
        }

        void AddTargetFrameworkSearchPathIfExists(string path)
        {
            if (targetFrameworkSearchPaths == null)
            {
                targetFrameworkSearchPaths = new HashSet<string>();
            }
            if (Directory.Exists(path))
                targetFrameworkSearchPaths.Add(path);
        }

        /// <summary>
        /// This only works on Windows
        /// </summary>
        string ResolveSilverlight(AssemblyNameReference name, Version version)
        {
            AddTargetFrameworkSearchPathIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Silverlight"));
            AddTargetFrameworkSearchPathIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Silverlight"));

            foreach (var baseDirectory in targetFrameworkSearchPaths)
            {
                var versionDirectory = Path.Combine(baseDirectory, FindClosestVersionDirectory(baseDirectory, version));
                var file = SearchDirectory(name, versionDirectory);
                if (file != null)
                    return file;
            }
            return null;
        }

        string FindClosestVersionDirectory(string basePath, Version version)
        {
            string path = null;
            foreach (var folder in new DirectoryInfo(basePath).GetDirectories().Select(d => DotNetCorePathFinder.ConvertToVersion(d.Name))
                .Where(v => v.Item1 != null).OrderByDescending(v => v.Item1))
            {
                if (path == null || folder.Item1 >= version)
                    path = folder.Item2;
            }
            return path ?? version.ToString();
        }

        string ResolveInternal(AssemblyNameReference name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var assembly = SearchDirectory(name, directories);
            if (assembly != null)
                return assembly;

            var framework_dir = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
            var framework_dirs = DetectMono()
                ? new[] { framework_dir, Path.Combine(framework_dir, "Facades") }
                : new[] { framework_dir };

            if (IsSpecialVersionOrRetargetable(name))
            {
                assembly = SearchDirectory(name, framework_dirs);
                if (assembly != null)
                    return assembly;
            }

            if (name.Name == "mscorlib")
            {
                assembly = GetCorlib(name);
                if (assembly != null)
                    return assembly;
            }

            assembly = GetAssemblyInGac(name);
            if (assembly != null)
                return assembly;

            assembly = SearchDirectory(name, framework_dirs);
            if (assembly != null)
                return assembly;

            if (throwOnError)
                throw new AssemblyResolutionException(name);
            return null;
        }

        #region .NET / mono GAC handling
        string SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                string file = SearchDirectory(name, directory);
                if (file != null)
                    return file;
            }

            return null;
        }

        static bool IsSpecialVersionOrRetargetable(AssemblyNameReference reference)
        {
            return IsZeroOrAllOnes(reference.Version) || reference.IsRetargetable;
        }

        string SearchDirectory(AssemblyNameReference name, string directory)
        {
            var extensions = name.IsWindowsRuntime ? new[] { ".winmd", ".dll" } : new[] { ".exe", ".dll" };
            foreach (var extension in extensions)
            {
                string file = Path.Combine(directory, name.Name + extension);
                if (!File.Exists(file))
                    continue;
                try
                {
                    return file;
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
            }
            return null;
        }

        static bool IsZeroOrAllOnes(Version version)
        {
            return version == null
                || (version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0)
                || (version.Major == 65535 && version.Minor == 65535 && version.Build == 65535 && version.Revision == 65535);
        }

        internal static Version ZeroVersion = new Version(0, 0, 0, 0);

        string GetCorlib(AssemblyNameReference reference)
        {
            var version = reference.Version;
            var corlib = typeof(object).Assembly.GetName();

            if (corlib.Version == version || IsSpecialVersionOrRetargetable(reference))
                return typeof(object).Module.FullyQualifiedName;

            string path;
            if (DetectMono())
            {
                path = GetMonoMscorlibBasePath(version);
            }
            else
            {
                path = GetMscorlibBasePath(version);
            }

            if (path == null)
                return null;

            var file = Path.Combine(path, "mscorlib.dll");
            if (File.Exists(file))
                return file;

            return null;
        }

        string GetMscorlibBasePath(Version version)
        {
            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET");
            string[] frameworkPaths = new[] {
                Path.Combine(rootPath, "Framework"),
                Path.Combine(rootPath, "Framework64")
            };

            string folder = GetSubFolderForVersion();

            foreach (var path in frameworkPaths)
            {
                var basePath = Path.Combine(path, folder);
                if (Directory.Exists(basePath))
                    return basePath;
            }

            if (throwOnError)
                throw new NotSupportedException("Version not supported: " + version);
            return null;

            string GetSubFolderForVersion()
            {
                switch (version.Major)
                {
                    case 1:
                        if (version.MajorRevision == 3300)
                            return "v1.0.3705";
                        return "v1.1.4322";
                    case 2:
                        return "v2.0.50727";
                    case 4:
                        return "v4.0.30319";
                    default:
                        if (throwOnError)
                            throw new NotSupportedException("Version not supported: " + version);
                        return null;
                }
            }
        }

        string GetMonoMscorlibBasePath(Version version)
        {
            var path = Directory.GetParent(typeof(object).Module.FullyQualifiedName).Parent.FullName;
            if (version.Major == 1)
                path = Path.Combine(path, "1.0");
            else if (version.Major == 2)
            {
                if (version.MajorRevision == 5)
                    path = Path.Combine(path, "2.1");
                else
                    path = Path.Combine(path, "2.0");
            }
            else if (version.Major == 4)
                path = Path.Combine(path, "4.0");
            else
            {
                if (throwOnError)
                    throw new NotSupportedException("Version not supported: " + version);
                return null;
            }
            return path;
        }

        static List<string> GetGacPaths()
        {
            if (DetectMono())
                return GetDefaultMonoGacPaths();

            var paths = new List<string>(2);
            var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (windir == null)
                return paths;

            paths.Add(Path.Combine(windir, "assembly"));
            paths.Add(Path.Combine(windir, "Microsoft.NET", "assembly"));
            return paths;
        }

        static List<string> GetDefaultMonoGacPaths()
        {
            var paths = new List<string>(1);
            var gac = GetCurrentMonoGac();
            if (gac != null)
                paths.Add(gac);

            var gac_paths_env = Environment.GetEnvironmentVariable("MONO_GAC_PREFIX");
            if (string.IsNullOrEmpty(gac_paths_env))
                return paths;

            var prefixes = gac_paths_env.Split(Path.PathSeparator);
            foreach (var prefix in prefixes)
            {
                if (string.IsNullOrEmpty(prefix))
                    continue;

                var gac_path = Path.Combine(Path.Combine(Path.Combine(prefix, "lib"), "mono"), "gac");
                if (Directory.Exists(gac_path) && !paths.Contains(gac))
                    paths.Add(gac_path);
            }

            return paths;
        }

        static string GetCurrentMonoGac()
        {
            return Path.Combine(
                Directory.GetParent(
                    Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName)).FullName,
                "gac");
        }

        string GetAssemblyInGac(AssemblyNameReference reference)
        {
            if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
                return null;

            if (DetectMono())
                return GetAssemblyInMonoGac(reference);

            return GetAssemblyInNetGac(reference);
        }

        string GetAssemblyInMonoGac(AssemblyNameReference reference)
        {
            for (int i = 0; i < gac_paths.Count; i++)
            {
                var gac_path = gac_paths[i];
                var file = GetAssemblyFile(reference, string.Empty, gac_path);
                if (File.Exists(file))
                    return file;
            }

            return null;
        }

        string GetAssemblyInNetGac(AssemblyNameReference reference)
        {
            var gacs = new[] { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
            var prefixes = new[] { string.Empty, "v4.0_" };

            for (int i = 0; i < gac_paths.Count; i++)
            {
                for (int j = 0; j < gacs.Length; j++)
                {
                    var gac = Path.Combine(gac_paths[i], gacs[j]);
                    var file = GetAssemblyFile(reference, prefixes[i], gac);
                    if (Directory.Exists(gac) && File.Exists(file))
                        return file;
                }
            }

            return null;
        }

        static string GetAssemblyFile(AssemblyNameReference reference, string prefix, string gac)
        {
            var gac_folder = new StringBuilder()
                .Append(prefix)
                .Append(reference.Version)
                .Append("__");

            for (int i = 0; i < reference.PublicKeyToken.Length; i++)
                gac_folder.Append(reference.PublicKeyToken[i].ToString("x2"));

            return Path.Combine(
                Path.Combine(
                    Path.Combine(gac, reference.Name), gac_folder.ToString()),
                reference.Name + ".dll");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public static ModuleDefinition LoadMainModule(string mainAssemblyFileName, 
            bool throwOnError = true, bool inMemory = false)
        {
            var resolver = new UniversalAssemblyResolver(mainAssemblyFileName, throwOnError, null);

            var module = ModuleDefinition.ReadModule(mainAssemblyFileName, new ReaderParameters
            {
                AssemblyResolver = resolver,
                InMemory = inMemory
            });

            resolver = new UniversalAssemblyResolver(mainAssemblyFileName, throwOnError,
                DotNetCorePathFinder.DetectTargetFrameworkId(module.Assembly));

            module = ModuleDefinition.ReadModule(mainAssemblyFileName, new ReaderParameters
            {
                AssemblyResolver = resolver,
                InMemory = inMemory
            });

            return module;
        }

        #endregion
    }
}