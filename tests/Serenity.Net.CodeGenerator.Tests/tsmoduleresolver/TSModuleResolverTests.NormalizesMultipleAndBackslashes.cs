namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Normalizes_Multiple_And_Backslashes()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "sub/");
        fileSystem.WriteAllText(root + "sub/b.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve(".\\sub//b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/sub/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "sub/b.ts"), result.FullPath);
    }
}
