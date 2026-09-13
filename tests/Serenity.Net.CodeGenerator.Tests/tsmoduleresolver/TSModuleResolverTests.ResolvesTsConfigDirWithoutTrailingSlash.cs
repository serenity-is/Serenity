namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_TsConfigDir_Without_Trailing_Slash()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "a.ts", "");

        var resolver = CreateResolver(fileSystem, tsConfigDir: "/root");
        var result = resolver.Resolve("/root/a.ts", null);

        Assert.NotNull(result);
        Assert.Equal("/a", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "a.ts"), result.FullPath);
    }
}
