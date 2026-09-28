namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    private static string GenerateNamespaceConstantsText(
        Dictionary<string, List<string>> namespaceConstants)
    {
        var keyToNamespace = namespaceConstants
            .Where(x => x.Value?.Count == 1)
            .ToLookup(x => x.Key.Replace(".", "", StringComparison.Ordinal));

        var output = new StringBuilder();
        foreach (var item in keyToNamespace.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (item.Count() > 1)
                continue;
            var ns = item.First().Value[0];

            string quoted = ns.ToDoubleQuoted();
            string quotedDot = (ns + ".").ToDoubleQuoted();
            output.AppendLine($"export const {item.Key}NS: {quoted} = {quoted};");
            output.AppendLine($"export const ns{item.Key}: {quotedDot} = {quotedDot};");
        }

        if (string.IsNullOrWhiteSpace(output.ToString()))
        {
            output.Clear();
            output.AppendLine("export {}");
        }

        return output.ToString().Trim();
    }

    public int GetRootNamespacesOnlyOutputLength()
    {
        var rootNamespaceConstants = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        if (RootNamespaces != null)
        {
            foreach (var ns in RootNamespaces)
                if (ns != "Serenity")
                    rootNamespaceConstants[ns] = [ns];
        }

        return GenerateNamespaceConstantsText(rootNamespaceConstants).Length;
    }

    protected void GenerateNamespaceConstants()
    {
        if (RootNamespaces != null)
        {
            foreach (var ns in RootNamespaces)
                if (ns != "Serenity")
                    AddNamespaceConstant(ns, includeRootNamespace: true);
        }

        sb.Append(GenerateNamespaceConstantsText(namespaceConstants));
        AddFile("Namespaces.ts");
    }
}