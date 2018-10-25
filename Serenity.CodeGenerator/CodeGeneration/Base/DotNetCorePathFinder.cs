﻿using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ICSharpCode.Decompiler
{
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
                this.Name = parts[0];
                this.Version = parts[1];
                this.Type = type;
                this.Path = path;
                this.RuntimeComponents = runtimeComponents ?? new string[0];
            }
        }

        static readonly string[] LookupPaths = new string[] {
             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"),
             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "sdk", "NuGetFallbackFolder"),
             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "sdk", "NuGetFallbackFolder")
        };

        readonly Dictionary<string, DotNetCorePackageInfo> packages;
        ISet<string> packageBasePaths = new HashSet<string>(StringComparer.Ordinal);
        readonly string assemblyName;
        readonly string basePath;
        readonly Version version;
        readonly string dotnetBasePath = FindDotNetExeDirectory();

        public DotNetCorePathFinder(string parentAssemblyFileName, string targetFrameworkId, Version version)
        {
            this.assemblyName = Path.GetFileNameWithoutExtension(parentAssemblyFileName);
            this.basePath = Path.GetDirectoryName(parentAssemblyFileName);
            this.version = version;

            var depsJsonFileName = Path.Combine(basePath, $"{assemblyName}.deps.json");
            if (File.Exists(depsJsonFileName))
                packages = LoadPackageInfos(depsJsonFileName, targetFrameworkId).ToDictionary(i => i.Name);

            foreach (var path in LookupPaths)
            {
                foreach (var pk in packages)
                {
                    foreach (var item in pk.Value.RuntimeComponents)
                    {
                        var itemPath = Path.GetDirectoryName(item);
                        var fullPath = Path.Combine(path, pk.Value.Name, pk.Value.Version, itemPath).ToLowerInvariant();
                        if (Directory.Exists(fullPath))
                            packageBasePaths.Add(fullPath);
                    }
                }
            }
        }

        public string TryResolveDotNetCore(AssemblyNameReference name)
        {
            foreach (var basePath in packageBasePaths)
            {
                if (File.Exists(Path.Combine(basePath, name.Name + ".dll")))
                {
                    return Path.Combine(basePath, name.Name + ".dll");
                }
                else if (File.Exists(Path.Combine(basePath, name.Name + ".exe")))
                {
                    return Path.Combine(basePath, name.Name + ".exe");
                }
            }

            return FallbackToDotNetSharedDirectory(name, version);
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
                var runtimeInfo = (rti?["runtime"]?.Children().OfType<JProperty>().Select(i => i.Name) ?? new string[0])
                    .Concat(rti?["compile"]?.Children().OfType<JProperty>().Select(i => i.Name) ?? new string[0])
                    .ToArray();

                yield return new DotNetCorePackageInfo(library.Name, type, path, runtimeInfo);
            }
        }

        string FallbackToDotNetSharedDirectory(AssemblyNameReference name, Version version)
        {
            if (dotnetBasePath == null) return null;
            var basePath = Path.Combine(dotnetBasePath, "shared", "Microsoft.NETCore.App");
            var closestVersion = GetClosestVersionFolder(basePath, version);
            if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".dll")))
            {
                return Path.Combine(basePath, closestVersion, name.Name + ".dll");
            }
            else if (File.Exists(Path.Combine(basePath, closestVersion, name.Name + ".exe")))
            {
                return Path.Combine(basePath, closestVersion, name.Name + ".exe");
            }
            return null;
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
            int dashIndex = shortName.IndexOf('-');
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

        [DllImport("libc")]
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
            {
                position++;
                return null;
            }

            int length = (int)ReadCompressedUInt32(buffer, ref position);
            if (length == 0)
                return string.Empty;

            string str = System.Text.Encoding.UTF8.GetString(
                buffer, position,
                buffer[position + length - 1] == 0 ? length - 1 : length);

            position += length;
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
}