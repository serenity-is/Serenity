namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Prefix_Asterisk_Path_Mapping()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "zzz/");
        fileSystem.WriteAllText(root + "zzz/b.ts", "");

        var resolver = CreateResolver(fileSystem, new TSConfig
        {
            CompilerOptions = new()
            {
                Paths = new() { ["my/*"] = ["./zzz/*"] }
            }
        });
        var result = resolver.Resolve("my/b", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("/zzz/b", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, root + "zzz/b.ts"), result.FullPath);
    }
}
