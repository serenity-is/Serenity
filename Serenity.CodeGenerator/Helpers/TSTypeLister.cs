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
            this.projectDir = projectDir;
        }

        private string GetEmbeddedScript(string name)
        {
            using (var sr = new StreamReader(this.GetType()
                .GetTypeInfo().Assembly.GetManifestResourceStream(name)))
            {
                return sr.ReadToEnd();
            }
        }

        public List<ExternalType> List()
        {
            var files = Directory.GetFiles(Path.Combine(projectDir, @"Modules"), "*.ts", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"Imports"), "*.ts", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"typings"), "*.ts", SearchOption.AllDirectories))
                .Where(x => !x.EndsWith(".d.ts") || x.IndexOf("Serenity") >= 0).OrderBy(x => x);

            var tsServices = GetEmbeddedScript("Serenity.CodeGenerator.typeScriptServices.js");
            var codeGeneration = GetEmbeddedScript("Serenity.CodeGenerator.Serenity.CodeGeneration.js");

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
                    TemporaryFileHelper.PurgeDirectory(cacheDir, TimeSpan.Zero, 9, null);
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
                sb.Insert(0, "var fs = require('fs');\n");
                sb.AppendLine("var types = JSON.stringify(Serenity.CodeGeneration.parseTypes());");
                sb.AppendLine("fs.writeFileSync('./typeList.json', types);");
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