using System.IO;

namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Dependency_NuGet_Exact_Version()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "@serenity-is/some-pkg": "1.2.3"
                }
            }
            """);

        var packageDir = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".nuget", "packages",
            "serenity.some.pkg", "1.2.3", "dist");
        fileSystem.CreateDirectory(packageDir);
        fileSystem.WriteAllText(Path.Combine(packageDir, "index.d.ts"), "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("@serenity-is/some-pkg", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("@serenity-is/some-pkg", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, Path.Combine(packageDir, "index.d.ts")), result.ActualPath);
    }
}
