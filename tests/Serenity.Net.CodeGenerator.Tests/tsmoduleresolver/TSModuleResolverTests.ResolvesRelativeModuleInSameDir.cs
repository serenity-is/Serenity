namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Relative_Module_In_Same_Dir()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "b.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "b.ts"), result.FullPath);
    }
}
