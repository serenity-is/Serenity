namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Prefers_TS_Over_TSX_And_D_TS()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "b.ts", "");
        fileSystem.WriteAllText(root + "b.tsx", "");
        fileSystem.WriteAllText(root + "b.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal(FullPath(fileSystem, root + "b.ts"), result.FullPath);
    }
}
