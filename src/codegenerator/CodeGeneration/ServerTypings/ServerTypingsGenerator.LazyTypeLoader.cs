namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator
{
    /// <summary>
    /// Generates the module that loads the types implementing <c>LazyLoadInclude</c>. As an .mts file it is
    /// an entry point with a stable URL, while the literal dynamic imports make esbuild split each
    /// type into its own chunk. It is only generated for a project that has such types.
    /// </summary>
    protected void GenerateLazyTypeLoader()
    {
        var lazyTypes = GetLazyLoadTypes();
        if (lazyTypes.Count == 0)
            return;

        sb.Append("export const loaderByKey: Record<string, () => Promise<any>> =");
        cw.InBrace(() =>
        {
            for (var i = 0; i < lazyTypes.Count; i++)
            {
                var (key, type) = lazyTypes[i];
                var module = RelativeModulePath("LazyTypeLoader.mts", type.Module);
                cw.Indented($"{Newtonsoft.Json.JsonConvert.SerializeObject(key)}: async () => (await import({Newtonsoft.Json.JsonConvert.SerializeObject(module)})).{type.Name}");
                cw.AppendLine(i < lazyTypes.Count - 1 ? "," : "");
            }
        });

        AddFile("LazyTypeLoader.mts");
    }
}
