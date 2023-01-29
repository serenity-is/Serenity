using Scriban.Runtime;

namespace Serenity.CodeGenerator;

public class CSharpDynamicUsings
{
    delegate string TypeDelegate(string key);
    delegate string NamespaceDelegate(string key);
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
        
        return scriptObject;
    }
}