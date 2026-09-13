namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Uses_Package_Name_From_Package_Json()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "name": "actual-package-name",
                "types": "index.d.ts"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("actual-package-name", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/index.d.ts"), result.FullPath);
    }
}
