namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Node_Module_Package_Types()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/src/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "name": "module-a",
                "types": "src/index.d.ts"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/src/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("module-a", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/src/index.d.ts"), result.FullPath);
    }
}
