namespace Serenity.CodeGenerator;

public partial class ProjectFileInfoTests
{
    [Fact]
    public void GetAssemblyName_Prefers_PropertyArgument_Over_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><AssemblyName>FromCsproj</AssemblyName></PropertyGroup></Project>",
            getProperty: name => name == "AssemblyName" ? "FromCallback" : null);
        Assert.Equal("FromCallback", info.GetAssemblyName());
    }

    [Fact]
    public void Exposes_FileSystem_And_ProjectFile()
    {
        var info = Create(out var fileSystem);
        Assert.Equal(projectFile, info.ProjectFile);
        Assert.Same(fileSystem, info.FileSystem);
    }

    [Fact]
    public void GetAssemblyName_Reads_From_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><AssemblyName>FromCsproj</AssemblyName></PropertyGroup></Project>");
        Assert.Equal("FromCsproj", info.GetAssemblyName());
    }

    [Fact]
    public void GetAssemblyName_Returns_Null_When_Absent()
    {
        var info = Create(out _);
        Assert.Null(info.GetAssemblyName());
    }

    [Fact]
    public void GetRootNamespace_Reads_From_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><RootNamespace>My.Root</RootNamespace></PropertyGroup></Project>");
        Assert.Equal("My.Root", info.GetRootNamespace());
    }

    [Fact]
    public void GetRootNamespace_Prefers_PropertyArgument()
    {
        var info = Create(out _, getProperty: name => name == "RootNamespace" ? "FromCallback" : null);
        Assert.Equal("FromCallback", info.GetRootNamespace());
    }

    [Fact]
    public void GetEsmAssetBasePath_Reads_From_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><ESMAssetBasePath>Modules/</ESMAssetBasePath></PropertyGroup></Project>");
        Assert.Equal("Modules/", info.GetEsmAssetBasePath());
    }

    [Fact]
    public void GetEsmAssetBasePath_Prefers_PropertyArgument()
    {
        var info = Create(out _, getProperty: name => name == "ESMAssetBasePath" ? "Callback/Path" : null);
        Assert.Equal("Callback/Path", info.GetEsmAssetBasePath());
    }

    [Fact]
    public void GetNullable_Reads_From_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><Nullable>enable</Nullable></PropertyGroup></Project>");
        Assert.Equal("enable", info.GetNullable());
    }

    [Fact]
    public void GetNullable_Prefers_PropertyArgument()
    {
        var info = Create(out _, getProperty: name => name == "Nullable" ? "disable" : null);
        Assert.Equal("disable", info.GetNullable());
    }

    [Fact]
    public void GetOutDir_Prefers_OutDir_And_Combines_With_Project_Directory()
    {
        var info = Create(out _, getProperty: name => name == "OutDir" ? "bin/custom" : null);
        Assert.EndsWith("bin/custom", info.GetOutDir().Replace('\\', '/'));
    }

    [Fact]
    public void GetOutDir_Falls_Back_To_OutputPath_Property()
    {
        var info = Create(out _, getProperty: name => name == "OutputPath" ? "bin/output" : null);
        Assert.EndsWith("bin/output", info.GetOutDir().Replace('\\', '/'));
    }

    [Fact]
    public void GetTargetFramework_Reads_From_Csproj()
    {
        var info = Create(out _, "<Project><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>");
        Assert.Equal("net8.0", info.GetTargetFramework());
    }

    [Fact]
    public void GetTargetFramework_Reads_Single_TargetFrameworks_Element()
    {
        var info = Create(out _, "<Project><PropertyGroup></PropertyGroup><PropertyGroup><TargetFrameworks>net8.0</TargetFrameworks></PropertyGroup></Project>");
        Assert.Equal("net8.0", info.GetTargetFramework());
    }

    [Fact]
    public void GetTargetFramework_Returns_Null_For_Multiple_TargetFrameworks()
    {
        var info = Create(out _, "<Project><PropertyGroup><TargetFrameworks>net8.0;net9.0</TargetFrameworks></PropertyGroup></Project>");
        Assert.Null(info.GetTargetFramework());
    }

    [Fact]
    public void Uses_Last_Value_When_Property_Is_Declared_Multiple_Times()
    {
        var info = Create(out _, "<Project><PropertyGroup><AssemblyName>First</AssemblyName><AssemblyName>Second</AssemblyName></PropertyGroup></Project>");
        Assert.Equal("Second", info.GetAssemblyName());
    }

    [Fact]
    public void Conditioned_Property_Is_Treated_As_Empty()
    {
        var info = Create(out _, "<Project><PropertyGroup><AssemblyName Condition=\"'$(X)'=='y'\">Conditional</AssemblyName></PropertyGroup></Project>");
        Assert.Null(info.GetAssemblyName());
    }

    [Fact]
    public void Property_With_Complex_Value_Is_Treated_As_Empty()
    {
        var info = Create(out _, "<Project><PropertyGroup><AssemblyName>$(MSBuildProjectName)</AssemblyName></PropertyGroup></Project>");
        Assert.Null(info.GetAssemblyName());
    }
}
