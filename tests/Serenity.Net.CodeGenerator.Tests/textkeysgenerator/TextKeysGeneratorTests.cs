using Serenity.CodeGeneration;
using Serenity.Testing;
using System.Text.RegularExpressions;

namespace Serenity.CodeGenerator;

public class TextKeysGeneratorTests
{
    private const string Template = """
        {
          "Db.Northwind.Customer.Fax": "Fax",
          "Db.Northwind.Customer.City": "City",
          "Site.Layout.Logout": "Logout"
        }
        """;

    private static (MockFileSystem FileSystem, MockGeneratorConsole Console, string ProjectDir) Create()
    {
        var fileSystem = new MockFileSystem();
        var projectDir = fileSystem.Combine("project");
        fileSystem.CreateDirectory(projectDir);
        return (fileSystem, new MockGeneratorConsole(), projectDir);
    }

    private static void AddTemplate(MockFileSystem fileSystem, string projectDir, string name = "site",
        string template = Template)
    {
        var path = fileSystem.Combine(projectDir, "texts", "resources", name + ".texts.__.json");
        fileSystem.CreateDirectory(fileSystem.GetDirectoryName(path));
        fileSystem.WriteAllText(path, template);
    }

    private static string KeysOf(MockFileSystem fileSystem, string projectDir)
    {
        var file = fileSystem.Combine(projectDir, TextKeysGenerator.FileName);
        Assert.True(fileSystem.FileExists(file));
        // the value of each constant is its key, and the keys of these tests have nothing to escape
        var keys = Regex.Matches(fileSystem.ReadAllText(file), @"public const string \w+ = ""([^""]*)"";")
            .Select(x => x.Groups[1].Value)
            .OrderBy(x => x, StringComparer.Ordinal);
        return string.Join("|", keys);
    }

