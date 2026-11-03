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

    private static ExternalType GroupedLazyEditor(string name, string? group, string? folder = null)
    {
        var type = LazyEditor(name, "LazyLoadInclude");
        type.LazyLoadGroup = group;
        if (folder != null)
        {
            type.Module = "/Modules/" + folder + "/" + name;
            type.Fields![0].Value = "ServerTypingsTest." + folder + "." + name;
        }
        return type;
    }

    [Fact]
    public void Lazy_Group_Types_Are_Loaded_From_A_Group_Module_While_Others_Keep_Their_Own_Import()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(GroupedLazyEditor("FirstEditor", "Viewer"));
        generator.AddTSType(GroupedLazyEditor("SecondEditor", "Viewer"));
        generator.AddTSType(GroupedLazyEditor("ThirdEditor", "Designer"));
        generator.AddTSType(GroupedLazyEditor("OwnChunkEditor", null));

        var files = generator.Run();
        var loader = Assert.Single(files, x => x.Filename == "LazyTypeLoader.mts").Text;
        var viewer = Assert.Single(files, x => x.Filename == "LazyTypeGroup.Viewer.ts").Text;
        var designer = Assert.Single(files, x => x.Filename == "LazyTypeGroup.Designer.ts").Text;

        Assert.Contains("\"ServerTypingsTest.FirstEditor\": async () => (await import(\"./LazyTypeGroup.Viewer\")).lazyTypeGroupViewer[\"ServerTypingsTest.FirstEditor\"]", loader);
        Assert.Contains("\"ServerTypingsTest.SecondEditor\": async () => (await import(\"./LazyTypeGroup.Viewer\")).lazyTypeGroupViewer[\"ServerTypingsTest.SecondEditor\"]", loader);
        Assert.Contains("\"ServerTypingsTest.ThirdEditor\": async () => (await import(\"./LazyTypeGroup.Designer\")).lazyTypeGroupDesigner[\"ServerTypingsTest.ThirdEditor\"]", loader);
        Assert.Contains("\"ServerTypingsTest.OwnChunkEditor\": async () => (await import(\"", loader);
        Assert.Contains("Editors/OwnChunkEditor\")).OwnChunkEditor", loader);

        Assert.Contains("import { FirstEditor } from \"", viewer);
        Assert.Contains("Editors/FirstEditor\";", viewer);
        Assert.Contains("\"ServerTypingsTest.FirstEditor\": FirstEditor", viewer);
        Assert.Contains("\"ServerTypingsTest.SecondEditor\": SecondEditor", viewer);
        Assert.DoesNotContain("ThirdEditor", viewer);
        Assert.Contains("export const lazyTypeGroupViewer:", viewer);
        Assert.DoesNotContain("export {", viewer);
        Assert.DoesNotContain("loaderByKey", viewer);

        Assert.Contains("\"ServerTypingsTest.ThirdEditor\": ThirdEditor", designer);
        Assert.DoesNotContain("LazyTypeGroup.Viewer", designer);
        Assert.DoesNotContain(files, x => x.Filename.Contains("OwnChunkEditor", StringComparison.Ordinal));
    }

    [Fact]
    public void Lazy_Group_Aliases_Classes_With_The_Same_Name()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(GroupedLazyEditor("SameEditor", "Viewer", "First"));
        generator.AddTSType(GroupedLazyEditor("SameEditor", "Viewer", "Second"));

        var code = Assert.Single(generator.Run(), x => x.Filename == "LazyTypeGroup.Viewer.ts").Text;

        Assert.Contains("import { SameEditor } from \"", code);
        Assert.Contains("import { SameEditor as SameEditor_2 } from \"", code);
        Assert.Contains("\"ServerTypingsTest.First.SameEditor\": SameEditor,", code);
        Assert.Contains("\"ServerTypingsTest.Second.SameEditor\": SameEditor_2", code);
    }

    [Fact]
    public void Lazy_Groups_Only_Differing_By_Case_Share_A_File()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(GroupedLazyEditor("FirstEditor", "Viewer"));
        generator.AddTSType(GroupedLazyEditor("SecondEditor", "viewer"));

        var files = generator.Run();
        var group = Assert.Single(files, x => x.Filename.StartsWith("LazyTypeGroup.", StringComparison.OrdinalIgnoreCase));
        var loader = Assert.Single(files, x => x.Filename == "LazyTypeLoader.mts").Text;

        Assert.Equal("LazyTypeGroup.Viewer.ts", group.Filename);
        Assert.Contains("FirstEditor", group.Text);
        Assert.Contains("SecondEditor", group.Text);
        Assert.DoesNotContain("./LazyTypeGroup.viewer", loader);
    }

    [Theory]
    [InlineData("../../Evil", "LazyTypeGroup.______Evil.ts")]
    [InlineData("With Space", "LazyTypeGroup.With_Space.ts")]
    [InlineData("With-Dash_1", "LazyTypeGroup.With_Dash_1.ts")]
    public void Lazy_Group_Name_Is_Made_Safe_For_A_File_Name(string group, string expected)
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.AddTSType(GroupedLazyEditor("MarkedEditor", group));

        Assert.Contains(generator.Run(), x => x.Filename == expected);
    }

    [Fact]
    public void Lazy_Group_Is_Not_Added_To_The_Module_ReExports()
    {
        var generator = CreateGenerator();
        generator.TypeFilter = _ => false;
        generator.ModuleReExports = true;
        generator.AddTSType(GroupedLazyEditor("MarkedEditor", "Viewer"));

        var files = generator.Run();

        Assert.Contains(files, x => x.Filename == "LazyTypeGroup.Viewer.ts");
        Assert.DoesNotContain(files, x => x.Text.Contains("export * from", StringComparison.Ordinal) &&
            x.Text.Contains("LazyTypeGroup", StringComparison.Ordinal));
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
