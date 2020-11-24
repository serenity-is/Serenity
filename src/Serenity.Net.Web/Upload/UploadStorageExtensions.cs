namespace Serenity.Web
{
    public static class UploadStorageExtensions
    {
        public static string GetThumbnailUrl(this IUploadStorage storage, string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var thumb = UploadPathHelper.GetThumbnailName(path);

            return storage.GetFileUrl(thumb);
        }
      
        public static CopyTemporaryFileResult CopyTemporaryFile(this IUploadStorage storage, CopyTemporaryFileOptions options)
        {
            long size = storage.GetFileSize(options.TemporaryFile);
            string path = PathHelper.ToUrl(UploadFormatting.FormatFilename(options));
            path = storage.CopyFrom(storage, options.TemporaryFile, path, autoRename: true);
            bool hasThumbnail = storage.FileExists(UploadPathHelper.GetThumbnailName(options.TemporaryFile));

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
    }
}
