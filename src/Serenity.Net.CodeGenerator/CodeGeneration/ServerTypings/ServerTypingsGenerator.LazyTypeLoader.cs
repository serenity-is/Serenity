namespace Serenity.CodeGeneration;

public partial class ServerTypingsGenerator : TypingsGeneratorBase
{
    protected void GenerateLazyTypeLoader()
    {
        var config = ImportFromCorelib("Config");
        sb.Append("const loaderByKey =");
        cw.InBrace(() => 
        {
            bool first = true;

            foreach (var types in modularDialogTypeByKey
               .Concat(modularEditorTypeByKey)
               .Concat(modularFormatterTypeByKey)
               .Select(x => (x.Key, Type: x.FirstOrDefault()))
               .Where(x => x.Type != null)
               .OrderBy(x => x.Key))
            {
                if (string.IsNullOrEmpty(types.Key))
                    continue;

                var type = types.Type;
                if (type == null)
                    continue;

                if (string.IsNullOrEmpty(type.Module))
                    continue;

                // these are currently loaded via appsettings.bundles.json
                if (type.Module == "@serenity-is/corelib" ||
                    type.Module == "@serenity-is/extensions" ||
                    type.Module == "@serenity-is/pro.extensions" ||
                    type.Module == "@serenity-is/sleekgrid")
                    continue;

                // don't generate dynamic imports for types that are not in current project
                if (!type.Module.StartsWith('/'))
                    continue;

                var module = RelativeModulePath("LazyTypeLoader.ts", type.Module);

                if (first)
                    first = false;
                else
                    cw.AppendLine(",");
                cw.Indented($"{Newtonsoft.Json.JsonConvert.SerializeObject(types.Key)}: async () => (await import({Newtonsoft.Json.JsonConvert.SerializeObject(module)})).{type.Name}");
            }
            if (!first)
                cw.AppendLine();
        });

        cw.AppendLine();
        cw.AppendLine("""
            Config.lazyTypeLoader = (function(org: any) {
                return (key: string, type: any) => loaderByKey[key]?.() || org?.(key, type);
            })(Config.lazyTypeLoader);
            """);
        AddFile("LazyTypeLoader.ts");
    }
}