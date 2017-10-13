using RazorGenerator.Templating;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace Serenity.CodeGenerator
{
    public static class Templates
    {
        public static string TemplatePath { get; set; }

        private static ConcurrentDictionary<string, Scriban.Template> templateCache = 
            new ConcurrentDictionary<string, Scriban.Template>();

        private static Scriban.Template GetTemplate(string templateKey)
        {
            Scriban.Template t;
            if (templateCache.TryGetValue(templateKey, out t))
                return t;

            string template = null;

            if (!string.IsNullOrEmpty(TemplatePath))
            {
                var overrideFile = Path.Combine(TemplatePath, templateKey + ".scriban");
                if (File.Exists(overrideFile))
                    template = File.ReadAllText(overrideFile);
            }

            if (template == null)
            {
                var stream = typeof(Templates).Assembly
                    .GetManifestResourceStream("Serenity.CodeGenerator.Templates." + templateKey + ".scriban");

                if (stream == null)
                    throw new System.Exception("Can't find template resource with key: " + templateKey);

                using (var sr = new StreamReader(stream))
                    template = sr.ReadToEnd();
            }

            t = Scriban.Template.Parse(template);
            templateCache[templateKey] = t;
            return t;
        }

        private static DelegateMemberRenamer noRenamer = new DelegateMemberRenamer(x => x);

        public static string Render(string templateKey, object model)
        {
            var template = GetTemplate(templateKey);
            try
            {
                var context = new TemplateContext();
                context.MemberRenamer = noRenamer;
                context.CurrentGlobal.Import(model, 
                    ScriptMemberImportFlags.Field | ScriptMemberImportFlags.Property, 
                    null, noRenamer);
                template.Render(context);
                return context.Output.ToString();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Template Has Errors: " + Environment.NewLine + 
                    string.Join(Environment.NewLine, template.Messages.Select(x => x.ToString())), ex);
            }
        }

        public static string Render<T>(RazorTemplateBase template, T model)
        {
            ((dynamic)template).Model = model;
            return template.TransformText();
        }

        public static string Render<T>(RazorTemplateBase template, T model, GeneratorConfig config)
        {
            ((dynamic)template).Model = model;
            ((dynamic)template).Config = config;
            return template.TransformText();
        }
    }
}