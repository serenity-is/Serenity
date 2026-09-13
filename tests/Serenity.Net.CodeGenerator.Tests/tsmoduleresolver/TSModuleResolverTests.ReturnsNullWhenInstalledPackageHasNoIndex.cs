using System.IO;

namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Returns_Null_When_Installed_Package_Has_No_Index()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "some-pkg": "^1.0.0"
                }
            }
            """);

        var packageDir = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".nuget", "packages",
            "some.pkg", "1.2.3", "dist");
        fileSystem.CreateDirectory(packageDir);

        var resolver = CreateResolver(fileSystem);

        Assert.Null(resolver.Resolve("some-pkg", root + "a.ts"));
    }
}
