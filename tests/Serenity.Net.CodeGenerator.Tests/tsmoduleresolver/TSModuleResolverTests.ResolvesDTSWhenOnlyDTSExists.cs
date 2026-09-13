namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_DTS_When_Only_DTS_Exists()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "b.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "b.d.ts"), result.FullPath);
    }
}
