namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_When_Package_Json_Has_No_Types()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root + "node_modules/module-a/sub/");
        fileSystem.WriteAllText(root + "node_modules/module-a/package.json", /*lang=json*/ """
            {
                "name": "module-a"
            }
            """);

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("module-a/sub", root + "a.ts"));
    }
}
