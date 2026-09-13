namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_When_Exact_Version_Not_Installed()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "some-pkg": "9.9.9"
                }
            }
            """);

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("some-pkg", root + "a.ts"));
    }
}
