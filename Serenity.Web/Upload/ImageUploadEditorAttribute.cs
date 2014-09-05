using Serenity.IO;
using Serenity.Services;
using Serenity.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Serenity.ComponentModel
{
    public partial class ImageUploadEditorAttribute
    {
        public ImageUploadEditorAttribute(string originalNameProperty = null,
            int minBytes = 0, int maxBytes = 0, int minWidth = 0, int maxWidth = 0, int minHeight = 0, int maxHeight = 0, 
            bool allowFlash = false, int scaleWidth = 0, int scaleHeight = 0, bool scaleSmaller = true, 
            ImageScaleMode scaleMode = ImageScaleMode.CropSourceImage, string thumbSizes = null, 
            ImageScaleMode thumbMode = ImageScaleMode.CropSourceImage, int thumbQuality = 0)
            : base("ImageUpload")
        {
            OriginalNameProperty = originalNameProperty;
            MinBytes = minBytes;
            MaxBytes = maxBytes;
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            MinHeight = minHeight;
            MaxHeight = maxHeight;
            ScaleWidth = scaleWidth;
            ScaleHeight = scaleHeight;
            ScaleSmaller = scaleSmaller;
            ScaleMode = scaleMode;
            ThumbSizes = thumbSizes;
            ThumbMode = thumbMode;
            ThumbQuality = ThumbQuality;
        }

        public override void SetParams(IDictionary<string, object> editorParams)
        {
            base.SetParams(editorParams);

            editorParams["originalNameProperty"] = OriginalNameProperty;
            editorParams["minWidth"] = MinWidth;
            editorParams["maxWidth"] = MaxWidth;
            editorParams["minHeight"] = MinHeight;
            editorParams["maxHeight"] = MaxHeight;
        }

        public int MinBytes { get; private set; }
        public int MaxBytes { get; private set; }
        public int ScaleWidth { get; private set; }
        public int ScaleHeight { get; private set; }
        public bool ScaleSmaller { get; private set; }
        public ImageScaleMode ScaleMode { get; private set; }
        public string ThumbSizes { get; private set; }
        public ImageScaleMode ThumbMode { get; private set; }
        public int ThumbQuality { get; private set; }

        public void CheckUploadedImageAndCreateThumbs(ref string temporaryFile, 
            params ImageCheckResult[] supportedFormats)
        {
            if (supportedFormats == null ||
                supportedFormats.Length == 0)
                supportedFormats = new ImageCheckResult[] { ImageCheckResult.JPEGImage, ImageCheckResult.GIFImage, ImageCheckResult.PNGImage };

            UploadHelper.CheckFileNameSecurity(temporaryFile);

            var checker = new ImageChecker();
            checker.MinWidth = this.MinWidth;
            checker.MaxWidth = this.MaxWidth;
            checker.MinHeight = this.MinHeight;
            checker.MaxHeight = this.MaxHeight;
            checker.MaxDataSize = this.MaxBytes;

            Image image = null;
            try
            {
                var temporaryPath = Path.Combine(UploadHelper.TemporaryPath, temporaryFile);
                using (var fs = new FileStream(temporaryPath, FileMode.Open))
                {
                    if (this.MinBytes != 0 && fs.Length < this.MinBytes)
                        throw new ValidationError(String.Format("Yükleyeceğiniz dosya en az {0} boyutunda olmalı!",
                            UploadHelper.FileSizeDisplay(this.MinBytes)));

                    if (this.MaxBytes != 0 && fs.Length > this.MaxBytes)
                        throw new ValidationError(String.Format("Yükleyeceğiniz dosya en çok {0} boyutunda olabilir!",
                            UploadHelper.FileSizeDisplay(this.MaxBytes)));

                    ImageCheckResult result;
                    if (Path.GetExtension(temporaryFile).ToLowerInvariant() == ".swf")
                    {
                        result = ImageCheckResult.FlashMovie;
                        // validate swf file somehow!
                    }
                    else
                    {
                        result = checker.CheckStream(fs, true, out image);
                    }

                    if (result > ImageCheckResult.FlashMovie ||
                        Array.IndexOf(supportedFormats, result) < 0)
                    {
                        string error = checker.FormatErrorMessage(result);
                        throw new ValidationError(error);
                    }

                    if (result != ImageCheckResult.FlashMovie)
                    {
                        string basePath = UploadHelper.TemporaryPath;
                        string baseFile = Path.GetFileNameWithoutExtension(temporaryFile);

                        TemporaryFileHelper.PurgeDirectoryDefault(basePath);

                        if ((this.ScaleWidth > 0 || this.ScaleHeight > 0) &&
                            ((this.ScaleWidth > 0 && (this.ScaleSmaller || checker.Width > this.ScaleWidth)) ||
                             (this.ScaleHeight > 0 && (this.ScaleSmaller || checker.Height > this.ScaleHeight))))
                        {
                            using (Image scaledImage = ThumbnailGenerator.Generate(
                                image, this.ScaleWidth, this.ScaleHeight, this.ScaleMode, Color.Empty))
                            {
                                temporaryFile = baseFile + ".jpg";
                                fs.Close();
                                scaledImage.Save(Path.Combine(basePath, temporaryFile), System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }

                        var thumbSizes = this.ThumbSizes.TrimToNull();
                        if (thumbSizes != null)
                        {
                            foreach (var sizeStr in thumbSizes.Replace(";", ",").Split(new char[] { ',' }))
                            {
                                var dims = sizeStr.ToLowerInvariant().Split(new char[] { 'x' });
                                int w, h;
                                if (dims.Length != 2 ||
                                    !Int32.TryParse(dims[0], out w) ||
                                    !Int32.TryParse(dims[1], out h) ||
                                    w < 0 ||
                                    h < 0 ||
                                    (w == 0 && h == 0))
                                    throw new ArgumentOutOfRangeException("thumbSizes");

                                using (Image thumbImage = ThumbnailGenerator.Generate(image, w, h, this.ThumbMode, Color.Empty))
                                {
                                    string thumbFile = Path.Combine(basePath,
                                        baseFile + "t_" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg");

                                    if (this.ThumbQuality != 0)
                                    {
                                        var p = new System.Drawing.Imaging.EncoderParameters(1);
                                        p.Param[0] = new EncoderParameter(Encoder.Quality, 80L);

                                        ImageCodecInfo jpegCodec = null;
                                        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                                        // Find the correct image codec 
                                        for (int i = 0; i < codecs.Length; i++)
                                            if (codecs[i].MimeType == "image/jpeg")
                                                jpegCodec = codecs[i];
                                        thumbImage.Save(thumbFile, jpegCodec, p);
                                    }
                                    else
                                        thumbImage.Save(thumbFile, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (image != null)
                    image.Dispose();
            }
        }
    }
}