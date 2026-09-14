namespace Serenity.CodeGenerator;

public partial class EntityCodeGeneratorTests
{
    [Fact]
    public void Run_GenerateUI_Merges_Navigation_Into_Existing_File()
    {
        var generator = Create(out var fs, out _, out _, out var config);
        config.GenerateUI = true;
        var navigationFile = "/app/Modules/TestModule/TestModuleNavigation.cs";
        AddFile(fs, navigationFile, """
            namespace App.Modules.TestModule
            {
                public class TestModuleNavigation
                {
                }
            }
            """);

        generator.Run();

        var text = fs.ReadAllText(navigationFile);
        Assert.Contains("using Serenity.Navigation;", text);
        Assert.Contains("using MyPages = TestNamespace.TestModule.Pages", text);
        Assert.Contains("[assembly: NavigationLink", text);
        Assert.Contains("typeof(MyPages.CustomerPage)", text);
        Assert.Contains("public class TestModuleNavigation", text);
        var usings = text.Replace("\r", "", StringComparison.Ordinal)
            .Split('\n').Count(x => x.StartsWith("using ", StringComparison.Ordinal));
        Assert.Equal(2, usings);
    }

    [Fact]
    public void Run_GenerateUI_Does_Not_Duplicate_Usings_On_Repeated_Navigation()
    {
        var generator = Create(out var fs, out _, out _, out var config);
        config.GenerateUI = true;
        var navigationFile = "/app/Modules/TestModule/TestModuleNavigation.cs";
        AddFile(fs, navigationFile, """
            using Serenity.Navigation;
            using MyPages = TestNamespace.TestModule.Pages;

            namespace App.Modules.TestModule
            {
                public class TestModuleNavigation
                {
                }
            }
            """);

        generator.Run();

        var text = fs.ReadAllText(navigationFile).Replace("\r", "", StringComparison.Ordinal);
        Assert.Equal(2, text.Split('\n').Count(x => x.StartsWith("using ", StringComparison.Ordinal)));
    }
}
