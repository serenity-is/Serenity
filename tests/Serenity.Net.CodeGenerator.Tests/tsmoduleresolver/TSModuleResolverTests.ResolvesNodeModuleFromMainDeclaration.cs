namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Node_Module_From_Main_Declaration()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/dist/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "name": "module-a",
                "main": "dist/index.js"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/dist/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("module-a", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/dist/index.d.ts"), result.FullPath);
    }
}
