using System.IO;

namespace Serenity.Web
{
    [Obsolete("Please inject and use IUploadProcessor interface")]
    public class UploadProcessor : ProcessedUploadInfo
    {
        private readonly IUploadStorage storage;
        private readonly IExceptionLogger logger;

        public UploadProcessor(IUploadStorage storage, IExceptionLogger logger = null)
        {
            ThumbBackColor = null;
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
            this.logger = logger;
        }

        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public string ThumbBackColor { get; set; }
        public ImageScaleMode ThumbScaleMode { get; set; }
        public int ThumbQuality { get; set; }

        public bool ProcessStream(Stream fileContent, string extension, 
            ITextLocalizer localizer, IUploadOptions options = null)
        {
            if (fileContent is null)
                throw new ArgumentNullException(nameof(fileContent));

            options ??= new UploadOptions
            {
                ThumbBackColor = ThumbBackColor,
                ThumbHeight = ThumbHeight,
                ThumbWidth = ThumbWidth,
                ThumbQuality = ThumbQuality,
                ThumbMode = ThumbScaleMode
            };

            var imageProcessor = new DefaultImageProcessor();
            var uploadProcessor = new DefaultUploadProcessor(new DefaultImageProcessor(),
                storage, new DefaultUploadValidator(imageProcessor, localizer, logger), localizer, logger);

            var result = uploadProcessor.Process(fileContent, "___tempfile__" + extension, options);
            ErrorMessage = result.ErrorMessage;
            FileSize = result.FileSize;
            ImageHeight = result.ImageWidth;
            ImageHeight = result.ImageHeight;
            IsImage = result.IsImage;
            Success = result.Success;
            TemporaryFile = result.TemporaryFile;
            return result.Success;
        }
    }
}
