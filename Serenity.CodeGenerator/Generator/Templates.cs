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

        public static string Render(string templateKey, object model)
        {
            var template = GetTemplate(templateKey);
            try
            {
                var context = new TemplateContext();
                context.LoopLimit = 100000;
                context.RecursiveLimit = 1000;
                context.MemberRenamer = x => x.Name;
                context.CurrentGlobal.Import(model,
                    ScriptMemberImportFlags.Field | ScriptMemberImportFlags.Property,
                    null, x => x.Name);
                template.Render(context);
                return context.Output.ToString();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Template Has Errors: " + Environment.NewLine +
                    string.Join(Environment.NewLine, template.Messages.Select(x => x.ToString())), ex);
            }
        }
    }
}