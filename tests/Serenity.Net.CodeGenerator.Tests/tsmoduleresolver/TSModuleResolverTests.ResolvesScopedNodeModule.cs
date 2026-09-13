namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Scoped_Node_Module()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/@scope/pkg/");
        fileSystem.WriteAllText(root + "node_modules/@scope/pkg/package.json", /*lang=json*/ """
            {
                "types": "index.d.ts"
            }
            """);
        fileSystem.WriteAllText(root + "node_modules/@scope/pkg/index.d.ts", "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("@scope/pkg", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("@scope/pkg", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "node_modules/@scope/pkg/index.d.ts"), result.FullPath);
    }
}
