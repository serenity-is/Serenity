using System.IO;
using Serenity.IO;

namespace Serenity.Web;

public class PhysicalDiskUploadFileSystemTests
{
    [Fact]
    public void Methods_Delegate_To_TemporaryFileHelper()
    {
        var fs = new PhysicalDiskUploadFileSystem();
        var dir = Path.Combine(Path.GetTempPath(), "serenity-phys-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var file = Path.Combine(dir, "a.txt");
            File.WriteAllText(file, "x");

            fs.TryDeleteMarkedFiles(dir);

            var check = Path.Combine(dir, TemporaryFileHelper.DefaultTemporaryCheckFile);
            File.WriteAllText(check, "");
            fs.PurgeDirectory(dir, TimeSpan.Zero, -1, null);
            fs.PurgeDirectory(dir);

            fs.TryDeleteOrMark(file);
            fs.Delete(file, DeleteType.TryDelete);
            fs.Delete(file, DeleteType.Delete);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [Fact]
    public void TryDeleteMarkedFiles_Ignores_Missing_Directory()
    {
        var fs = new PhysicalDiskUploadFileSystem();
        fs.TryDeleteMarkedFiles(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
    }
}
