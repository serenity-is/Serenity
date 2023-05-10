namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GenerateModuleReExports()
    {
        var rootNamespace = RootNamespaces.FirstOrDefault(x => x != "Serenity") ?? "App";

        var byDirectory = generatedCode
            .Where(x => x.Module)
            .ToLookup(type =>
            {
                var directory = System.IO.Path.GetDirectoryName(type.Filename);
                if (string.IsNullOrEmpty(directory))
                    return null;

                return directory;
            }).Where(x => x.Key != null);

        foreach (var files in byDirectory)
        {
            foreach (var file in files.OrderBy(x => x.Filename))
            {
                var name = System.IO.Path.GetFileNameWithoutExtension(file.Filename);
                sb.AppendLine($"export * from \"./{files.Key}/{name}\"");
            }

            if (sb.Length > 0)
            {
                AddFile(files.Key + ".ts", module: true);
            }
        }

    }
}