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

        private static readonly ConcurrentDictionary<string, Template> templateCache = new();

        private static Template GetTemplate(string templateKey)
        {
            if (templateCache.TryGetValue(templateKey, out Template t))
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
                    throw new Exception("Can't find template resource with key: " + templateKey);

                using var sr = new StreamReader(stream);
                template = sr.ReadToEnd();
            }

            t = Template.Parse(template);
            templateCache[templateKey] = t;
            return t;
        }

        public static string Render(string templateKey, object model)
        {
            var template = GetTemplate(templateKey);
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
                return template.Render(context);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Template Has Errors: " + Environment.NewLine +
                    string.Join(Environment.NewLine, template.Messages.Select(x => x.ToString())), ex);
            }
        }
    }
}