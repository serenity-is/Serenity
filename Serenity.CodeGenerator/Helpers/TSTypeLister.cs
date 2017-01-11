using JavaScriptEngineSwitcher.ChakraCore;
using Serenity.CodeGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
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

        private JavaScriptEngineSwitcher.Core.IJsEngine SetupJsEngine()
        {
            var jsEngine = new ChakraCoreJsEngineFactory().CreateEngine();
            try
            {
                jsEngine.Evaluate(GetEmbeddedScript("typeScriptServices.js"));
                jsEngine.Evaluate(GetEmbeddedScript("Serenity.CodeGeneration.js"));

                return jsEngine;
            }
            catch
            {
                jsEngine.Dispose();
                throw;
            }
        }

        public List<ExternalType> List()
        {
            var files = Directory.GetFiles(Path.Combine(projectDir, @"Modules"), "*.ts", SearchOption.AllDirectories)
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"Imports"), "*.ts", SearchOption.AllDirectories))
                .Concat(Directory.GetFiles(Path.Combine(projectDir, @"typings"), "*.ts", SearchOption.AllDirectories))
                .Where(x => !x.EndsWith(".d.ts") || x.IndexOf("Serenity") >= 0);

            using (var jsEngine = SetupJsEngine())
            {
                jsEngine.SetVariableValue("fileName", "/lib.d.ts");
                jsEngine.SetVariableValue("sourceText", GetEmbeddedScript("lib.d.ts_"));
                jsEngine.Evaluate("Serenity.CodeGeneration.addSourceFile(fileName, sourceText)");

                foreach (var file in files)
                {
                    var name = file.Substring(projectDir.Length - 1).Replace("\\", "/");
                    jsEngine.SetVariableValue("fileName", name);
                    jsEngine.SetVariableValue("sourceText", File.ReadAllText(file));
                    jsEngine.Evaluate("Serenity.CodeGeneration.addSourceFile(fileName, sourceText)");
                }

                var json = jsEngine.Evaluate<string>(
                    "JSON.stringify(Serenity.CodeGeneration.parseTypes(sourceText))");

                return JSON.Parse<List<ExternalType>>(json);
            }
        }
    }
}