using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Serenity.Web
{
    public class DefaultUploadStorage : IUploadStorage
    {
        private readonly CombinedUploadStorage combined;

        public DefaultUploadStorage(IOptions<UploadSettings> options)
        {
            var opt = (options ?? throw new ArgumentNullException(nameof(options))).Value;
            if (string.IsNullOrEmpty(opt.Path))
                throw new ArgumentOutOfRangeException(nameof(opt.Path), 
                    "Please set UploadSettings.Path in appsettings.json!");

            if (string.IsNullOrEmpty(opt.Url))
                throw new ArgumentOutOfRangeException(nameof(opt.Path),
                    "Please set UploadSettings.Url in appsettings.json!");

            combined = new CombinedUploadStorage(
                new DiskUploadStorage(new DiskUploadStorageOptions
                {
                    RootPath = opt.Path,
                    RootUrl = opt.Url
                }),
                new TempUploadStorage(new DiskUploadStorageOptions
                {
                    RootPath = Path.Combine(opt.Path),
                    RootUrl = opt.Url + "temporary/"
                }),
                "temporary/");
        }

        public string ArchiveFile(string path)
        {
            return combined.ArchiveFile(path);
        }

        public string CopyFrom(IUploadStorage sourceStorage, string sourcePath, string targetPath, bool autoRename)
        {
            return combined.CopyFrom(sourceStorage, sourcePath, targetPath, autoRename);
        }

        public void DeleteFile(string path)
        {
            combined.DeleteFile(path);
        }

        public bool FileExists(string path)
        {
            return combined.FileExists(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return combined.GetFiles(path, searchPattern);
        }

        public long GetFileSize(string path)
        {
            return combined.GetFileSize(path);
        }

        public string GetFileUrl(string path)
        {
            return combined.GetFileUrl(path);
        }

        public string GetOriginalName(string path)
        {
            return combined.GetOriginalName(path);
        }

        public Stream OpenFile(string path)
        {
            return combined.OpenFile(path);
        }

        public void PurgeTemporaryFiles()
        {
            combined.PurgeTemporaryFiles();
        }

        public string WriteFile(string path, Stream source, bool autoRename)
        {
            return combined.WriteFile(path, source, autoRename);
        }
    }
}