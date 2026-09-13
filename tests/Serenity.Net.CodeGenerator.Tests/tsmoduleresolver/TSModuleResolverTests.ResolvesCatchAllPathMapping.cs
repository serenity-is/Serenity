namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_CatchAll_Path_Mapping()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "src/");
        fileSystem.WriteAllText(root + "src/b.ts", "");

        var resolver = CreateResolver(fileSystem, new TSConfig
        {
            CompilerOptions = new()
            {
                Paths = new() { ["*"] = ["./src/*"] }
            }
        });
        var result = resolver.Resolve("b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/src/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "src/b.ts"), result.FullPath);
    }
}
