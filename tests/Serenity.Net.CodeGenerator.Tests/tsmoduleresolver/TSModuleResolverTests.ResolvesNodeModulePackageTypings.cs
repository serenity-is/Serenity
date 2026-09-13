namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Node_Module_Package_Typings()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "typings": "index.d.ts"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("module-a", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/index.d.ts"), result.FullPath);
    }
}
