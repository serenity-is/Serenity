namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Finds_Dependencies_From_Parent_Directory()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "sub/");
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "my-lib": "file:./libs/my-lib"
                }
            }
            """);
        fileSystem.CreateDirectory(root + "libs/my-lib/dist/");
        fileSystem.WriteAllText(root + "libs/my-lib/dist/index.d.ts", "");

        var resolver = CreateResolver(fileSystem, tsConfigDir: root + "sub/");
        var result = resolver.Resolve("my-lib", root + "sub/a.ts");

        Assert.NotNull(result);
        Assert.Equal("my-lib", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/my-lib"), result.FullPath);
        Assert.Equal(FullPath(fileSystem, root + "libs/my-lib/dist/index.d.ts"), result.ActualPath);
    }
}
