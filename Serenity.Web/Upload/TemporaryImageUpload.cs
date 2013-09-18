using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace Serenity.Web
{
    /// <summary>
    ///   TemporaryFile's specialized version for image uploads.</summary>
    [Serializable]
    public class TemporaryImageUpload : TemporaryFileUpload
    {
        private int minImageHeight;
        private int minImageWidth;
        private int maxImageHeight;
        private int maxImageWidth;
        private int setImageWidth;
        private int setImageHeight;
        private bool setImageSizeIfBigger;
        private ImageScaleMode imageScaleMode;
        private int thumbWidth;
        private int thumbHeight;
        private string thumbSuffix;
        private Color thumbBackColor = Color.Empty;
        private ImageScaleMode thumbScaleMode;
        private ImageDisplayMode displayMode;
        private bool allowFlashMovies;

        internal ImageCheckResult checkResult;
        internal string thumbFile;
        internal int imageWidth;
        internal int imageHeight;
        internal int generatedThumbWidth;
        internal int generatedThumbHeight;

        /// <summary>
        ///   Overriden to also check for thumb file existence.</summary>
        public override bool HasFile
        {
            get
            {
                if (base.HasFile)
                {
                    if ((!FileExists(UploadedFile)) ||
                        (checkResult != ImageCheckResult.FlashMovie && 
                            ThumbRequested && (!FileExists(ThumbFile))))
                    {
                        base.HasFile = false;
                    }
                }
                return base.HasFile;
            }
        }

        /// <summary>
        ///   Gets temporary thumb file name.</summary>
        public string ThumbFile
        {
            get { return thumbFile; }
        }

        /// <summary>
        ///   Gets the URL for temporary image, or its thumbnail if requested.</summary>
        public override string TemporaryFileUrl
        {
            get
            {
                if (HasFile)
                {
                    string temp;

                    if (checkResult != ImageCheckResult.FlashMovie &&
                        ThumbRequested &&
                        DisplayMode == ImageDisplayMode.Thumbnail &&
                        ThumbFile != null &&
                        ThumbFile.Length > 0)
                        temp = ThumbFile;
                    else
                        temp = UploadedFile;

                    temp = UrlHelper.Combine(TemporaryUrl, Path.GetFileName(temp));

                    return temp.Length == 0 ? "" : UrlHelper.ResolveUrl(temp);
                }
                else
                    return "";
            }
        }

        /// <summary>
        ///   Overriden to also clean the thumb file if any.</summary>
        protected internal override void ClearFiles()
        {
            try
            {
                if (FileExists(thumbFile))
                    File.Delete(thumbFile);
            }
            catch
            {
            }
            thumbFile = "";
            base.ClearFiles();
        }

        /// <summary>
        ///   Overriden to clear state specific to TemporaryImage.</summary>
        protected internal override void ClearState()
        {
            base.ClearState();
            checkResult = ImageCheckResult.InvalidImage;
            imageHeight = 0;
            imageWidth = 0;
            generatedThumbHeight = 0;
            generatedThumbWidth = 0;
        }

        /// <summary>
        ///   Gets uploaded image width</summary>
        public int ImageWidth
        {
            get { return imageWidth; }
        }

        /// <summary>
        ///   Gets uploaded image height</summary>
        public int ImageHeight
        {
            get { return imageHeight; }
        }

        /// <summary>
        ///   Is thumbnail requested? True if ThumbSuffix is not empty and
        ///   one ThumbWidth or ThumbHeight parameters are greater than 0.</summary>
        public bool ThumbRequested
        {
            get
            {
                return
                    (thumbHeight > 0 ||
                     thumbWidth > 0) &&
                    thumbSuffix != null &&
                    thumbSuffix.Length > 0;
            }
        }

        /// <summary>
        ///   Minimum image height (0 = unlimited)</summary>
        public int MinImageHeight
        {
            get { return minImageHeight; }
            set { minImageHeight = value; }
        }

        /// <summary>
        ///   Minimum image width (0 = unlimited)</summary>
        public int MinImageWidth
        {
            get { return minImageWidth; }
            set { minImageWidth = value; }
        }

        /// <summary>
        ///   Maximum image height (0 = unlimited)</summary>
        public int MaxImageHeight
        {
            get { return maxImageHeight; }
            set { maxImageHeight = value; }
        }

        /// <summary>
        ///   Maximum image width (0 = unlimited)</summary>
        public int MaxImageWidth
        {
            get { return maxImageWidth; }
            set { maxImageWidth = value; }
        }

        /// <summary>
        ///   The width to scale image to (0 = no width scaling)</summary>
        public int SetImageWidth
        {
            get { return setImageWidth; }
            set { setImageWidth = value; }
        }

        /// <summary>
        ///   The height to scale image to (0 = no height scaling)</summary>
        public int SetImageHeight
        {
            get { return setImageHeight; }
            set { setImageHeight = value; }
        }

        /// <summary>
        ///   True, if image scaling should only occur when image is larger than specified 
        ///   SetImageWidth and SetImageHeight parameters. For smaller values, no scaling
        ///   occurs.</summary>
        public bool SetImageSizeIfBigger
        {
            get { return setImageSizeIfBigger; }
            set { setImageSizeIfBigger = value; }
        }

        /// <summary>
        ///   Image scaling mode</summary>
        public ImageScaleMode ImageScaleMode
        {
            get { return imageScaleMode; }
            set { imageScaleMode = value; }
        }

        /// <summary>
        ///   Thumbnail width</summary>
        public int ThumbHeight
        {
            get { return thumbHeight; }
            set { thumbHeight = value; }
        }

        /// <summary>
        ///   Thumbnail width requested</summary>
        public int ThumbWidth
        {
            get { return thumbWidth; }
            set { thumbWidth = value; }
        }

        /// <summary>
        ///   Thumbnail suffix ("_t")</summary>
        public string ThumbSuffix
        {
            get { return thumbSuffix; }
            set { thumbSuffix = value; }
        }

        /// <summary>
        ///   Background color for thumbnail fills</summary>
        public Color ThumbBackColor
        {
            get { return thumbBackColor; }
            set { thumbBackColor = value; }
        }

        /// <summary>
        ///   Thumbnail scale mode</summary>
        public ImageScaleMode ThumbScaleMode
        {
            get { return thumbScaleMode; }
            set { thumbScaleMode = value; }
        }

        /// <summary>
        ///   Image display mode.</summary>
        public ImageDisplayMode DisplayMode
        {
            get { return displayMode; }
            set { displayMode = value; }
        }

        /// <summary>
        ///   Allow flash (.swf) movie uploads</summary>
        public bool AllowFlashMovies
        {
            get { return allowFlashMovies; }
            set { allowFlashMovies = value; }
        }

        /// <summary>
        ///   Checks if given file name ends with ".swf"</summary>
        /// <param name="filename">
        ///   Filename.</param>
        /// <returns>
        ///   True if SWF files allowed (AllowFlashMovies) and file extension is .SWF.</returns>
        private bool IsSWF(string filename)
        {
            return AllowFlashMovies && filename.EndsWith(".swf", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///   Tries to upload a file and generates its thumbnail.</summary>
        /// <param name="uploadSlot">
        ///   File upload slot.</param>
        /// <param name="postedFile">
        ///   Stream with posted file.</param>
        /// <param name="originalFile">
        ///   Original file name.</param>
        /// <remarks>
        ///   No exceptions raised if upload fails. Should check IsUploadFailed and ErrorMessage
        ///   for errors.</remarks>
        public override void TryUploadingFile(Stream postedFile, string originalFile)
        {
            ClearAll();

            bool isSWF = IsSWF(originalFile);
            System.Drawing.Image image = null;
            ImageCheckResult imageCheckResult = ImageCheckResult.InvalidImage;
            ImageChecker imageChecker = null;
            string imageExtension = ".jpg";
            string thumbExtension = ThumbSuffix + ".jpg";
            bool thumbRequested = ThumbRequested;
            int swfWidth = 0;
            int swfHeight = 0;
            int swfFileSize = 0;

            Stream fileContent = postedFile;
            try
            {
                imageChecker = new ImageChecker();
                imageChecker.MinWidth = MinImageWidth;
                imageChecker.MaxWidth = MaxImageWidth;
                imageChecker.MinHeight = MinImageHeight;
                imageChecker.MaxHeight = MaxImageHeight;
                imageChecker.MaxDataSize = MaxFileSize;

                if (!isSWF)
                {
                    imageCheckResult = imageChecker.CheckStream(
                        fileContent, true, out image);
                }
                else
                {
                    swfFileSize = (int)(fileContent.Length);
                    if (MaxFileSize > 0 && swfFileSize > MaxFileSize)
                        imageCheckResult = ImageCheckResult.DataSizeTooHigh;
                    else
                    {
                        MemoryStream ms = new MemoryStream();
                        TemporaryFileUpload.CopyStream(fileContent, ms);
                        try
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            
                            SwfInfo swfInfo = new SwfInfo(ms);
                            swfWidth = swfInfo.Width;
                            swfHeight = swfInfo.Height;

                            imageCheckResult = imageChecker.CheckSizeConstraints(swfWidth, swfHeight);
                            if (imageCheckResult == ImageCheckResult.JPEGImage)
                                imageCheckResult = ImageCheckResult.FlashMovie;
                        }
                        catch
                        {
                            imageCheckResult = ImageCheckResult.UnsupportedFormat;
                        }
                    }
                }
                try
                {
                    try
                    {
                        OriginalFile = originalFile;
                        if (!isSWF)
                        {
                            checkResult = imageCheckResult;
                            FileSize = imageChecker.DataSize;
                            imageWidth = imageChecker.Width;
                            imageHeight = imageChecker.Height;
                        }
                        else
                        {
                            imageWidth = swfWidth;
                            imageHeight = swfHeight;
                            FileSize = swfFileSize;
                        }

                        if (imageCheckResult != ImageCheckResult.JPEGImage &&
                            imageCheckResult != ImageCheckResult.GIFImage &&
                            imageCheckResult != ImageCheckResult.PNGImage &&
                            imageCheckResult != ImageCheckResult.FlashMovie)
                        {
                            IsUploadFailed = true;
                            ErrorMessage = imageChecker.FormatErrorMessage(imageCheckResult);
                        }
                        else
                        {
                            if (!isSWF)
                            {
                                if (imageCheckResult == ImageCheckResult.GIFImage)
                                    imageExtension = ".gif";
                                else if (imageCheckResult == ImageCheckResult.PNGImage)
                                    imageExtension = ".png";
                            }

                            TemporaryFileHelper.PurgeDirectoryDefault(TemporaryPath);

                            string baseFileName = System.IO.Path.Combine(TemporaryPath,
                                Guid.NewGuid().ToString("N"));

                            if (!isSWF)
                            {
                                if ((setImageWidth > 0 || 
                                     setImageHeight > 0) &&
                                    ((setImageWidth > 0 &&
                                      ((setImageSizeIfBigger && imageWidth > setImageWidth) ||
                                       (!setImageSizeIfBigger && imageWidth != setImageWidth))) ||
                                     (setImageHeight > 0 &&
                                      (setImageSizeIfBigger && imageHeight > setImageHeight) ||
                                       !setImageSizeIfBigger && imageHeight != setImageHeight)))
                                {
                                    UploadedFile = baseFileName + ".jpg";
                                    using (
                                        System.Drawing.Image scaledImage =
                                            ThumbnailGenerator.Generate(
                                                image,
                                                setImageWidth,
                                                setImageHeight,
                                                imageScaleMode,
                                                thumbBackColor))
                                    {
                                        scaledImage.Save(UploadedFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                                        imageWidth = scaledImage.Width;
                                        imageHeight = scaledImage.Height;
                                    }

                                }
                                else
                                {
                                    UploadedFile = baseFileName + imageExtension;
                                    fileContent.Seek(0, SeekOrigin.Begin);
                                    using (FileStream fs = new FileStream(UploadedFile, FileMode.Create))
                                        CopyStream(fileContent, fs);
                                }
                            }
                            else
                            {
                                UploadedFile = baseFileName + ".swf";
                                fileContent.Seek(0, SeekOrigin.Begin);
                                using (FileStream fs = new FileStream(UploadedFile, FileMode.Create))
                                    CopyStream(fileContent, fs);
                                checkResult = ImageCheckResult.FlashMovie;
                            }

                            if (!isSWF && thumbRequested)
                            {
                                using (
                                    System.Drawing.Image thumbImage =
                                        ThumbnailGenerator.Generate(
                                            image,
                                            thumbWidth,
                                            thumbHeight,
                                            thumbScaleMode,
                                            thumbBackColor))
                                {
                                    /* ENABLE THIS CODE TO SET IMAGE QUALITY FOR THUMBNAILS
                                    var p = new System.Drawing.Imaging.EncoderParameters(1);
                                    p.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

                                    ImageCodecInfo jpegCodec = null;
                                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                                    // Find the correct image codec 
                                    for (int i = 0; i < codecs.Length; i++)
                                        if (codecs[i].MimeType == "image/jpeg")
                                            jpegCodec = codecs[i];
                                    thumbImage.Save(slot.thumbFile, jpegCodec, p);
                                    */
                                    thumbFile = baseFileName + thumbExtension;
                                    thumbImage.Save(thumbFile,
                                        System.Drawing.Imaging.ImageFormat.Jpeg);
                                    generatedThumbHeight = thumbImage.Width;
                                    generatedThumbWidth = thumbImage.Height;
                                }
                            }

                            HasFile = true;
                        }
                    }
                    catch (Exception exc)
                    {
                        ClearFiles();
                        ErrorMessage = exc.Message;
                        IsUploadFailed = true;
                    }
                }
                finally
                {
                    if (image != null)
                    {
                        image.Dispose();
                        image = null;
                    }
                }
            }
            finally
            {
                fileContent.Dispose();
            }
        }

        /// <summary>
        ///   Saves temporary file and its thumbnail to specified target file.</summary>
        /// <param name="uploadSlot">
        ///   Upload slot.</param>
        /// <param name="fileName">
        ///   Target file full path (required).</param>
        public override void SaveToFile(string fileName)
        {
            if (!HasFile)
                throw new InvalidOperationException("TemporaryImage has no image file to save");

            try
            {
                string targetFile = fileName;

                TemporaryFileHelper.Delete(targetFile, DeleteType.Delete);
                File.Copy(UploadedFile, targetFile);
                TemporaryFileHelper.TryDelete(UploadedFile);

                if (ThumbRequested)
                {
                    targetFile = Path.Combine(Path.GetDirectoryName(targetFile), 
                        Path.GetFileNameWithoutExtension(fileName) + ThumbSuffix + "_t.jpg");
                    if (File.Exists(targetFile))
                        File.Delete(targetFile);
                    if (!IsSWF(fileName))
                    {
                        TemporaryFileHelper.Delete(targetFile, DeleteType.Delete);
                        File.Copy(thumbFile, targetFile);
                        TemporaryFileHelper.TryDelete(thumbFile);
                    }
                }
            }
            finally
            {
                ClearAll();
            }
        }

        /// <summary>
        ///   Find thumbnail file name from image file name.</summary>
        /// <param name="fileName">
        ///   Image file name (if passed as null or empty result is also empty)</param>
        /// <param name="thumbSuffix">
        ///   Thumbnail extension (can be null, default "_t")</param>
        /// <returns>
        ///   Thumbnail file name.</returns>
        public static string GetThumbFileName(string fileName, string thumbSuffix)
        {
            if (fileName != null && fileName.Length > 0)
            {
                if (fileName.IndexOf('/') >= 0)
                    fileName = fileName.Replace('/', '\\');
                string path = Path.GetDirectoryName(fileName);
                if (thumbSuffix == null)
                    thumbSuffix = "_t";
                return Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + 
                    thumbSuffix + ".jpg");
            }
            else
                return fileName;
        }

        /// <summary>
        ///   Saves a temporary files content to given file.</summary>
        /// <param name="uploadSlot">
        ///   File upload slot.</param>
        /// <param name="fileName">
        ///   Target file (required).</param>
        public override void RelaseAndKeepTemporaryFile()
        {
            thumbFile = null;
            UploadedFile = null;
        }
    }
}