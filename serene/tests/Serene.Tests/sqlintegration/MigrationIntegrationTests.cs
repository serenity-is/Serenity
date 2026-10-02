using Serenity.IO;

namespace Serene.Migrations;

[Trait("tag", "sqldb")]
public sealed partial class MigrationIntegrationTests : IDisposable
{
    private readonly string tempPath;
    private readonly string defaultDatabase;
    private readonly string northwindDatabase;
    private Action testCleanup;

    public MigrationIntegrationTests()
    {
        tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "SereneMigrations", TemporaryFileHelper.RandomFileCode());
        defaultDatabase = "SereneDefaultIT_" + Guid.NewGuid().ToString("N");
        northwindDatabase = "SereneNorthwindIT_" + Guid.NewGuid().ToString("N");
        System.IO.Directory.CreateDirectory(tempPath);
    }

    public void Dispose()
    {
        try
        {
            testCleanup?.Invoke();
            testCleanup = null;
        }
        finally
        {
            if (System.IO.Directory.Exists(tempPath))
                System.IO.Directory.Delete(tempPath, true);
        }
    }

}
