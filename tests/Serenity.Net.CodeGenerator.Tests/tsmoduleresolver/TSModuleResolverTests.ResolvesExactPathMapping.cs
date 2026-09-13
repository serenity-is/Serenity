namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Exact_Path_Mapping()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "src/");
        fileSystem.WriteAllText(root + "src/lib.ts", "");

        var resolver = CreateResolver(fileSystem, new TSConfig
        {
            CompilerOptions = new()
            {
                Paths = new() { ["lib"] = ["./src/lib"] }
            }
        });
        var result = resolver.Resolve("lib", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/src/lib", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "src/lib.ts"), result.FullPath);
    }
}
