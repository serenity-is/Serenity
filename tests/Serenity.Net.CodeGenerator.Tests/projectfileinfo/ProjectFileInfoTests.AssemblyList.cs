namespace Serenity.CodeGenerator;

public partial class ProjectFileInfoTests
{
    [Fact]
    public void GetAssemblyList_Resolves_Dll_From_OutDir_And_AssemblyName()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "OutDir" => "bin/out",
            _ => null
        }, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/out/MyApp.dll");

        var list = info.GetAssemblyList(null);

        Assert.NotNull(list);
        Assert.Single(list);
        Assert.EndsWith("bin/out/MyApp.dll", list[0].Replace('\\', '/'));
        Assert.Empty(errors);
    }

    [Fact]
    public void GetAssemblyList_Resolves_Exe_When_Dll_Is_Missing()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "OutDir" => "bin/out",
            _ => null
        }, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/out/MyApp.exe");

        var list = info.GetAssemblyList(null);

        Assert.NotNull(list);
        Assert.EndsWith("bin/out/MyApp.exe", list[0].Replace('\\', '/'));
    }

    [Fact]
    public void GetAssemblyList_Reports_Error_When_Output_Not_Found()
    {
        var errors = new List<string>();
        var info = Create(out _, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "OutDir" => "bin/out",
            _ => null
        }, onError: errors.Add);

        var list = info.GetAssemblyList(null);

        Assert.Null(list);
        Assert.Contains(errors, e => e.Contains("Couldn't find output file at"));
    }

    [Fact]
    public void GetAssemblyList_Uses_Debug_Folder_When_OutDir_Is_Not_Known()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "TargetFramework" => "net8.0",
            _ => null
        }, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/Debug/net8.0/MyApp.dll");

        var list = info.GetAssemblyList(null);

        Assert.NotNull(list);
        Assert.EndsWith("bin/Debug/net8.0/MyApp.dll", list[0].Replace('\\', '/'));
    }

    [Fact]
    public void GetAssemblyList_Prefers_Newer_Release_Folder()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "TargetFramework" => "net8.0",
            _ => null
        }, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/Debug/net8.0/MyApp.dll");
        AddFile(fileSystem, "/app/bin/Release/net8.0/MyApp.dll");
        fileSystem.File.SetLastWriteTimeUtc("/app/bin/Release/net8.0/MyApp.dll", DateTime.UtcNow.AddMinutes(5));

        var list = info.GetAssemblyList(null);

        Assert.NotNull(list);
        Assert.EndsWith("bin/Release/net8.0/MyApp.dll", list[0].Replace('\\', '/'));
    }

    [Fact]
    public void GetAssemblyList_Reports_Error_When_TargetFramework_Not_Found()
    {
        var errors = new List<string>();
        var info = Create(out _, getProperty: name => name == "AssemblyName" ? "MyApp" : null,
            onError: errors.Add);

        var list = info.GetAssemblyList(null);

        Assert.Null(list);
        Assert.Contains(errors, e => e.Contains("Couldn't read TargetFramework"));
    }

    [Fact]
    public void GetAssemblyList_Uses_Configured_Assembly_Path()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/Debug/net8.0/MyApp.dll");

        var list = info.GetAssemblyList(["/app/bin/Debug/net8.0/MyApp.dll"]);

        Assert.NotNull(list);
        Assert.Single(list);
        Assert.EndsWith("bin/Debug/net8.0/MyApp.dll", list[0].Replace('\\', '/'));
    }

    [Fact]
    public void GetAssemblyList_Prefers_Release_For_Configured_Debug_Path()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/Debug/net8.0/MyApp.dll");
        AddFile(fileSystem, "/app/bin/Release/net8.0/MyApp.dll");
        fileSystem.File.SetLastWriteTimeUtc("/app/bin/Release/net8.0/MyApp.dll", DateTime.UtcNow.AddMinutes(5));

        var list = info.GetAssemblyList(["/app/bin/Debug/net8.0/MyApp.dll"]);

        Assert.NotNull(list);
        Assert.EndsWith("bin/Release/net8.0/MyApp.dll", list[0].Replace('\\', '/'));
    }

    [Fact]
    public void GetAssemblyList_Reports_Error_For_Missing_Configured_Assembly()
    {
        var errors = new List<string>();
        var info = Create(out _, onError: errors.Add);

        var list = info.GetAssemblyList(["/app/bin/Debug/net8.0/Missing.dll"]);

        Assert.Null(list);
        Assert.Contains(errors, e => e.Contains("is not found"));
    }

    [Fact]
    public void GetAssemblyList_Reports_Error_When_No_Build_Output_Exists()
    {
        var errors = new List<string>();
        var info = Create(out _, getProperty: name => name switch
        {
            "AssemblyName" => "MyApp",
            "TargetFramework" => "net8.0",
            _ => null
        }, onError: errors.Add);

        var list = info.GetAssemblyList(null);

        Assert.Null(list);
        Assert.Contains(errors, e => e.Contains("Couldn't find output file at"));
    }

    [Fact]
    public void GetAssemblyList_Falls_Back_To_Release_For_Configured_Debug_Path()
    {
        var errors = new List<string>();
        var info = Create(out var fileSystem, onError: errors.Add);
        AddFile(fileSystem, "/app/bin/Release/net8.0/MyApp.dll");

        var list = info.GetAssemblyList(["/app/bin/Debug/net8.0/MyApp.dll"]);

        Assert.NotNull(list);
        Assert.EndsWith("bin/Release/net8.0/MyApp.dll", list[0].Replace('\\', '/'));
    }
}
