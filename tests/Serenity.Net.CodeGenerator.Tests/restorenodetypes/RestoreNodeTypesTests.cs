namespace Serenity.CodeGenerator;

public partial class RestoreNodeTypesTests
{
    [Fact]
    public void UsesPackageIdFromPackageJson()
    {
        var fileSystem = new MockFileSystem();
        var task = new RestoreNodeTypesTask(fileSystem)
        {
            FolderNames = "test.lib"
        };
        fileSystem.AddFile("package.json", """"
            {
              "name": "host",
              "dependencies": {
              }
            }
            """");
        fileSystem.AddFile("node_modules/.dotnet/test.lib/package.dotnet.json", """"
            {
              "name": "my-test-lib"
            }
            """");

        task.Execute();

        Assert.Equal(""""
            {
              "name": "my-test-lib"
            }
            """", fileSystem.ReadAllText("node_modules/.dotnet/test.lib/package.json"));
    }
}
