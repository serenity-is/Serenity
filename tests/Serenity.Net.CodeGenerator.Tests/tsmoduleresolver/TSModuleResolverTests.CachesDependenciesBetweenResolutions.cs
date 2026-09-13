namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Caches_Dependencies_Between_Resolutions()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "lib-a": "file:./libs/a",
                    "lib-b": "file:./libs/b"
                }
            }
            """);
        fileSystem.CreateDirectory(root + "libs/a/dist/");
        fileSystem.CreateDirectory(root + "libs/b/dist/");
        fileSystem.WriteAllText(root + "libs/a/dist/index.d.ts", "");
        fileSystem.WriteAllText(root + "libs/b/dist/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);

        var resultA = resolver.Resolve("lib-a", root + "a.ts");
        var resultB = resolver.Resolve("lib-b", root + "b.ts");

        Assert.NotNull(resultA);
        Assert.Equal(FullPath(fileSystem, root + "libs/a/dist/index.d.ts"), resultA.ActualPath);
        Assert.NotNull(resultB);
        Assert.Equal(FullPath(fileSystem, root + "libs/b/dist/index.d.ts"), resultB.ActualPath);
    }
}
