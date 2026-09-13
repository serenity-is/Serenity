namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_For_Missing_File_Without_ReferencedFrom()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("/root/missing.ts", null));
    }
}
