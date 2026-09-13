namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Walks_Up_To_Find_Node_Modules()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "sub/deep/");
        fileSystem.CreateDirectory(root + "node_modules/module-a/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "types": "index.d.ts"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a", root + "sub/deep/a.ts");

        Assert.NotNull(result);
        Assert.Equal("module-a", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/index.d.ts"), result.FullPath);
    }
}
