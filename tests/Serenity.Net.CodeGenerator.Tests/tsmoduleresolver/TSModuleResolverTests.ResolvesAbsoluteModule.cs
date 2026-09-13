namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Absolute_Module()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "x/y/");
        fileSystem.CreateDirectory(root + "z/");
        fileSystem.WriteAllText(root + "z/b.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("/z/b", root + "x/y/a.ts");

        Assert.NotNull(result);
        Assert.Equal("/z/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "z/b.ts"), result.FullPath);
    }
}
