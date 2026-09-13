namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Directory_Index()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "folder/");
        fileSystem.WriteAllText(root + "folder/index.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./folder", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/folder/", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "folder/index.ts"), result.FullPath);
    }
}
