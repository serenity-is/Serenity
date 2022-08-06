namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGenerator : TypingsGeneratorBase
    {
        protected void GenerateNamespaceExports()
        {
            var rootNamespace = RootNamespaces.FirstOrDefault(x => x != "Serenity") ?? "App";

            var byNamespace = generatedTypes.ToLookup(fullName =>
            {
                if (string.IsNullOrEmpty(fullName))
                    return null;

                if (!fullName.StartsWith(rootNamespace + ".", StringComparison.OrdinalIgnoreCase))
                    return null;

                var idx = fullName.IndexOf('.', rootNamespace.Length + 1);
                if (idx < 0)
                    return rootNamespace;

                return fullName[(rootNamespace.Length + 1)..idx];
            }).Where(x => x.Key != null);

            foreach (var ns in byNamespace)
            {
                foreach (var fullName in ns.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                {
                    cw.Indented("export import ");

                    var shortName = fullName;
                    if (shortName.StartsWith(rootNamespace + "." + ns.Key + ".", StringComparison.OrdinalIgnoreCase))
                        shortName = shortName[(rootNamespace.Length + ns.Key.Length + 2)..];
                    else if (shortName.StartsWith(rootNamespace + ".", StringComparison.OrdinalIgnoreCase))
                        shortName = shortName[(rootNamespace.Length + 1)..];

                    sb.Append(shortName.Replace('.', '_'));
                    sb.Append(" = ");
                    sb.Append(fullName);
                    sb.AppendLine(";");
                }

                if (sb.Length > 0)
                {
                    AddFile(ns.Key + ".ts", module: true);
                    sb.Clear();
                }
            }

        }
    }
}