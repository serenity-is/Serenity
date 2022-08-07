namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        protected void GenerateNamespaceImports()
        {
            var rootNamespace = RootNamespaces.FirstOrDefault(x => x != "Serenity") ?? "App";

            var byNamespace = generatedTypes
                .Where(x => !x.Module)
                .ToLookup(type =>
                {
                    string fullName = string.IsNullOrEmpty(type.Namespace) ? type.Name :
                        (type.Namespace + "." + type.Name);

                    if (string.IsNullOrEmpty(fullName))
                        return null;

                    if (!fullName.StartsWith(rootNamespace + ".", StringComparison.OrdinalIgnoreCase))
                        return null;

                    var idx = fullName.IndexOf('.', rootNamespace.Length + 1);
                    if (idx < 0)
                        return rootNamespace;

                    return fullName[(rootNamespace.Length + 1)..idx];
                }).Where(x => x.Key != null);

            foreach (var types in byNamespace)
            {
                foreach (var type in types.OrderBy(x => x.TypeOnly)
                    .ThenBy(x => x.Namespace, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(x => x.Name))
                {
                    string fullName = string.IsNullOrEmpty(type.Namespace) ? type.Name :
                        (type.Namespace + "." + type.Name);

                    if (type.TypeOnly)
                        cw.Indented("export type ");
                    else
                        cw.Indented("export import ");

                    var shortName = fullName;
                    if (shortName.StartsWith(rootNamespace + "." + types.Key + ".", StringComparison.OrdinalIgnoreCase))
                        shortName = shortName[(rootNamespace.Length + types.Key.Length + 2)..];
                    else if (shortName.StartsWith(rootNamespace + ".", StringComparison.OrdinalIgnoreCase))
                        shortName = shortName[(rootNamespace.Length + 1)..];

                    sb.Append(shortName.Replace('.', '_'));
                    sb.Append(" = ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }

                if (sb.Length > 0)
                {
                    AddFile(types.Key + ".ts", module: true);
                    sb.Clear();
                }
            }

        }
    }
}