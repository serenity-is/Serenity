using System;
using System.Collections.Generic;
using System.IO;

namespace Serenity.Web
{
    public static class UploadStorageExtensions
    {
        public static string GetThumbnailUrl(this IUploadStorage uploadStorage, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var thumb = UploadPathHelper.GetThumbnailName(path);

            return uploadStorage.GetFileUrl(thumb);
        }
      
        public static CopyTemporaryFileResult CopyTemporaryFile(this IUploadStorage uploadStorage, 
            CopyTemporaryFileOptions options)
        {
            if (uploadStorage is null)
                throw new ArgumentNullException(nameof(uploadStorage));

            long size = uploadStorage.GetFileSize(options.TemporaryFile);
            string path = PathHelper.ToUrl(UploadFormatting.FormatFilename(options));
            path = uploadStorage.CopyFrom(uploadStorage, options.TemporaryFile, path, autoRename: true);
            bool hasThumbnail = uploadStorage.FileExists(UploadPathHelper.GetThumbnailName(options.TemporaryFile));

            var result = new CopyTemporaryFileResult()
            {
                FileSize = size,
                Path = path,
                OriginalName = options.OriginalName,
                HasThumbnail = hasThumbnail
            };

            options.FilesToDelete?.RegisterNewFile(path);
            options.FilesToDelete?.RegisterOldFile(options.TemporaryFile);
            return result;
        }

        public static byte[] ReadAllFileBytes(this IUploadStorage uploadStorage, string path)
        {
            if (uploadStorage is null)
                throw new ArgumentNullException(nameof(uploadStorage));

            using var ms = new MemoryStream();
            using (var fs = uploadStorage.OpenFile(path))
                fs.CopyTo(ms);

            return ms.ToArray();
        }

        public static string GetOriginalName(this IUploadStorage uploadStorage, string path)
        {
            if (uploadStorage is null)
                throw new ArgumentNullException(nameof(uploadStorage));

            var metadata = uploadStorage.GetFileMetadata(path);
            if (metadata != null &&
                metadata.TryGetValue(FileMetadataKeys.OriginalName, out string originalName))
                return originalName;

            return null;
        }

        public static void SetOriginalName(this IUploadStorage uploadStorage, string path, string originalName)
        {
            if (uploadStorage is null)
                throw new ArgumentNullException(nameof(uploadStorage));

            var metadata = new Dictionary<string, string>()
            {
                [FileMetadataKeys.OriginalName] = originalName
            };

            uploadStorage.SetFileMetadata(path, metadata, overwriteAll: false);
        }
    }
}
