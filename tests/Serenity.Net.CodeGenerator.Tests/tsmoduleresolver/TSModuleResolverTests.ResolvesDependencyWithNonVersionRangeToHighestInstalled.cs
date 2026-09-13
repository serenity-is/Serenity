using System.IO;

namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Dependency_With_Non_Version_Range_To_Highest_Installed()
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

        var packagesRoot = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".nuget", "packages", "some.pkg");
        var version123 = Path.Combine(packagesRoot, "1.2.3", "dist");
        var version200 = Path.Combine(packagesRoot, "2.0.0", "dist");
        fileSystem.CreateDirectory(version123);
        fileSystem.CreateDirectory(version200);
        fileSystem.WriteAllText(Path.Combine(version123, "index.d.ts"), "");
        fileSystem.WriteAllText(Path.Combine(version200, "index.d.ts"), "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("some-pkg", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("some-pkg", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, Path.Combine(version200, "index.d.ts")), result.ActualPath);
    }
}
