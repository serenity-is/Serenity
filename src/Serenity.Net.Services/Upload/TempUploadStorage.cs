using Serenity.IO;

namespace Serenity.Web
{
    public class TempUploadStorage : DiskUploadStorage
    {
        public TempUploadStorage(DiskUploadStorageOptions options, IDiskUploadFileSystem fileSystem = null)
            : base(options, fileSystem)
        {
            if (!this.fileSystem.DirectoryExists(RootPath))
            {
                try
                {
                    this.fileSystem.CreateDirectory(RootPath);
                    this.fileSystem.WriteAllText(fileSystem.Combine(RootPath, ".temporary"), "");
                }
                catch
                {
                    // swallow exception as this causes startup errors
                    // and application pool crashes if upload folder
                    // can't be accessed, better to ignore
                }
            }
        }

        public override void PurgeTemporaryFiles()
        {
            fileSystem.PurgeDirectory(RootPath);
        }
    }
}