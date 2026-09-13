namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Relative_Module_With_JSX_Extension_To_DTS()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "comp.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("./comp.jsx", root + "a.tsx");

        Assert.NotNull(result);
        Assert.Equal("/comp", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "comp.d.ts"), result.FullPath);
    }
}
