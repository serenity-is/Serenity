using Scriban;
using Scriban.Runtime;
using Serenity.CodeGeneration;

namespace Serenity.CodeGenerator
{
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
                    .GetManifestResourceStream("Serenity.CodeGenerator.Templates." + templateKey + ".scriban");

                if (stream == null)
                    throw new Exception("Can't find template resource with key: " + templateKey);

                using var sr = new System.IO.StreamReader(stream);
                template = sr.ReadToEnd();
            }

            t = Template.Parse(template);
            templateCache[templateKey] = t;
            return t;
        }

        delegate string TypeDelegate(string key);
        delegate string NamespaceDelegate(string key);
        delegate string TypeListDelegate(List<string> key);
        delegate string TypeModelListDelegate(List<AttributeTypeRef> key);
        delegate void UsingDelegate(string key);
        
        public static string Render(IGeneratorFileSystem fileSystem, string templateKey, object model)
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

                var scriptObject = new ScriptObject();
                scriptObject.Import("TYPEREF", new TypeDelegate((fullName) =>
                {
                    return cw.ShortTypeName(cw, fullName);
                }));

                scriptObject.Import("TYPEREFLIST", new TypeListDelegate((fullNames) =>
                {
                    if (fullNames == null)
                        return "";

                    HashSet<string> result = new();

                    foreach (var fullName in fullNames)
                    {
                        result.Add(cw.ShortTypeName(cw, fullName));
                    }

                    return string.Join(", ", result);
                }));

                scriptObject.Import("ATTRREF", new TypeModelListDelegate((models) =>
                {
                    if (models == null)
                        return "";

                    HashSet<string> result = new();

                    foreach (var model in models)
                    {
                        result.Add(cw.ShortTypeName(cw, model.TypeName) + (string.IsNullOrEmpty(model.Arguments) ? "" : "(" + model.Arguments + ")"));
                    }

                    return string.Join(", ", result);
                }));

                scriptObject.Import("USING", new UsingDelegate((requestedNamespace) =>
                {
                    cw.Using(requestedNamespace, true);
                }));

                scriptObject.Import("NAMESPACE", new NamespaceDelegate((requestedNamespace) =>
                {
                    cw.CurrentNamespace = requestedNamespace;
                    return requestedNamespace;
                }));

                context.PushGlobal(scriptObject);

                cw.Append(template.Render(context).Trim());
                return cw.ToString();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception("Template Has Errors: " + Environment.NewLine +
                    string.Join(Environment.NewLine, template.Messages.Select(x => x.ToString())), ex);
            }
        }
    }
}