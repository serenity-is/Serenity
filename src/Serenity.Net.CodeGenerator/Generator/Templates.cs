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
        delegate string TypeListDelegate(List<string> key);
        delegate string TypeModelListDelegate(List<TypeRefModel> key);
        delegate void UsingDelegate(string key);

        public static string ShortTypeName(CodeWriter cw, string fullName)
        {
            fullName = fullName.Trim();

            if (string.IsNullOrEmpty(fullName))
                return string.Empty;
            
            var nullableText = "";
            if (fullName.EndsWith('?'))
            {
                fullName = fullName[..^1];
                nullableText = "?";
            }

            if (SystemTypes.IsCSKeyword(fullName))
                return fullName + nullableText;

            if (fullName.IndexOf('.', StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (fullName == "Stream")
                    fullName = "System.IO.Stream";
                else
                {
                    var type = Type.GetType("System." + fullName);

                    if (type != null)
                    {
                        fullName = type.FullName;
                    }
                    else
                        return fullName + nullableText;
                }
            }

            if (fullName.EndsWith('>'))
            {
                var idx = fullName.IndexOf('<', StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    return cw.ShortTypeName(fullName[..idx]) + '<' + ShortTypeName(cw, fullName[(idx + 1)..^1]) + '>' + nullableText;
            }

            return cw.ShortTypeName(fullName) + nullableText;
        }

        public static string Render(IGeneratorFileSystem fileSystem, string templateKey, object model, string subNamespacePath = "")
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
                      ns == "System.Collections.Generic"
                };

                if (model is EntityModel em && !string.IsNullOrEmpty(em.RootNamespace) && !string.IsNullOrEmpty(em.DotModule) && !string.IsNullOrEmpty(subNamespacePath))
                {
                    cw.CurrentNamespace = em.RootNamespace + em.DotModule + subNamespacePath;
                }

                var scriptObject = new ScriptObject();
                scriptObject.Import("TYPEREF", new TypeDelegate((fullName) =>
                {
                    return ShortTypeName(cw, fullName);
                }));

                scriptObject.Import("TYPEREFLIST", new TypeListDelegate((fullNames) =>
                {
                    if (fullNames == null)
                        return "";

                    HashSet<string> result = new();

                    foreach (var fullName in fullNames)
                    {
                        result.Add(ShortTypeName(cw, fullName));
                    }

                    return string.Join(", ", result);
                }));

                scriptObject.Import("TYPEREFMODELLIST", new TypeModelListDelegate((models) =>
                {
                    if (models == null)
                        return "";

                    HashSet<string> result = new();

                    foreach (var model in models)
                    {
                        result.Add(ShortTypeName(cw, model.TypeName) + model.ArgumentPart);
                    }

                    return string.Join(", ", result);
                }));

                scriptObject.Import("USING", new UsingDelegate((requestedNamespace) =>
                {
                    cw.Using(requestedNamespace, true);
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