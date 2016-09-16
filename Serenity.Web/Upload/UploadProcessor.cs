using System;
using System.IO;
using Serenity.IO;
#if !COREFX
using System.Drawing;
using System.Drawing.Imaging;
#endif

namespace Serenity.Web
{
    public class UploadProcessor
    {
#if !COREFX
        public UploadProcessor()
        {
            ThumbBackColor = Color.Empty;
        }

        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public Color ThumbBackColor { get; set; }
        public ImageScaleMode ThumbScaleMode { get; set; }
        public int ThumbQuality { get; set; }
        public string ThumbFile { get; private set; }
        public string ThumbUrl { get; private set; }
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }
#endif
        public ImageCheckResult CheckResult { get; private set; }
        public string ErrorMessage { get; private set; }
        public long FileSize { get; private set; }
        public string FilePath { get; private set; }
        public string FileUrl { get; private set; }
        public bool IsImage { get; private set; }


        private bool IsImageExtension(string extension)
        {
            return extension.EndsWith(".jpg") ||
                extension.EndsWith(".jpeg") ||
                extension.EndsWith(".png") ||
                extension.EndsWith(".gif");
        }

        private bool IsDangerousExtension(string extension)
        {
            return extension.EndsWith(".exe") ||
                extension.EndsWith(".bat") ||
                extension.EndsWith(".cmd") ||
                extension.EndsWith(".dll") ||
                extension.EndsWith(".jar") ||
                extension.EndsWith(".jsp") ||
                extension.EndsWith(".htaccess") ||
                extension.EndsWith(".htpasswd") ||
                extension.EndsWith(".lnk") ||
                extension.EndsWith(".vbs") ||
                extension.EndsWith(".vbe") ||
                extension.EndsWith(".aspx") ||
                extension.EndsWith(".ascx") ||
                extension.EndsWith(".config") ||
                extension.EndsWith(".com") ||
                extension.EndsWith(".asmx") ||
                extension.EndsWith(".asax") ||
                extension.EndsWith(".compiled") ||
                extension.EndsWith(".php");
        }

        public bool ProcessStream(Stream fileContent, string extension)
        {
            extension = extension.TrimToEmpty().ToLowerInvariant();
            if (IsDangerousExtension(extension))
            {
                ErrorMessage = "Unsupported file extension!";
                return false;
            }

            CheckResult = ImageCheckResult.InvalidImage;
            ErrorMessage = null;
#if !COREFX
            ImageWidth = 0;
            ImageHeight = 0;
#endif
            IsImage = false;

            var success = false;

            var temporaryPath = UploadHelper.TemporaryPath;
            Directory.CreateDirectory(temporaryPath);
            TemporaryFileHelper.PurgeDirectoryDefault(temporaryPath);
            string baseFileName = System.IO.Path.Combine(temporaryPath, Guid.NewGuid().ToString("N"));

            try
            {
                try
                {
                    if (IsImageExtension(extension))
                    {
#if COREFX
                        IsImage = true;
                        success = true;
                        FilePath = baseFileName + extension;
                        fileContent.Seek(0, SeekOrigin.Begin);
                        using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                            fileContent.CopyTo(fs);
#else
                        success = ProcessImageStream(fileContent, extension);
#endif
                    }
                    else
                    {
                        FilePath = baseFileName + extension;
                        fileContent.Seek(0, SeekOrigin.Begin);
                        using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                            fileContent.CopyTo(fs);
                        success = true;
                    }

                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                    success = false;
                    return success;
                }
            }
            finally
            {
                if (!success)
                {
#if !COREFX
                    if (!ThumbFile.IsNullOrEmpty())
                        TemporaryFileHelper.TryDelete(ThumbFile);
#endif
                    if (!FilePath.IsNullOrEmpty())
                        TemporaryFileHelper.TryDelete(FilePath);
                }

                fileContent.Dispose();
            }

            return success;
        }

#if !COREFX
        private bool ProcessImageStream(Stream fileContent, string extension)
        {
            Image image = null;
            var imageChecker = new ImageChecker();
            CheckResult = imageChecker.CheckStream(fileContent, true, out image);
            try
            {
                FileSize = imageChecker.DataSize;
                ImageWidth = imageChecker.Width;
                ImageHeight = imageChecker.Height;

                if (CheckResult != ImageCheckResult.JPEGImage &&
                    CheckResult != ImageCheckResult.GIFImage &&
                    CheckResult != ImageCheckResult.PNGImage)
                {
                    ErrorMessage = imageChecker.FormatErrorMessage(CheckResult);
                    return false;
                }
                else
                {
                    IsImage = true;

                    extension = (CheckResult == ImageCheckResult.PNGImage ? ".png" :
                        (CheckResult == ImageCheckResult.GIFImage ? ".gif" : ".jpg"));

                    var temporaryPath = UploadHelper.TemporaryPath;
                    Directory.CreateDirectory(temporaryPath);
                    TemporaryFileHelper.PurgeDirectoryDefault(temporaryPath);

                    string baseFileName = System.IO.Path.Combine(temporaryPath, Guid.NewGuid().ToString("N"));

                    FilePath = baseFileName + extension;
                    fileContent.Seek(0, SeekOrigin.Begin);
                    using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                        fileContent.CopyTo(fs);

                    if (ThumbWidth > 0 || ThumbHeight > 0)
                    {
                        using (System.Drawing.Image thumbImage =
                            ThumbnailGenerator.Generate(image, ThumbWidth, ThumbHeight, ThumbScaleMode, ThumbBackColor))
                        {
                            ThumbFile = baseFileName + "_t.jpg";

                            if (ThumbQuality != 0)
                            {
                                var p = new System.Drawing.Imaging.EncoderParameters(1);
                                p.Param[0] = new EncoderParameter(Encoder.Quality, ThumbQuality);

                                ImageCodecInfo jpegCodec = null;
                                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                                // Find the correct image codec 
                                for (int i = 0; i < codecs.Length; i++)
                                    if (codecs[i].MimeType == "image/jpeg")
                                        jpegCodec = codecs[i];

                                thumbImage.Save(ThumbFile, jpegCodec, p);
                            }
                            else
                                thumbImage.Save(ThumbFile, System.Drawing.Imaging.ImageFormat.Jpeg);

                            ThumbHeight = thumbImage.Width;
                            ThumbWidth = thumbImage.Height;
                        }
                    }

                    return true;
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
#endif
    }
}