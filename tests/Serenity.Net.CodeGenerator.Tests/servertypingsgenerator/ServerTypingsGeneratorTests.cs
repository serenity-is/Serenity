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
            x.Filename != "LazyTypeLoader.mts")];
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

    private static ExternalType LazyEditor(string name, string? marker)
    {
        return new ExternalType
        {
            Name = name,
            Module = "/Modules/Editors/" + name,
            Interfaces = marker is null ? null : [marker],
            Fields =
            [
                new ExternalMember
                {
                    Name = "[Symbol.typeInfo]",
                    IsStatic = true,
                    Type = "EditorTypeInfo",
                    Value = "ServerTypingsTest." + name
                }
            ]
        };
    }

    [Fact]
    public void Generates_LazyTypeLoader_With_Literal_Imports_Only_For_Marked_Types()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(LazyEditor("MarkedEditor", "@serenity-is/corelib:LazyLoadInclude"));
        generator.AddTSType(LazyEditor("ShortMarkedEditor", "LazyLoadInclude"));
        generator.AddTSType(LazyEditor("PlainEditor", null));
        generator.AddTSType(LazyEditor("OtherEditor", "IStringValue"));

        var code = Assert.Single(generator.Run(), x => x.Filename == "LazyTypeLoader.mts").Text;

        Assert.Contains("export const loaderByKey", code);
        Assert.Contains("\"ServerTypingsTest.MarkedEditor\": async () => (await import(\"", code);
        Assert.Contains("Editors/MarkedEditor\")).MarkedEditor", code);
        Assert.Contains("\"ServerTypingsTest.ShortMarkedEditor\"", code);
        Assert.DoesNotContain("PlainEditor", code);
        Assert.DoesNotContain("OtherEditor", code);
        Assert.DoesNotContain("Config", code);
    }

    [Fact]
    public void Does_Not_Generate_LazyTypeLoader_Without_A_Marked_Type()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(LazyEditor("PlainEditor", null));

        Assert.DoesNotContain(generator.Run(), x => x.Filename == "LazyTypeLoader.mts");
    }

    [Fact]
    public void LazyTypeLoader_Is_Not_Added_To_The_Module_ReExports()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.ModuleReExports = true;
        generator.AddTSType(LazyEditor("MarkedEditor", "LazyLoadInclude"));

        var files = generator.Run();

        Assert.Contains(files, x => x.Filename == "LazyTypeLoader.mts");
        Assert.DoesNotContain(files, x => x.Text.Contains("export * from", StringComparison.Ordinal) &&
            x.Text.Contains("LazyTypeLoader", StringComparison.Ordinal));
    }
}
