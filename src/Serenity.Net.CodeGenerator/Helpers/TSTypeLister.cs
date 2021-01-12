using Newtonsoft.Json;
using Serenity.CodeGeneration;
using Serenity.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Serenity.CodeGenerator
{
    public class TSTypeLister
    {
        private string projectDir;

        public TSTypeLister(string projectDir)
        {
            this.projectDir = Path.GetFullPath(projectDir);
        }

        private string GetEmbeddedScript(string name)
        {
            using (var sr = new StreamReader(this.GetType()
                .GetTypeInfo().Assembly.GetManifestResourceStream(name)))
            {
                return sr.ReadToEnd();
            }
        }

        private class TSConfig
        {
            public CompilerConfig CompilerOptions { get; set; }
            public string[] Files { get; set; }
            public string[] Include { get; set; }
            public string[] Exclude { get; set; }

            public class CompilerConfig
            {
                public string[] TypeRoots { get; set; }
                public string[] Types { get; set; }
            }
        }

        public List<ExternalType> List()
        {
            var tsconfig = Path.Combine(projectDir, "tsconfig.json");
            IEnumerable<string> files = null;
            if (File.Exists(tsconfig))
            {
                var cfg = JSON.ParseTolerant<TSConfig>(File.ReadAllText(tsconfig));
                if (!cfg.Files.IsEmptyOrNull())
                {
                    files = cfg.Files.Where(x => File.Exists(Path.Combine(projectDir, PathHelper.ToPath(x))))
                        .Select(x => Path.GetFullPath(Path.Combine(projectDir, PathHelper.ToPath(x))));
                }
                else if (!cfg.Include.IsEmptyOrNull())
                {
                    var typeRoots = cfg.CompilerOptions?.TypeRoots?.IsEmptyOrNull() == true ?
                        new string[] { "./node_modules/types" } : cfg.CompilerOptions.TypeRoots;

                    var types = new HashSet<string>(cfg.CompilerOptions?.Types ?? Array.Empty<string>(),
                        StringComparer.OrdinalIgnoreCase);

                    files = typeRoots.Select(typeRoot => Path.Combine(projectDir, typeRoot))
                        .Where(typeRoot => Directory.Exists(typeRoot))
                        .Select(typeRoot => Path.GetFullPath(typeRoot))
                        .SelectMany(typeRoot =>
                            Directory.GetDirectories(typeRoot)
                                .Where(typing => (cfg.CompilerOptions?.Types == null) || types.Contains(Path.GetDirectoryName(typing)))
                                .Where(typing => Path.GetFileName(typing).Contains("serenity", StringComparison.OrdinalIgnoreCase) ||
                                    !PathHelper.ToUrl(typing).Contains("/node_modules/", StringComparison.OrdinalIgnoreCase))
                                .Select(typing => Path.Combine(typing, "index.d.ts"))
                                .Where(typing => File.Exists(typing)))
                        .ToList();

                    var includePatterns = cfg.Include
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    var excludePatterns = (cfg.Exclude ?? Array.Empty<string>())
                        .Select(x => PathHelper.ToUrl(x))
                        .Where(x => !x.StartsWith("../", StringComparison.Ordinal))
                        .Select(x => x.StartsWith("./", StringComparison.Ordinal) ? x[2..] :
                            (x.StartsWith("/", StringComparison.Ordinal) ? x[1..] : x))
                        .Select(x => PathHelper.ToPath(x));

                    files = files.Concat(cfg.Include.Select(x => PathHelper.ToUrl(x))
                        .Where(x => x.StartsWith("../", StringComparison.Ordinal) &&
                        !x.Contains('*', StringComparison.Ordinal))
                        .Select(x => Path.Combine(projectDir, x))
                        .Select(x => PathHelper.ToPath(x))
                        .Where(x => File.Exists(x)));

                    var includeGlob = new GlobFilter(includePatterns);
                    var excludeGlob = new GlobFilter(excludePatterns);

                    var allTsFiles = Directory.GetFiles(projectDir, "*.ts", SearchOption.AllDirectories)
                        .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                            !File.Exists(x.Substring(0, x.Length - ".d.ts".Length) + ".ts"));

                    files = files.Concat(allTsFiles.Where(x => includePatterns.Any() && 
                        includeGlob.IsMatch(x.Substring(projectDir.Length + 1)) &&
                        (!excludePatterns.Any() || !excludeGlob.IsMatch(x.Substring(projectDir.Length + 1)))));

                    files = files.Distinct();
                }
            }

            if (files == null || !files.Any())
            {
                var directories = new[]
                {
                    Path.Combine(projectDir, @"Modules"),
                    Path.Combine(projectDir, @"Imports"),
                    Path.Combine(projectDir, @"typings", "serenity"),
                    Path.Combine(projectDir, @"wwwroot", "Scripts", "serenity")
                }.Where(x => Directory.Exists(x));

                files = directories.SelectMany(x =>
                    Directory.GetFiles(x, "*.ts", SearchOption.AllDirectories))
                    .Where(x => !x.EndsWith(".d.ts", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetFileName(x).StartsWith("Serenity.", StringComparison.OrdinalIgnoreCase) ||
                        Path.GetFileName(x).StartsWith("Serenity-", StringComparison.OrdinalIgnoreCase));

                var corelib = files.Where(x => string.Equals(Path.GetFileName(x),
                    "Serenity.CoreLib.d.ts", StringComparison.OrdinalIgnoreCase));

                Func<string, bool> corelibUnderTypings = x =>
                    x.Replace('\\', '/').EndsWith("/typings/serenity/Serenity.CoreLib.d.ts",
                        StringComparison.OrdinalIgnoreCase);

                if (corelib.Count() > 1 &&
                    corelib.Any(x => !corelibUnderTypings(x)))
                {
                    files = files.Where(x => !corelibUnderTypings(x));
                }

                files = files.OrderBy(x => x);
            }

            var tsServices = GetEmbeddedScript("Serenity.CodeGenerator.Resource.typescriptServices.js");
            var codeGeneration = GetEmbeddedScript("Serenity.CodeGenerator.Resource.Serenity.CodeGeneration.js");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("var fs = require('fs');");
            sb.AppendLine(tsServices);
            sb.AppendLine(codeGeneration);
            foreach (var file in files)
                sb.AppendLine("Serenity.CodeGeneration.addSourceFile(" +
                    file.Replace('\\', '/').ToJson() + ", " +
                    File.ReadAllText(file).ToJson() + ");");
            sb.AppendLine("var types = JSON.stringify(Serenity.CodeGeneration.parseTypes());");
            sb.AppendLine("fs.writeFileSync('./typeList.json', types);");

            var cacheDir = Path.Combine(Path.GetTempPath(), ".tstypecache");

            var md5 = MD5.Create();
            var hash = BitConverter.ToString(md5.ComputeHash(Encoding.Unicode.GetBytes(sb.ToString())));
            var cacheFile = Path.Combine(cacheDir, hash + ".json");

            if (File.Exists(cacheFile))
            {
                try
                {
                    return JSON.Parse<List<ExternalType>>(File.ReadAllText(cacheFile));
                }
                catch
                {
                }
            }

            Action<string> writeCache = (json) =>
            {
                try
                {
                    Directory.CreateDirectory(cacheDir);
                    TemporaryFileHelper.PurgeDirectory(cacheDir, TimeSpan.Zero, 99, null);
                    File.WriteAllText(cacheFile, json);
                }
                catch
                {
                }
            };

            var tempDirectory = Path.ChangeExtension(Path.GetTempFileName(), null) + "__";
            Directory.CreateDirectory(tempDirectory);
            try
            {
                File.WriteAllText(Path.Combine(tempDirectory, "index.js"), sb.ToString());

                var process = Process.Start(new ProcessStartInfo()
                {
                    FileName = "node",
                    Arguments = "index.js",
                    WorkingDirectory = tempDirectory,
                    CreateNoWindow = true
                });
                process.WaitForExit(60000);
                var json = File.ReadAllText(Path.Combine(tempDirectory, "typeList.json"));
                writeCache(json);
                return JSON.Parse<List<ExternalType>>(json);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}