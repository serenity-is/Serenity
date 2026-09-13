namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_TSX_When_Only_TSX_Exists()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "b.tsx", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "b.tsx"), result.FullPath);
    }
}
