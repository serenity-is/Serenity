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

        scriptObject.Import("TYPEREF", new TypeDelegate(cw.ShortTypeRef));

        scriptObject.Import("TYPEREFLIST", new TypeListDelegate((fullNames) =>
        {
            if (fullNames == null)
                return "";

            HashSet<string> result = [];

            foreach (var fullName in fullNames)
            {
                result.Add(cw.ShortTypeRef(fullName));
            }

            return string.Join(", ", result);
        }));

        scriptObject.Import("ATTRREF", new TypeModelListDelegate((models) =>
        {
            if (models == null)
                return "";

            var sb = new StringBuilder();
            var lineLength = 0;
            for (var i = 0; i < models.Count; i++)
            {
                var str = models[i].ToString(cw);
                if (lineLength == 0 || lineLength + str.Length + 3 < 130)
                {
                    if (lineLength > 0)
                        sb.Append(", ");
                    sb.Append(str);
                    lineLength += str.Length;
                }
                else
                {
                    sb.AppendLine("]");
                    sb.Append(cw.FileScopedNamespaces ? "    " : "        ");
                    sb.Append('[');
                    lineLength = str.Length + 1;
                    sb.Append(str);
                }
            }

            return sb.ToString();
            
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