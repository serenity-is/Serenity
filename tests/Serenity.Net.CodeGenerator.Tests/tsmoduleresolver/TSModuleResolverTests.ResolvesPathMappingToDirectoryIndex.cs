namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Path_Mapping_To_Directory_Index()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "src/my/");
        fileSystem.WriteAllText(root + "src/my/index.ts", "");

        var resolver = CreateResolver(fileSystem, new TSConfig
        {
            CompilerOptions = new()
            {
                Paths = new() { ["my"] = ["./src/my"] }
            }
        });
        var result = resolver.Resolve("my", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/src/my/", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "src/my/index.ts"), result.FullPath);
    }
}
