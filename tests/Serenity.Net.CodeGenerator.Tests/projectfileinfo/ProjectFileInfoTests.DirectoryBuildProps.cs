namespace Serenity.CodeGenerator;

public partial class ProjectFileInfoTests
{
    [Fact]
    public void Reads_Directory_Build_Props_From_Parent_Directories()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/repo/app");
        fileSystem.WriteAllText("/repo/app/My.Web.csproj", "<Project></Project>");
        fileSystem.WriteAllText("/repo/Directory.Build.props",
            "<Project><PropertyGroup><AssemblyName>FromParent</AssemblyName></PropertyGroup></Project>");

        var info = new ProjectFileInfo(fileSystem, "/repo/app/My.Web.csproj");
        Assert.Equal("FromParent", info.GetAssemblyName());
    }

    [Fact]
    public void Project_Property_Takes_Precedence_Over_Directory_Build_Props()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.CreateDirectory("/app");
        fileSystem.WriteAllText("/app/My.Web.csproj",
            "<Project><PropertyGroup><RootNamespace>FromCsproj</RootNamespace></PropertyGroup></Project>");
        fileSystem.WriteAllText("/app/Directory.Build.props",
            "<Project><PropertyGroup><AssemblyName>AsmFromProps</AssemblyName><RootNamespace>RootFromProps</RootNamespace></PropertyGroup></Project>");

        var info = new ProjectFileInfo(fileSystem, "/app/My.Web.csproj");
        Assert.Equal("AsmFromProps", info.GetAssemblyName());
        Assert.Equal("FromCsproj", info.GetRootNamespace());
    }
}
