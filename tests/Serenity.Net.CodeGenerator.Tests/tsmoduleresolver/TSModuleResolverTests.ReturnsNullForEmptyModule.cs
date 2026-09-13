namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_For_Empty_Module()
    {
        var fileSystem = new MockFileSystem();
        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve(null, null));
        Assert.Null(resolver.Resolve("", "/root/a.ts"));
    }
}
