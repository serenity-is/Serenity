namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_When_Relative_Dependency_Missing()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "my-lib": "file:./libs/missing"
                }
            }
            """);

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("my-lib", root + "a.ts"));
    }
}
