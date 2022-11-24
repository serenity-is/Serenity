
using System.IO;

namespace Serenity.Web
{
    /// <summary>
    /// Default implementation for <see cref="IUploadProcessor"/>
    /// </summary>
    public class DefaultUploadProcessor : IUploadProcessor
    {
        private readonly IImageProcessor imageProcessor;
        private readonly IUploadStorage uploadStorage;
        private readonly IUploadValidator uploadValidator;
        private readonly IExceptionLogger logger;

        /// <summary>
        /// Creates a new instance of the class
        /// </summary>
        /// <param name="imageProcessor">Image processor</param>
        /// <param name="uploadStorage">Upload storage</param>
        /// <param name="uploadValidator">Upload validator</param>
        /// <param name="logger">Exception logger</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DefaultUploadProcessor(IImageProcessor imageProcessor, IUploadStorage uploadStorage, IUploadValidator uploadValidator,
            IExceptionLogger logger = null)
        {
            this.imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
            this.uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));
            this.uploadValidator = uploadValidator ?? throw new ArgumentNullException(nameof(uploadValidator));
            this.logger = logger;
        }

        /// <inheritdoc/>
        public ProcessedUploadInfo Process(Stream stream, string filename, IUploadOptions options)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (filename is null)
                throw new ArgumentNullException(nameof(filename));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            var result = new ProcessedUploadInfo
            {
                FileSize = stream.Length
            };

            try
            {
                try
                {
                    uploadValidator.ValidateFile(options as IUploadFileConstraints ?? new UploadOptions(),
                        stream, filename, out bool isImageExtension);

                    object image = null;
                    if (isImageExtension)
                        uploadValidator.ValidateImage(options as IUploadImageContrains ?? new UploadOptions(),
                            stream, filename, out image);
                    try
                    {
                        uploadStorage.PurgeTemporaryFiles();

                        var basePath = "temporary/" + Guid.NewGuid().ToString("N");
                        stream.Seek(0, SeekOrigin.Begin);
                        result.TemporaryFile = uploadStorage.WriteFile(basePath + Path.GetExtension(filename), 
                            stream, OverwriteOption.Disallowed);
                        result.IsImage = isImageExtension && image != null;
                        if (result.IsImage)
                        {
                            var (width, height) = imageProcessor.GetImageSize(image);
                            result.ImageWidth = width;
                            result.ImageHeight = height;
                            stream.Close();
                            result.TemporaryFile = UploadStorageExtensions.ScaleImageAndCreateAllThumbs(image, imageProcessor,
                                options as IUploadImageOptions ?? new UploadOptions(),
                                uploadStorage, result.TemporaryFile, OverwriteOption.Overwrite);
                        }
                        result.Success = true;
                    }
                    finally
                    {
                        (image as IDisposable)?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorMessage = ex.Message;
                    result.Success = false;
                    ex.Log(logger);
                }
            }
            finally
            {
                if (!result.Success && !string.IsNullOrEmpty(result.TemporaryFile))
                    uploadStorage.DeleteFile(result.TemporaryFile);

                stream?.Dispose();
            }

            return result;
        }
    }
}
