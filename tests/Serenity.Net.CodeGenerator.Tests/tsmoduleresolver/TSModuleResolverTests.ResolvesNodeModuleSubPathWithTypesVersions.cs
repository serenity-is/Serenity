namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Node_Module_SubPath_With_TypesVersions()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/sub/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "name": "module-a",
                "typesVersions": {
                    "*": {
                        "sub": ["sub/index.d.ts"]
                    }
                }
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/module-a/sub/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("module-a/sub", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("module-a/sub", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/module-a/sub/index.d.ts"), result.FullPath);
    }
}
