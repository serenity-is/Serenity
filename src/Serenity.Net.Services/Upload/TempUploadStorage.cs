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
                try
                {
                    Directory.CreateDirectory(RootPath);
                    File.WriteAllText(Path.Combine(RootPath, ".temporary"), "");
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
            TemporaryFileHelper.PurgeDirectoryDefault(RootPath);
        }
    }
}