﻿using System;
using Serenity.CodeGeneration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(name);
            if (stream == null)
                throw new Exception("Script " + name + " doesn't exist.");
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }

        public List<ExternalType> List()
        {
            var files = Directory.GetFiles(Path.Combine(projectDir, @"Modules"), "*.ts", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"Imports"), "*.ts", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"typings"), "*.ts", SearchOption.AllDirectories))
                .Where(x => !x.EndsWith(".d.ts") || x.IndexOf("Serenity") >= 0);

            var tempDirectory = Path.ChangeExtension(Path.GetTempFileName(), null) + "__";
            Directory.CreateDirectory(tempDirectory);
            try
            {
#if NET46
                const string nsPrefix = "Serenity.CodeGenerator.Core.";
#else
                const string nsPrefix = "Serenity.CodeGenerator.";
#endif
                File.WriteAllText(Path.Combine(tempDirectory, "typeScriptServices.js"), GetEmbeddedScript(nsPrefix + "typeScriptServices.js") + "\n\nif (exports)\nexports.ts = ts;\n");
                File.WriteAllText(Path.Combine(tempDirectory, "Serenity.CodeGeneration.js"), GetEmbeddedScript(nsPrefix + "Serenity.CodeGeneration.js") + "\n\nif (exports)\nexports.Serenity = Serenity;\n");
                File.WriteAllText(Path.Combine(tempDirectory, "lib.d.ts_"), GetEmbeddedScript(nsPrefix + "lib.d.ts_"));

                var sb = new StringBuilder();
                sb.AppendLine("var fs = require('fs');");
                sb.AppendLine("global.ts = require('./typescriptServices.js');");
                sb.AppendLine("global.Serenity = require('./Serenity.CodeGeneration.js').Serenity;");
                sb.AppendLine("var libdts = fs.readFileSync('./lib.d.ts_').toString();");
                sb.AppendLine("var inputFiles = " + JSON.StringifyIndented(files.Select(x => x.Replace('\\', '/'))) + ";");
                sb.AppendLine("for (var i = 0; i < inputFiles.length; i++) {");
                sb.AppendLine("  var sourceText = fs.readFileSync(inputFiles[i]);");
                sb.AppendLine("  Serenity.CodeGeneration.addSourceFile(inputFiles[i], sourceText.toString());");
                sb.AppendLine("}");
                sb.AppendLine("var types = JSON.stringify(Serenity.CodeGeneration.parseTypes(sourceText));");
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
                return JSON.Parse<List<ExternalType>>(json);
            }
            finally
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}