    [Fact]
    public void Writes_The_Keys_Of_The_Text_Resources_Of_The_Project()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            RootNamespace = "StartSharp"
        }.Run();

        Assert.Equal("Db.Northwind.Customer.City|Db.Northwind.Customer.Fax|Site.Layout.Logout",
            KeysOf(fileSystem, projectDir));

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName));
        Assert.Contains("namespace StartSharp", source);
        // the summary of a constant is the text of its key, which the name of the constant does not need to repeat
        Assert.Contains("/// <summary>Fax</summary>", source);
    }

    [Fact]
    public void Does_Not_Write_The_File_Again_When_It_Has_Not_Changed()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        for (var i = 0; i < 2; i++)
        {
            new TextKeysGenerator(fileSystem, console)
            {
                ProjectDir = projectDir,
                RootNamespace = "StartSharp"
            }.Run();
        }

        // the command runs before every build, and a file that is written again has a new timestamp, which makes the
        // build compile every time, so the message that says it wrote the file is shown once
        Assert.Single(console.WriteCalls, x => x.message is not null &&
            x.message.StartsWith("Wrote 3 text key constants", StringComparison.Ordinal));
    }

    [Fact]
    public void Deletes_The_File_When_The_Project_Has_No_Text_Resources_Any_More()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console) { ProjectDir = projectDir }.Run();
        var file = fileSystem.Combine(projectDir, TextKeysGenerator.FileName);
        Assert.True(fileSystem.FileExists(file));

        fileSystem.DeleteFile(fileSystem.Combine(projectDir, "texts", "resources", "site.texts.__.json"));
        new TextKeysGenerator(fileSystem, console) { ProjectDir = projectDir }.Run();

        // a file that is left behind is compiled, and carries the keys of a build that is gone
        Assert.False(fileSystem.FileExists(file));
    }

    [Fact]
    public void Deletes_The_File_When_The_Project_Does_Not_Want_The_Constants()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console) { ProjectDir = projectDir }.Run();
        var file = fileSystem.Combine(projectDir, TextKeysGenerator.FileName);
        Assert.True(fileSystem.FileExists(file));

        new TextKeysGenerator(fileSystem, console) { ProjectDir = projectDir, TextKeys = false }.Run();

        Assert.False(fileSystem.FileExists(file));
    }

    [Fact]
    public void Merges_The_Templates_Of_The_Packages_Of_The_Project()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir, "site");
        AddTemplate(fileSystem, projectDir, "dataexplorer", """{ "Dataexplorer.Dashboard.Title": "Dashboard" }""");

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir
        }.Run();

        Assert.Contains("Dataexplorer.Dashboard.Title", KeysOf(fileSystem, projectDir));
        Assert.Contains("Db.Northwind.Customer.Fax", KeysOf(fileSystem, projectDir));
    }

    [Fact]
    public void Writes_Nothing_When_The_Project_Has_No_Text_Resources()
    {
        var (fileSystem, console, projectDir) = Create();

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            TextKeys = null
        }.Run();

        // a project that takes the constants if its texts are there says nothing when they are not
        Assert.False(fileSystem.FileExists(fileSystem.Combine(projectDir, TextKeysGenerator.FileName)));
        Assert.DoesNotContain(console.WriteCalls, x => (x.message ?? "").Contains("template"));
    }

    [Fact]
    public void Tells_A_Project_That_Asks_For_Them_That_It_Has_No_Text_Resources()
    {
        var (fileSystem, console, projectDir) = Create();

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            TextKeys = true
        }.Run();

        Assert.False(fileSystem.FileExists(fileSystem.Combine(projectDir, TextKeysGenerator.FileName)));
        Assert.Contains(console.WriteCalls, x => (x.message ?? "").Contains("has no template of them"));
    }

    [Fact]
    public void Writes_Nothing_When_The_Project_Does_Not_Want_Them()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            TextKeys = false
        }.Run();

        Assert.False(fileSystem.FileExists(fileSystem.Combine(projectDir, TextKeysGenerator.FileName)));
        Assert.Empty(console.WriteCalls);
    }

    [Fact]
    public void Writes_The_File_To_The_Directory_Of_The_Mvc_Command()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            OutDir = fileSystem.Combine(projectDir, "Imports", "MVC")
        }.Run();

        Assert.True(fileSystem.FileExists(fileSystem.Combine(projectDir, "Imports", "MVC", TextKeysGenerator.FileName)));
        Assert.False(fileSystem.FileExists(fileSystem.Combine(projectDir, TextKeysGenerator.FileName)));
    }

    [Fact]
    public void Makes_The_Class_Available_As_TextKeys_With_A_Global_Using()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            RootNamespace = "MyApp.MVC"
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName))
            .Replace("\r\n", "\n");

        // a file of the project writes TextKeys rather than the namespace that the class is under
        Assert.Contains("global using TextKeys = MyApp.MVC.TextKeys;\n", source);
        Assert.True(source.IndexOf("global using TextKeys = MyApp.MVC.TextKeys;") <
            source.IndexOf("namespace MyApp.MVC;"), source);
    }

    [Fact]
    public void Does_Not_Make_The_Class_Available_As_TextKeys_When_The_Project_Does_Not_Want_It()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            RootNamespace = "MyApp.MVC",
            UsingAlias = false
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName));
        Assert.DoesNotContain("global using", source);
    }

    [Fact]
    public void Writes_The_Outermost_Class_As_An_Internal_One_When_The_Project_Does()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            InternalAccess = true
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName))
            .Replace("\r\n", "\n");

        // the classes under it are public, as the one that a project hides is the outermost
        Assert.Contains("internal static partial class TextKeys\n", source);
        Assert.Contains("public static partial class Db\n", source);
        Assert.DoesNotContain("public static partial class TextKeys", source);
    }

    [Fact]
    public void Writes_The_Namespace_Of_The_Mvc_Command_As_A_File_Scoped_One()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            RootNamespace = "Serenity.Extensions.MVC"
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName))
            .Replace("\r\n", "\n");
        Assert.Contains("namespace Serenity.Extensions.MVC;\n", source);
    }

    [Fact]
    public void Writes_The_Namespace_As_A_Block_When_The_Project_Does()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            RootNamespace = "Serenity.Extensions.MVC",
            FileScopedNamespaces = false
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName));
        Assert.Contains("namespace Serenity.Extensions.MVC\n{\n", source.Replace("\r\n", "\n"));
        Assert.DoesNotContain("namespace Serenity.Extensions.MVC;", source);
    }

    [Fact]
    public void Uses_The_End_Of_Line_Of_The_Project()
    {
        var (fileSystem, console, projectDir) = Create();
        AddTemplate(fileSystem, projectDir);

        new TextKeysGenerator(fileSystem, console)
        {
            ProjectDir = projectDir,
            EndOfLine = "LF"
        }.Run();

        var source = fileSystem.ReadAllText(fileSystem.Combine(projectDir, TextKeysGenerator.FileName));
        Assert.DoesNotContain("\r", source);
        Assert.Contains("\n", source);
    }
}
