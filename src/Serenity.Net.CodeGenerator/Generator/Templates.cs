using Scriban;
using Scriban.Runtime;

namespace Serenity.CodeGenerator;

public static class Templates
{
    public static string TemplatePath { get; set; }

    private static readonly ConcurrentDictionary<string, Template> templateCache = new();

    private static Template GetTemplate(IGeneratorFileSystem fileSystem, string templateKey)
    {
        if (templateCache.TryGetValue(templateKey, out Template t))
            return t;

        string template = null;

        if (!string.IsNullOrEmpty(TemplatePath))
        {
            var overrideFile = fileSystem.Combine(TemplatePath, templateKey + ".scriban");
            if (fileSystem.FileExists(overrideFile))
                template = fileSystem.ReadAllText(overrideFile);
        }

        if (template == null)
        {
            var stream = typeof(Templates).Assembly
                .GetManifestResourceStream("Serenity.CodeGenerator.Templates." + templateKey + ".scriban") ?? 
                    throw new Exception("Can't find template resource with key: " + templateKey);

            using var sr = new System.IO.StreamReader(stream);
            template = sr.ReadToEnd();
        }

        t = Template.Parse(template);
        templateCache[templateKey] = t;
        return t;
    }

    public static string Render(IGeneratorFileSystem fileSystem, string templateKey, object model,
        Action<CodeWriter> initWriter = null)
    {
        var template = GetTemplate(fileSystem, templateKey);
        try
        {
            var context = new TemplateContext
            {
                LoopLimit = 100000,
                RecursiveLimit = 1000,
                MemberRenamer = x => x.Name
            };
            context.CurrentGlobal.Import(model,
                ScriptMemberImportFlags.Field | ScriptMemberImportFlags.Property,
                null, x => x.Name);

            var cw = new CodeWriter
            {
                AllowUsing = ns => ns == "Serenity" ||
                  ns.StartsWith("Serenity.", StringComparison.Ordinal) ||
                  ns == "Microsoft.AspNetCore.Mvc" ||
                  ns == "System.Globalization" ||
                  ns == "System.Data" ||
                  ns == "System" ||
                  ns == "System.IO" ||
                  ns == "System.ComponentModel" ||
                  ns == "System.Collections.Generic",
                IsCSharp = true
            };

            if (model is EntityModel entityModel)
            {
                cw.GlobalUsings = new(entityModel.GlobalUsings);
                cw.FileScopedNamespaces = entityModel.FileScopedNamespaces;
            }

            initWriter?.Invoke(cw);

            var modularTSImporter = new ModularTSImporter((model as EntityModel)?.Module);

            context.PushGlobal(CSharpDynamicUsings.GetScriptObject(cw));
            context.PushGlobal(ModularTSImporter.GetScriptObject(modularTSImporter));

            cw.Append(template.Render(context).Trim());

            cw.Insert(0, modularTSImporter.GetImports());

            return cw.ToString();

        }
        catch (InvalidOperationException ex)
        {
            throw new Exception("Template Has Errors: " + Environment.NewLine +
                string.Join(Environment.NewLine, template.Messages.Select(x => x.ToString())), ex);
        }
    }
}