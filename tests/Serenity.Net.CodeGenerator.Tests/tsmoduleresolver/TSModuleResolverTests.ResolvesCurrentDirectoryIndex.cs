namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Current_Directory_Index()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "index.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve(".", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "index.ts"), result.FullPath);
    }
}
