namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Skips_Rooted_And_Empty_Path_Mappings()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "src/");
        fileSystem.WriteAllText(root + "src/my.ts", "");

        var resolver = CreateResolver(fileSystem, new TSConfig
        {
            CompilerOptions = new()
            {
                Paths = new() { ["my"] = ["/abs/my", "", "./src/my"] }
            }
        });
        var result = resolver.Resolve("my", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/src/my", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "src/my.ts"), result.FullPath);
    }
}
