using Serenity.IO;
using System.IO;

namespace Serenity.Web
{
    public class TempUploadStorage : DiskUploadStorage
    {
        public TempUploadStorage(DiskUploadStorageOptions options)
            : base(options)
        {
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
                File.WriteAllText(Path.Combine(RootPath, ".temporary"), "");
            }
        }

        public override void PurgeTemporaryFiles()
        {
            TemporaryFileHelper.PurgeDirectoryDefault(RootPath);
        }
    }
}