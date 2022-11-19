using Serenity.ComponentModel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace Serenity.Web
{
    public class UploadProcessor
    {
        private readonly IUploadStorage storage;

        public UploadProcessor(IUploadStorage storage, IExceptionLogger logger = null)
        {
            ThumbBackColor = null;
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Logger = logger;
        }

        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public Color? ThumbBackColor { get; set; }
        public ImageScaleMode ThumbScaleMode { get; set; }
        public int ThumbQuality { get; set; }

        public string ThumbFile { get; private set; }
        public string ThumbUrl { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }
        public ImageCheckResult CheckResult { get; private set; }
        public string ErrorMessage { get; private set; }
        public long FileSize { get; private set; }
        public string TemporaryFile { get; private set; }
        public bool IsImage { get; private set; }

        protected IExceptionLogger Logger { get; }

        private bool IsImageExtension(string extension)
        {
            return extension.EndsWith(".jpg") ||
                extension.EndsWith(".jpeg") ||
                extension.EndsWith(".png") ||
                extension.EndsWith(".gif");
        }

        public bool ProcessStream(Stream fileContent, string extension, 
            ITextLocalizer localizer, IUploadEditor attr = null)
        {
            if (fileContent is null)
                throw new ArgumentNullException(nameof(fileContent));

            storage.PurgeTemporaryFiles();

            var success = false;
            IsImage = false;
            FileSize = fileContent.Length;
            ErrorMessage = null;
            ImageWidth = 0;
            ImageHeight = 0;

            try
            {
                try
                {
                    attr ??= new ImageUploadEditorAttribute
                    {
                        ThumbBackColor = ThumbBackColor?.ToHex(),
                        ThumbHeight = ThumbHeight,
                        ThumbWidth = ThumbWidth,
                        ThumbQuality = ThumbQuality,
                        ThumbMode = ThumbScaleMode
                    };

                    UploadValidator.CheckFileConstraints(attr as IUploadFileConstraints,
                        FileSize, extension, localizer, out bool isImageExtension);

                    SixLabors.ImageSharp.Image image = null;
                    if (isImageExtension)
                        UploadValidator.CheckImageConstraints(attr as IUploadImageContrains,
                            fileContent, extension, localizer, Logger, out image);
                    try
                    {
                        storage.PurgeTemporaryFiles();

                        var basePath = "temporary/" + Guid.NewGuid().ToString("N");
                        fileContent.Seek(0, SeekOrigin.Begin);
                        TemporaryFile = storage.WriteFile(basePath + extension, fileContent, autoRename: false);
                        IsImage = isImageExtension && image != null;

                        if (IsImage)
                        {
                            string temporaryFile = TemporaryFile;
                            ScaleImageAndCreateThumbs(attr as IUploadImageOptions,
                                storage, image, ref temporaryFile);
                            TemporaryFile = temporaryFile;
                        }

                        success = true;
                    }
                    finally
                    {
                        image?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    ex.Log(Logger);
                    success = false;
                    return success;
                }
            }
            finally
            {
                if (!success)
                {
                    if (!ThumbFile.IsNullOrEmpty())
                        storage.DeleteFile(ThumbFile);

                    if (!TemporaryFile.IsNullOrEmpty())
                        storage.DeleteFile(TemporaryFile);
                }

                fileContent.Dispose();
            }

            return success;
        }

        public static void ScaleImageAndCreateThumbs(IUploadImageOptions uploadImageOptions,
            IUploadStorage storage, SixLabors.ImageSharp.Image image, ref string temporaryFile)
        {
            if (storage is null) 
                throw new ArgumentNullException(nameof(storage));

            var imageWidth = image.Width;
            var imageHeight = image.Height;
            var scaleWidth = uploadImageOptions?.ScaleWidth ?? 0;
            var scaleHeight = uploadImageOptions?.ScaleHeight ?? 0;
            var scaleSmaller = uploadImageOptions?.ScaleSmaller == true;

            var baseFile = Path.ChangeExtension(temporaryFile, null);
            if ((scaleWidth > 0 || scaleHeight > 0) &&
                ((scaleWidth > 0 && (scaleSmaller || imageWidth > scaleWidth)) ||
                (scaleHeight > 0 && (scaleSmaller || imageHeight > scaleHeight))))
            {
                var originalName = storage.GetOriginalName(temporaryFile);
                var scaleBackColor = !string.IsNullOrEmpty(uploadImageOptions?.ScaleBackColor) ?
                    Color.Parse(uploadImageOptions.ScaleBackColor) : (Color?)null;
                var scaleMode = uploadImageOptions?.ScaleMode ?? ImageScaleMode.PreserveRatioNoFill;
                var scaleQuality = uploadImageOptions?.ScaleQuality ?? 0;
                using var scaledImage = ThumbnailGenerator.Generate(
                    image, scaleWidth, scaleHeight, scaleMode, 
                    backgroundColor: scaleBackColor);
                temporaryFile = baseFile + ".jpg";
                using var ms = new MemoryStream();
                scaledImage.Save(ms, new JpegEncoder { Quality = scaleQuality == 0 ? null : scaleQuality });
                ms.Seek(0, SeekOrigin.Begin);
                temporaryFile = storage.WriteFile(temporaryFile, ms, autoRename: null); // overwrite
                if (!string.IsNullOrEmpty(originalName))
                    storage.SetOriginalName(temporaryFile, Path.ChangeExtension(originalName, ".jpg"));
            }

            var thumbSizes = uploadImageOptions?.ThumbSizes.TrimToNull();
            var thumbWidth = uploadImageOptions?.ThumbWidth ?? 0;
            var thumbHeight = uploadImageOptions?.ThumbHeight ?? 0;
            if (thumbSizes == null &&
                thumbWidth == 0 &&
                thumbHeight == 0)
                return;

            var thumbBackColor = !string.IsNullOrEmpty(uploadImageOptions?.ThumbBackColor) ?
                Color.Parse(uploadImageOptions.ThumbBackColor) : (Color?)null;

            var thumbQuality = (uploadImageOptions?.ThumbQuality ?? 0) == 0 ? (int?)null :
                uploadImageOptions.ThumbQuality;

            var thumbEncoder = new JpegEncoder { Quality = thumbQuality };

            if (thumbWidth != 0 ||
                thumbHeight != 0)
            {
                var thumbMode = uploadImageOptions?.ThumbMode ?? ImageScaleMode.StretchToFit;
                using var thumbImage = ThumbnailGenerator.Generate(image, thumbWidth, thumbHeight, 
                    thumbMode, backgroundColor: thumbBackColor);
                var thumbFile = baseFile + "_t.jpg";
                using var ms = new MemoryStream();
                thumbImage.Save(ms, thumbEncoder);
                ms.Seek(0, SeekOrigin.Begin);
                storage.WriteFile(thumbFile, ms, autoRename: null);
            }

            if (thumbSizes != null)
            {
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
                        throw new ArgumentOutOfRangeException(nameof(uploadImageOptions.ThumbSizes));
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

                    using var thumbImage = ThumbnailGenerator.Generate(image, w, h, 
                        uploadImageOptions?.ThumbMode ?? ImageScaleMode.PreserveRatioNoFill, backgroundColor: thumbBackColor);
                    var thumbFile = baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg";
                    using var ms = new MemoryStream();
                    thumbImage.Save(ms, thumbEncoder);
                    ms.Seek(0, SeekOrigin.Begin);
                    storage.WriteFile(thumbFile, ms, autoRename: null);
                }
            }

        }
    }
}
