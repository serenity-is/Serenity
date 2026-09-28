using Mono.Cecil;

namespace Serenity.CodeGeneration;

public partial class ServerTypingsGeneratorTests
{
    public static bool InTestNamespace(TypeDefinition type)
    {
        return type.Namespace == "ServerTypingsTest" ||
            type.Namespace.StartsWith("ServerTypingsTest.", StringComparison.Ordinal);
    }

    private static List<GeneratedSource> ExceptGenericFiles(List<GeneratedSource> files)
    {
        return [.. files.Where(x => x.Filename != "Texts.ts" &&
            x.Filename != "Namespaces.ts" &&
            x.Filename != "LazyTypeLoader.ts")];
    }

    private static ServerTypingsGenerator CreateGenerator(params Type[] types)
    {
        var generator = new ServerTypingsGenerator(new MockFileSystem(),
            typeof(ServerTypingsGeneratorTests).Assembly.Location);
        generator.AddBuiltinTSTypes();
        generator.RootNamespaces.Add("ServerTypingsTest");
        generator.ModuleReExports = false;
        generator.TypeFilter = type =>
        {
            return types.Length == 0 ? InTestNamespace(type) :
                types.Any(x => type.FullName == x.FullName);
        };
        return generator;
    }

    private static ServerTypingsGenerator CreateGeneratorModules(params Type[] types)
    {
        var generator = new ServerTypingsGenerator(new MockFileSystem(),
            typeof(ServerTypingsGeneratorTests).Assembly.Location);
        generator.RootNamespaces.Add("ServerTypingsTest");
        generator.TypeFilter = type =>
        {
            return types.Length == 0 ? InTestNamespace(type) :
                types.Any(x => type.FullName == x.FullName);
        };
        generator.ModuleReExports = false;
        return generator;
    }

    [Fact]
    public void Generates_Root_Namespace_Constants_Without_Server_Types()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;

        var result = generator.Run();
        var code = Assert.Single(result, x => x.Filename == "Namespaces.ts").Text;

        Assert.Contains("export const ServerTypingsTestNS: \"ServerTypingsTest\" = \"ServerTypingsTest\";", code);
        Assert.Contains("export const nsServerTypingsTest: \"ServerTypingsTest.\" = \"ServerTypingsTest.\";", code);
        Assert.Equal(code.Trim().Length, generator.GetRootNamespacesOnlyOutputLength());
    }

}
