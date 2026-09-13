namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Relative_Parent_Module()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "sub/");
        fileSystem.WriteAllText(root + "b.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("../b", root + "sub/a.ts");

        Assert.NotNull(result);
        Assert.Equal("/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "b.ts"), result.FullPath);
    }
}
