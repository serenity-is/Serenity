namespace Serenity.CodeGenerator;

public partial class MvcCommandTests
{

    MvcCommand CreateCommand(string[] viewPaths, out MockFileSystem fileSystem)
    {
        fileSystem = new MockFileSystem();
        var project = new ProjectFileInfo(fileSystem, @"/Repos/MyTest.Web/MyTest.Web.csproj");
        var directory = fileSystem.GetDirectoryName(project.ProjectFile);
        fileSystem.AddFile(project.ProjectFile, "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>");
        fileSystem.AddFile(fileSystem.Combine(directory, "sergen.json"),
            "{\"MVC\": {\"UseRootNamespace\": true}}");
        var command = new MvcCommand(project, new MockGeneratorConsole());
        foreach (var viewPath in viewPaths)
            fileSystem.AddFile(fileSystem.Combine(directory, PathHelper.ToPath(viewPath)), "");
        return command;
    }

    [Fact]
    public void SameSubFolderNameInDifferentParents()
    {
        var expected =
@"namespace MyTest
{
    public static partial class MVC
    {
        public static partial class Views
        {
            public static partial class MyModule
            {
                public static partial class A
                {
                    public static partial class Same
                    {
                        public const string A1 = ""~/Modules/MyModule/A/Same/A1.cshtml"";
                        public const string A2 = ""~/Modules/MyModule/A/Same/A2.cshtml"";
                        public const string A3 = ""~/Modules/MyModule/A/Same/A3.cshtml"";
                    }
                }

                public static partial class B
                {
                    public static partial class Same
                    {
                        public const string B1 = ""~/Modules/MyModule/B/Same/B1.cshtml"";
                        public const string B2 = ""~/Modules/MyModule/B/Same/B2.cshtml"";
                    }
                }
            }
        }
    }
}".ReplaceLineEndings();

        var command = CreateCommand([
            "Modules/MyModule/A/Same/A1.cshtml",
            "Modules/MyModule/A/Same/A2.cshtml",
            "Modules/MyModule/A/Same/A3.cshtml",
            "Modules/MyModule/B/Same/B1.cshtml",
            "Modules/MyModule/B/Same/B2.cshtml"], out var fileSystem);

        var exitCode = command.Run();
        Assert.Equal(ExitCodes.Success, exitCode);

        var generated = fileSystem.ReadAllText(@"/Repos/MyTest.Web/Imports/MVC/MVC.cs").Trim().ReplaceLineEndings();
        Assert.Equal(expected, generated);
    }
}
