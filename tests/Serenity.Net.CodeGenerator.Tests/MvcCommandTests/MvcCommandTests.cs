using Serenity.CodeGenerator;

namespace Serenity.Tests.CodeGenerator;

public partial class MvcCommandTests
{

    MvcCommand CreateCommand(string[] viewPaths, out MockFileSystem fileSystem)
    {
        fileSystem = new MockFileSystem();
        var project = new ProjectFileInfo(fileSystem, @"C:\Repos\MyTest.Web\MyTest.Web.csproj");
        var directory = fileSystem.GetDirectoryName(project.ProjectFile);
        fileSystem.AddFile(project.ProjectFile, "<Project Sdk=\"Microsoft.NET.Sdk.Web\"></Project>");
        fileSystem.AddFile(fileSystem.Combine(directory, "sergen.json"),
            "{\"MVC\": {\"UseRootNamespace\": true}}");
        var command = new MvcCommand(project, new MockGeneratorConsole());
        foreach (var viewPath in viewPaths)
            fileSystem.AddFile(fileSystem.Combine(directory, viewPath), "");
        return command;
    }

    [Fact]
    public void SameSubFolderNameInDifferentParents()
    {
        var expected =
@"namespace MyTest.MVC
{
    public static class Views
    {
        public static class Data
        {
            public static class DataReports
            {
                public static class Viewer
                {
                    public const string DataReportDashboard = ""~/Modules/Data/DataReports/Viewer/DataReportDashboard.cshtml"";
                    public const string DataReportViewer = ""~/Modules/Data/DataReports/Viewer/DataReportViewer.cshtml"";
                    public const string WorkbookViewer = ""~/Modules/Data/DataReports/Viewer/WorkbookViewer.cshtml"";
                }
            }

            public static class DataVisuals
            {
                public static class Viewer
                {
                    public const string DataVisualDashboard = ""~/Modules/Data/DataVisuals/Viewer/DataVisualDashboard.cshtml"";
                    public const string DataVisualViewer = ""~/Modules/Data/DataVisuals/Viewer/DataVisualViewer.cshtml"";
                }
            }
        }
    }
}".ReplaceLineEndings();

        var command = CreateCommand([
            @"Modules\Data\DataReports\Viewer\DataReportDashboard.cshtml",
            @"Modules\Data\DataReports\Viewer\DataReportViewer.cshtml",
            @"Modules\Data\DataReports\Viewer\WorkbookViewer.cshtml",
            @"Modules\Data\DataVisuals\Viewer\DataVisualDashboard.cshtml",
            @"Modules\Data\DataVisuals\Viewer\DataVisualViewer.cshtml"], out var fileSystem);

        var exitCode = command.Run();
        Assert.Equal(ExitCodes.Success, exitCode);

        var generated = fileSystem.ReadAllText(@"C:\Repos\MyTest.Web\Imports\MVC\MVC.cs").Trim().ReplaceLineEndings();
        Assert.Equal(expected, generated);
    }
}
