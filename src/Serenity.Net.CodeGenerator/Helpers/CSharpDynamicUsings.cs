using Scriban.Runtime;

namespace Serenity.CodeGenerator;

public class CSharpDynamicUsings
{
    delegate string TypeDelegate(string key);
    delegate string NamespaceDelegate(string key);
    delegate string NamespaceBlockDelegate(string text);
    delegate string TypeListDelegate(List<string> key);
    delegate string TypeModelListDelegate(List<AttributeTypeRef> key);
    delegate void UsingDelegate(string key);

    public static ScriptObject GetScriptObject(CodeWriter cw)
    {
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
            return "namespace " + requestedNamespace;
        }));

        scriptObject.Import("NAMESPACEBLOCK", new NamespaceBlockDelegate((code) =>
        {
            if (!cw.FileScopedNamespaces ||
                !cw.IsCSharp)
                return "\n{\n" + string.Join("\n", (code ?? "").TrimStart()
                    .Replace("\r", "")
                    .Split('\n')
                    .Select(line => (line.Length > 0 ? ("    " + line) : line)))
                        .TrimEnd() + "\n}";

            return ";\n\n" + code.Trim();
        }));

        return scriptObject;
    }
}