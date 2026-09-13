namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_When_Module_Cannot_Be_Resolved()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.CreateDirectory(root + "node_modules/");

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("missing-module", root + "a.ts"));
    }
}
