namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GenerateNamespaceConstants()
    {
        var keyToNamespace = namespaceConstants
            .Where(x => x.Value?.Count == 1)
            .ToLookup(x => x.Key.Replace(".", "", StringComparison.Ordinal));

        foreach (var item in keyToNamespace)
        {
            if (item.Count() > 1)
                continue;
            var ns = item.First().Value[0];

            string quoted = ns.ToDoubleQuoted();
            string quotedDot = (ns + ".").ToDoubleQuoted();
            sb.AppendLine($"export const {item.Key}NS: {quoted} = {quoted};");
            sb.AppendLine($"export const ns{item.Key}: {quotedDot} = {quotedDot};");
        }

        AddFile("Namespaces.ts");
    }
}