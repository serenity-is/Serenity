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

        public static string ScaleImageAs(object image, IImageProcessor imageProcessor,
            int width, int height, ImageScaleMode mode, string backgroundColor, 
            string mimeType, ImageEncoderParams encoderParams,
            IUploadStorage uploadStorage, string path, bool? autoRename)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            if (imageProcessor is null)
                throw new ArgumentNullException(nameof(imageProcessor));

            if (uploadStorage is null)
                throw new ArgumentNullException(nameof(uploadStorage));

            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (width == 0 && height == 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            var scaledImage = imageProcessor.Scale(image, width, height, mode, backgroundColor, inplace: false);
            try
            {
                using var ms = new MemoryStream();
                imageProcessor.Save(scaledImage, ms, mimeType, encoderParams);
                ms.Seek(0, SeekOrigin.Begin);
                return uploadStorage.WriteFile(path, ms, autoRename: autoRename);
            }
            finally
            {
                (scaledImage as IDisposable)?.Dispose();
            }
        }

        public static string ScaleImage(object image,
            IImageProcessor imageProcessor, IUploadImageOptions options,
            IUploadStorage uploadStorage, string temporaryFile, bool? autoRename = null)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(temporaryFile))
                throw new ArgumentNullException(nameof(temporaryFile));

            var (imageWidth, imageHeight) = imageProcessor.GetImageSize(image);
            var scaleSmaller = options.ScaleSmaller == true;

            var baseFile = Path.ChangeExtension(temporaryFile, null);
            if ((options.ScaleWidth > 0 || options.ScaleHeight > 0) &&
                (options.ScaleWidth != imageWidth || options.ScaleHeight != imageHeight) &&
                ((options.ScaleWidth > 0 && (scaleSmaller || options.ScaleWidth < imageWidth)) ||
                 (options.ScaleHeight > 0 && (scaleSmaller || options.ScaleHeight < imageHeight))))
            {
                var originalName = uploadStorage.GetOriginalName(temporaryFile);
                temporaryFile = ScaleImageAs(image, imageProcessor,
                    options.ScaleWidth, options.ScaleHeight, options.ScaleMode, options.ScaleBackColor,
                    mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ScaleQuality },
                    uploadStorage, baseFile + ".jpg", autoRename);
                if (!string.IsNullOrEmpty(originalName))
                    uploadStorage.SetOriginalName(temporaryFile, Path.ChangeExtension(originalName, ".jpg"));
            }
            return temporaryFile;
        }

        public static string CreateDefaultThumb(object image,
            IImageProcessor imageProcessor, IUploadImageOptions options,
            IUploadStorage uploadStorage, string temporaryFile, bool? autoRename = null)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(temporaryFile))
                throw new ArgumentNullException(nameof(temporaryFile));

            if ((options.ThumbWidth > 0 || options.ThumbHeight > 0) &&
                (options.ThumbWidth >= 0 && options.ThumbHeight >= 0))
            {
                return ScaleImageAs(image, imageProcessor,
                    options.ThumbWidth, options.ThumbHeight, options.ThumbMode, options.ThumbBackColor,
                    mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ThumbQuality },
                    uploadStorage, Path.ChangeExtension(temporaryFile, null) + "_t.jpg", autoRename);
            }

            return null;
        }

        public static void CreateAdditionalThumbs(object image,
            IImageProcessor imageProcessor, IUploadImageOptions options,
            IUploadStorage uploadStorage, string temporaryFile, bool? autoRename = null)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(temporaryFile))
                throw new ArgumentNullException(nameof(temporaryFile));

            var thumbSizes = options.ThumbSizes.TrimToNull();
            if (thumbSizes is null)
                return;

            var baseFile = Path.ChangeExtension(temporaryFile, null);

            foreach (var sizeStr in thumbSizes.Replace(";", ",", StringComparison.Ordinal).Split(new[] { ',' }))
            {
                var dims = sizeStr.ToUpperInvariant().Split(new[] { 'X' });
                if (dims.Length != 2 ||
                    !int.TryParse(dims[0], out int w) ||
                    !int.TryParse(dims[1], out int h) ||
                    w < 0 ||
                    h < 0 ||
                    (w == 0 && h == 0))
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                    throw new ArgumentOutOfRangeException(nameof(thumbSizes));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

                var thumbFile = baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg";

                ScaleImageAs(image, imageProcessor,
                    w, h, options.ThumbMode, options.ThumbBackColor,
                    mimeType: "image/jpeg", new ImageEncoderParams { Quality = options.ThumbQuality },
                    uploadStorage, thumbFile, autoRename);
            }
        }

        public static string ScaleImageAndCreateAllThumbs(object image,
            IImageProcessor imageProcessor, IUploadImageOptions options,
            IUploadStorage uploadStorage, string temporaryFile, bool? autoRename = null)
        {
            var orgTempFile = temporaryFile;
            temporaryFile = ScaleImage(image, imageProcessor, options, uploadStorage, temporaryFile, autoRename);
            CreateDefaultThumb(image, imageProcessor, options, uploadStorage, orgTempFile, autoRename);
            CreateAdditionalThumbs(image, imageProcessor, options, uploadStorage, orgTempFile, autoRename);
            return temporaryFile;
        }
    }
}
