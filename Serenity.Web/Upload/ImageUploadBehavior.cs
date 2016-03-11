using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.IO;
using Serenity.Web;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Serenity.Services
{
    public class ImageUploadBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private ImageUploadEditorAttribute attr;
        private string fileNameFormat;
        private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";
        private UploadHelper uploadHelper;
        private StringField originalNameField;

        public bool ActivateFor(Row row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<ImageUploadEditorAttribute>();
            if (attr == null || attr.DisableDefaultBehavior || attr.EditorType != "ImageUpload")
                return false;

            if (!(Target is StringField))
                throw new ArgumentException(String.Format(
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!(row is IIdRow))
                throw new ArgumentException(String.Format(
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!attr.OriginalNameProperty.IsEmptyOrNull())
            {
                var originalNameField = row.FindFieldByPropertyName(attr.OriginalNameProperty) ??
                    row.FindField(attr.OriginalNameProperty);

                if (ReferenceEquals(null, originalNameField))
                    throw new ArgumentException(String.Format(
                        "Field '{0}' on row type '{1}' has a UploadEditor attribute but " +
                        "a field with OriginalNameProperty '{2} is not found!'",
                            Target.PropertyName ?? Target.Name, 
                            row.GetType().FullName,
                            attr.OriginalNameProperty));

                this.originalNameField = (StringField)originalNameField;
            }

            var format = attr.FilenameFormat;

            if (format == null)
            {
                format = row.GetType().Name;
                if (format.EndsWith("Row"))
                    format = format.Substring(0, format.Length - 3);
                format += "/~";
            }

            this.fileNameFormat = format.Replace("~", SplittedFormat);
            this.uploadHelper = new UploadHelper((attr.SubFolder.IsEmptyOrNull() ? "" : (attr.SubFolder + "/")) + (this.fileNameFormat));

            return true;
        }

        private class UploadedFile
        {
            public string Filename { get; set; }
            public string OriginalName { get; set; }
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            var filesToDelete = new FilesToDelete();
            UploadHelper.RegisterFilesToDelete(handler.UnitOfWork, filesToDelete);
            handler.StateBag[this.GetType().FullName + "_" + Target.Name + "_FilesToDelete"] = filesToDelete;

            var filename = (StringField)(Target);
            var oldFilename = handler.IsCreate ? null : filename[handler.Old];
            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();

            if (oldFilename.IsTrimmedSame(newFilename))
            {
                filename[handler.Row] = oldFilename;
                return;
            }

            DeleteOldFile(filesToDelete, oldFilename);

            if (newFilename == null)
            {
                if (oldFilename.IsTrimmedEmpty())
                    return;

                filename[handler.Row] = null;

                if (!ReferenceEquals(null, originalNameField))
                    originalNameField[handler.Row] = null;

                return;
            }

            if (!newFilename.ToLowerInvariant().StartsWith("temporary/"))
                throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

            if (!ReferenceEquals(null, originalNameField))
            {
                var originalName = File.ReadAllText(Path.ChangeExtension(
                    UploadHelper.DbFilePath(newFilename), ".orig")).TrimToNull();

                originalNameField[handler.Row] = originalName;
            }

            if (handler.IsUpdate)
            {
                var copyResult = CopyTemporaryFile(handler, filesToDelete);
                filename[handler.Row] = copyResult.DbFileName;
            }
        }

        private void DeleteOldFile(FilesToDelete filesToDelete, string oldFilename)
        {
            if (!oldFilename.IsEmptyOrNull())
            {
                var actualOldFile = (attr.SubFolder.IsEmptyOrNull() ? "" : (attr.SubFolder + "/")) + oldFilename;
                filesToDelete.RegisterOldFile(actualOldFile);

                if (attr.CopyToHistory)
                {
                    var oldFilePath = UploadHelper.ToPath(actualOldFile);
                    string date = DateTime.UtcNow.ToString("yyyyMM", Invariants.DateTimeFormat);
                    string historyFile = "history/" + date + "/" + Path.GetFileName(oldFilePath);
                    if (File.Exists(UploadHelper.DbFilePath(oldFilePath)))
                        UploadHelper.CopyFileAndRelated(UploadHelper.DbFilePath(oldFilePath), UploadHelper.DbFilePath(historyFile), overwrite: true);
                }
            }
        }

        public override void OnAfterDelete(IDeleteRequestHandler handler)
        {
            if (handler.Row is IIsActiveDeletedRow ||
                handler.Row is IDeleteLogRow)
                return;

            var filename = (StringField)(Target);
            var oldFilename = filename[handler.Row];
            var filesToDelete = new FilesToDelete();
            UploadHelper.RegisterFilesToDelete(handler.UnitOfWork, filesToDelete);

            DeleteOldFile(filesToDelete, oldFilename);
        }

        private CopyTemporaryFileResult CopyTemporaryFile(ISaveRequestHandler handler, FilesToDelete filesToDelete)
        {
            var fileName = (StringField)Target;
            var newFilename = fileName[handler.Row] = fileName[handler.Row].TrimToNull();
            CheckUploadedImageAndCreateThumbs(attr, ref newFilename);

            var idField = (Field)(((IIdRow)handler.Row).IdField);
            var copyResult = uploadHelper.CopyTemporaryFile(newFilename, idField.AsObject(handler.Row), filesToDelete);
            if (!attr.SubFolder.IsEmptyOrNull())
                copyResult.DbFileName = copyResult.DbFileName.Substring(attr.SubFolder.Length + 1);
            return copyResult;
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            var filename = (StringField)Target;

            if (handler.IsUpdate)
                return;

            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();
            if (newFilename == null)
                return;

            var filesToDelete = handler.StateBag[this.GetType().FullName + "_" + Target.Name + "_FilesToDelete"] as FilesToDelete;
            var copyResult = CopyTemporaryFile(handler, filesToDelete);
            var idField = (Field)(((IIdRow)handler.Row).IdField);

            new SqlUpdate(handler.Row.Table)
                .Set(filename, copyResult.DbFileName)
                .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
                .Execute(handler.UnitOfWork.Connection);
        }

        public static void CheckUploadedImageAndCreateThumbs(ImageUploadEditorAttribute attr, ref string temporaryFile)
        {
            ImageCheckResult[] supportedFormats = null;

            if (!attr.AllowNonImage)
                supportedFormats = new ImageCheckResult[] {
                    ImageCheckResult.JPEGImage,
                    ImageCheckResult.GIFImage,
                    ImageCheckResult.PNGImage
                };

            UploadHelper.CheckFileNameSecurity(temporaryFile);

            var checker = new ImageChecker();
            checker.MinWidth = attr.MinWidth;
            checker.MaxWidth = attr.MaxWidth;
            checker.MinHeight = attr.MinHeight;
            checker.MaxHeight = attr.MaxHeight;
            checker.MaxDataSize = attr.MaxSize;

            Image image = null;
            try
            {
                var temporaryPath = UploadHelper.DbFilePath(temporaryFile);
                using (var fs = new FileStream(temporaryPath, FileMode.Open))
                {
                    if (attr.MinSize != 0 && fs.Length < attr.MinSize)
                        throw new ValidationError(String.Format(Texts.Controls.ImageUpload.UploadFileTooSmall,
                            UploadHelper.FileSizeDisplay(attr.MinSize)));

                    if (attr.MaxSize != 0 && fs.Length > attr.MaxSize)
                        throw new ValidationError(String.Format(Texts.Controls.ImageUpload.UploadFileTooBig,
                            UploadHelper.FileSizeDisplay(attr.MaxSize)));

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

                    if (result == ImageCheckResult.InvalidImage &&
                        attr.AllowNonImage)
                    {
                        return;
                    }

                    if (result > ImageCheckResult.UnsupportedFormat || 
                        (supportedFormats != null && Array.IndexOf(supportedFormats, result) < 0))
                    {
                        string error = checker.FormatErrorMessage(result);
                        throw new ValidationError(error);
                    }

                    if (result >= ImageCheckResult.FlashMovie)
                        return;

                    string basePath = UploadHelper.TemporaryPath;
                    string baseFile = Path.GetFileNameWithoutExtension(Path.GetFileName(temporaryPath));

                    TemporaryFileHelper.PurgeDirectoryDefault(basePath);

                    if ((attr.ScaleWidth > 0 || attr.ScaleHeight > 0) &&
                        ((attr.ScaleWidth > 0 && (attr.ScaleSmaller || checker.Width > attr.ScaleWidth)) ||
                            (attr.ScaleHeight > 0 && (attr.ScaleSmaller || checker.Height > attr.ScaleHeight))))
                    {
                        using (Image scaledImage = ThumbnailGenerator.Generate(
                            image, attr.ScaleWidth, attr.ScaleHeight, attr.ScaleMode, Color.Empty))
                        {
                            temporaryFile = baseFile + ".jpg";
                            fs.Close();
                            scaledImage.Save(Path.Combine(basePath, temporaryFile), System.Drawing.Imaging.ImageFormat.Jpeg);
                            temporaryFile = "temporary/" + temporaryFile;
                        }
                    }

                    var thumbSizes = attr.ThumbSizes.TrimToNull();
                    if (thumbSizes == null)
                        return;

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

                        using (Image thumbImage = ThumbnailGenerator.Generate(image, w, h, attr.ThumbMode, Color.Empty))
                        {
                            string thumbFile = Path.Combine(basePath,
                                baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg");

                            thumbImage.Save(thumbFile);
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