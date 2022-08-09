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
        delegate void UsingDelegate(string key);

        public static string ShortTypeName(this CodeWriter cw, string fullName)
        {
            fullName = fullName.Trim();

            // checks for nullable and normal system types
            if (SystemTypes.IsCSKeyword(fullName) || SystemTypes.IsCSKeyword(fullName[..^1]))
                return fullName;
                        
            if (fullName.IndexOf(".") < 0)
            {
                if (fullName == "Stream")
                    fullName = "System.IO.Stream";
                else
                {
                    var isNullable = fullName.EndsWith("?");

                    var type = Type.GetType("System." + fullName.Replace("?", ""));

                    if (type != null)
                    {
                        fullName = type.FullName;
                        if (isNullable)
                            fullName += "?";
                    }
                    else
                        return fullName;
                }
            }
            
            var typeFullName = fullName.Trim();
            
            var functionPart = typeFullName.IndexOf("(") < 0 ? "" : typeFullName[typeFullName.IndexOf("(")..];

            typeFullName = typeFullName.IndexOf("(") < 0 ? typeFullName : typeFullName[..typeFullName.IndexOf("(")];

            var genericPart = typeFullName.IndexOf("<") < 0 ? "" : typeFullName[typeFullName.IndexOf("<")..];

            typeFullName = typeFullName.IndexOf("<") < 0 ? typeFullName : typeFullName[..typeFullName.IndexOf("<")];

            var nameSpace = typeFullName[..typeFullName.LastIndexOf(".")];
            
            var typeName = typeFullName[(typeFullName.LastIndexOf(".") + 1)..];

            return ShortTypeName(nameSpace, cw.CurrentNamespace, cw.Using) + typeName + genericPart + functionPart;
        }

        public static string ShortTypeName(this string ns, string nsCode,
        Func<string, bool> uses = null)
        {
            if (string.IsNullOrEmpty(ns) ||
                (!string.IsNullOrEmpty(nsCode) &&
                 (nsCode == ns ||
                  nsCode.StartsWith(ns + ".", StringComparison.Ordinal))) ||
                (uses != null && uses(ns)))
                return string.Empty;

            if (!string.IsNullOrEmpty(nsCode))
            {
                var idx = nsCode.IndexOf('.');
                if (idx >= 0 && ns.StartsWith(nsCode.Substring(0, idx + 1), StringComparison.Ordinal))
                    return ns.Substring(idx + 1) + ".";
            }

            return ns + ".";
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