using System.IO;

namespace Serenity.CodeGenerator;

public partial class TSModuleResolverTests
{
    [Fact]
    public void Resolves_Dependency_NuGet_Company_Scoped_Id()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory(root);
        fileSystem.WriteAllText(root + "package.json", /*lang=json*/ """
            {
                "dependencies": {
                    "@some-company/my.lib": "3.0.0"
                }
            }
            """);

        var packageDir = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".nuget", "packages",
            "some.company.my.lib", "3.0.0", "dist");
        fileSystem.CreateDirectory(packageDir);
        fileSystem.WriteAllText(Path.Combine(packageDir, "index.d.ts"), "");

        var resolver = CreateResolver(fileSystem);
        var result = resolver.Resolve("@some-company/my.lib", root + "a.ts");

        Assert.NotNull(result);
        Assert.Equal("@some-company/my.lib", result.ModuleName);
        Assert.Equal(FullPath(fileSystem, Path.Combine(packageDir, "index.d.ts")), result.ActualPath);
    }
}
