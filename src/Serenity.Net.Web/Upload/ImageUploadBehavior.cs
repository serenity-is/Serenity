using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Serenity.Services
{
    public class ImageUploadBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private ImageUploadEditorAttribute attr;
        private string fileNameFormat;
        private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";
        private readonly ITextLocalizer localizer;
        private readonly IUploadStorage storage;
        private StringField originalNameField;
        private Dictionary<string, Field> replaceFields;

        public ImageUploadBehavior(IUploadStorage storage, ITextLocalizer localizer)
        {
            this.storage = storage;
            this.localizer = localizer;
        }

        public bool ActivateFor(IRow row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<ImageUploadEditorAttribute>();
            if (attr == null || attr.DisableDefaultBehavior || attr.EditorType != "ImageUpload")
                return false;

            if (!(Target is StringField))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!(row is IIdRow))
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!attr.OriginalNameProperty.IsEmptyOrNull())
            {
                var originalNameField = row.FindFieldByPropertyName(attr.OriginalNameProperty) ??
                    row.FindField(attr.OriginalNameProperty);

                if (ReferenceEquals(null, originalNameField))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                        "Field '{0}' on row type '{1}' has a UploadEditor attribute but " +
                        "a field with OriginalNameProperty '{2}' is not found!",
                            Target.PropertyName ?? Target.Name, 
                            row.GetType().FullName,
                            attr.OriginalNameProperty));

                this.originalNameField = (StringField)originalNameField;
            }

            var format = attr.FilenameFormat;

            if (format == null)
            {
                format = row.GetType().Name;
                if (format.EndsWith("Row", StringComparison.Ordinal))
                    format = format.Substring(0, format.Length - 3);
                format += "/~";
            }

            fileNameFormat = format.Replace("~", SplittedFormat, StringComparison.Ordinal);
            replaceFields = ParseReplaceFields(fileNameFormat, row, Target);

            return true;
        }

        internal static Dictionary<string, Field> ParseReplaceFields(string fileNameFormat, IRow row, Field target)
        {
            if (fileNameFormat.IndexOf('|', StringComparison.Ordinal) < 0)
                return null;

            var replaceFields = new Dictionary<string, Field>();

            int start = 0;
            while ((start = fileNameFormat.IndexOf('|', start)) >= 0)
            {
                var end = fileNameFormat.IndexOf('|', start + 1);
                if (end <= start + 1)
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Field '{0}' on row type '{1}' has a UploadEditor attribute " +
                        "with invalid format string '{2}'!",
                            target.PropertyName ?? target.Name,
                            row.GetType().FullName,
                            fileNameFormat));

                var fieldName = fileNameFormat.Substring(start + 1, end - start - 1);
                var actualName = fieldName;
                var colon = fieldName.IndexOf(":", StringComparison.Ordinal);
                if (colon >= 0)
                    actualName = fieldName.Substring(0, colon);

                var replaceField = row.FindFieldByPropertyName(actualName) ??
                    row.FindField(actualName);

                if (ReferenceEquals(null, replaceField))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        "Field '{0}' on row type '{1}' has a UploadEditor attribute that " +
                        "references field '{2}', but no such field is found!'",
                            target.PropertyName ?? target.Name,
                            row.GetType().FullName,
                            actualName));
                }

                replaceFields['|' + fieldName + '|'] = replaceField;

                start = end + 1;
            }

            return replaceFields;
        }

        internal static string ProcessReplaceFields(string s, Dictionary<string, Field> replaceFields, ISaveRequestHandler handler)
        {
            if (replaceFields == null)
                return s;

            var row = handler.Row;

            // foreign / calculated fields might not be available yet in new row
            // so load them from database 

            // TODO: if referenced foreign fields changed on update, 
            // values might be wrong in before update where we set filename
            // so need to handle update in AfterSave just like create

            if (handler.IsCreate &&
                replaceFields.Values.Any(x => !x.IsTableField()))
            {
                var idField = ((IIdRow)handler.Row).IdField;

                row = handler.Row.Clone();
                var query = new SqlQuery()
                    .From(row);

                foreach (var field in replaceFields.Values)
                    query.Select(field);

                query.Where(idField == new ValueCriteria(idField.AsObject(row)));

                query.GetFirst(handler.Connection);
            }

            foreach (var p in replaceFields)
            {
                var val = p.Value.AsObject(row);
                string str;

                var colon = p.Key.IndexOf(":", StringComparison.Ordinal);
                if (colon >= 0)
                    str = string.Format(CultureInfo.InvariantCulture, "{0:" + p.Key.Substring(colon + 1, p.Key.Length - colon - 2) + "}", val);
                else
                    str = Convert.ToString(val ?? "", CultureInfo.InvariantCulture);

                str = StringHelper.SanitizeFilename(str).Replace('\\', '_').Replace("..", "_", StringComparison.Ordinal);
                if (string.IsNullOrWhiteSpace(str))
                    str = "_";

                while (str.EndsWith(".", StringComparison.Ordinal))
                    str = str.Substring(0, str.Length - 1) + "_";

                s = s.Replace(p.Key, str, StringComparison.Ordinal);
            }

            while (s.IndexOf("//", StringComparison.Ordinal) > 0)
                s = s.Replace("//", "/_/", StringComparison.Ordinal);

            return s;
        }

        public override void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
        {
            base.OnPrepareQuery(handler, query);

            if (replaceFields != null)
            {
                foreach (var field in replaceFields.Values)
                {
                    if (!field.IsTableField() &&
                        (!(query is ISqlQueryExtensible) ||
                          ((ISqlQueryExtensible)query).GetSelectIntoIndex(field) <= 0))
                        query.Select(field);
                }
            }
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            var filesToDelete = new FilesToDelete(storage);
            handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);
            handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] = filesToDelete;

            var filename = (StringField)(Target);
            var oldFilename = handler.IsCreate ? null : filename[handler.Old];
            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();

            if (oldFilename.IsTrimmedSame(newFilename))
            {
                filename[handler.Row] = oldFilename;
                return;
            }

            DeleteOldFile(storage, filesToDelete, oldFilename, attr.CopyToHistory);

            if (newFilename == null)
            {
                if (oldFilename.IsTrimmedEmpty())
                    return;

                filename[handler.Row] = null;

                if (!(originalNameField is null))
                    originalNameField[handler.Row] = null;

                return;
            }

            if (!newFilename.StartsWith("temporary/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

            if (!ReferenceEquals(null, originalNameField))
            {
                var originalName = storage.GetOriginalName(newFilename).TrimToNull();

                originalNameField[handler.Row] = originalName;
            }

            if (handler.IsUpdate)
            {
                var copyResult = CopyTemporaryFile(handler, filesToDelete);
                filename[handler.Row] = copyResult.Path;
            }
        }

        internal static void DeleteOldFile(IUploadStorage storage, FilesToDelete filesToDelete, string oldFilename, bool copyToHistory)
        {
            if (!oldFilename.IsEmptyOrNull())
            {
                filesToDelete.RegisterOldFile(oldFilename);

                if (copyToHistory)
                {
                    if (storage.FileExists(oldFilename))
                        storage.ArchiveFile(oldFilename);
                }
            }
        }

        public override void OnAfterDelete(IDeleteRequestHandler handler)
        {
            if (handler.Row is IIsActiveDeletedRow ||
                handler.Row is IIsDeletedRow ||
                handler.Row is IDeleteLogRow)
                return;

            var filename = (StringField)(Target);
            var oldFilename = filename[handler.Row];
            var filesToDelete = new FilesToDelete(storage);
            handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);

            DeleteOldFile(storage, filesToDelete, oldFilename, attr.CopyToHistory);
        }

        private CopyTemporaryFileResult CopyTemporaryFile(ISaveRequestHandler handler, FilesToDelete filesToDelete)
        {
            var fileName = (StringField)Target;
            var newFilename = fileName[handler.Row] = fileName[handler.Row].TrimToNull();
            CheckUploadedImageAndCreateThumbs(attr, localizer, storage, ref newFilename);

            var idField = ((IIdRow)handler.Row).IdField;

            var copyResult = storage.CopyTemporaryFile(new CopyTemporaryFileOptions
            {
                Format = fileNameFormat,
                PostFormat = s => ProcessReplaceFields(s, replaceFields, handler),
                TemporaryFile = newFilename,
                EntityId = idField.AsObject(handler.Row),
                FilesToDelete = filesToDelete,
                OriginalName = storage.GetOriginalName(newFilename)
            });

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

            var filesToDelete = handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] as FilesToDelete;
            var copyResult = CopyTemporaryFile(handler, filesToDelete);
            var idField = ((IIdRow)handler.Row).IdField;

            new SqlUpdate(handler.Row.Table)
                .Set(filename, copyResult.Path)
                .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
                .Execute(handler.UnitOfWork.Connection);

            filename[handler.Row] = copyResult.Path;
        }

        public static void CheckUploadedImageAndCreateThumbs(ImageUploadEditorAttribute attr, ITextLocalizer localizer,
            IUploadStorage storage, ref string temporaryFile)
        {
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            ImageCheckResult[] supportedFormats = null;

            if (!attr.AllowNonImage)
                supportedFormats = new ImageCheckResult[] {
                    ImageCheckResult.JPEGImage,
                    ImageCheckResult.GIFImage,
                    ImageCheckResult.PNGImage
                };

            UploadPathHelper.CheckFileNameSecurity(temporaryFile);

            var checker = new ImageChecker();
            checker.MinWidth = attr.MinWidth;
            checker.MaxWidth = attr.MaxWidth;
            checker.MinHeight = attr.MinHeight;
            checker.MaxHeight = attr.MaxHeight;
            checker.MaxDataSize = attr.MaxSize;

            Image image = null;
            try
            {
                using var fs = storage.OpenFile(temporaryFile);
                if (attr.MinSize != 0 && fs.Length < attr.MinSize)
                    throw new ValidationError(string.Format(CultureInfo.CurrentCulture, 
                        Texts.Controls.ImageUpload.UploadFileTooSmall.ToString(localizer),
                        UploadFormatting.FileSizeDisplay(attr.MinSize)));

                if (attr.MaxSize != 0 && fs.Length > attr.MaxSize)
                    throw new ValidationError(string.Format(CultureInfo.CurrentCulture, 
                        Texts.Controls.ImageUpload.UploadFileTooBig.ToString(localizer),
                        UploadFormatting.FileSizeDisplay(attr.MaxSize)));

                ImageCheckResult result;
                if (string.Compare(Path.GetExtension(temporaryFile), ".swf", StringComparison.OrdinalIgnoreCase) == 0)
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
                    string error = checker.FormatErrorMessage(result, localizer);
                    throw new ValidationError(error);
                }

                if (result >= ImageCheckResult.FlashMovie)
                    return;

                string baseFile = Path.ChangeExtension(temporaryFile, null);

                storage.PurgeTemporaryFiles();

                if ((attr.ScaleWidth > 0 || attr.ScaleHeight > 0) &&
                    ((attr.ScaleWidth > 0 && (attr.ScaleSmaller || checker.Width > attr.ScaleWidth)) ||
                        (attr.ScaleHeight > 0 && (attr.ScaleSmaller || checker.Height > attr.ScaleHeight))))
                {
                    using (Image scaledImage = ThumbnailGenerator.Generate(
                        image, attr.ScaleWidth, attr.ScaleHeight, attr.ScaleMode, Color.Empty))
                    {
                        temporaryFile = baseFile + ".jpg";
                        fs.Close();
                        using (var ms = new MemoryStream())
                        {
                            scaledImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            ms.Seek(0, SeekOrigin.Begin);
                            temporaryFile = storage.WriteFile(temporaryFile, ms, false);
                        }
                    }
                }

                var thumbSizes = attr.ThumbSizes.TrimToNull();
                if (thumbSizes == null)
                    return;

                foreach (var sizeStr in thumbSizes.Replace(";", ",", StringComparison.Ordinal).Split(new char[] { ',' }))
                {
                    var dims = sizeStr.ToUpperInvariant().Split(new char[] { 'X' });
                    int w, h;
                    if (dims.Length != 2 ||
                        !int.TryParse(dims[0], out w) ||
                        !int.TryParse(dims[1], out h) ||
                        w < 0 ||
                        h < 0 ||
                        (w == 0 && h == 0))
                        throw new ArgumentOutOfRangeException(nameof(thumbSizes));

                    using (Image thumbImage = ThumbnailGenerator.Generate(image, w, h, attr.ThumbMode, Color.Empty))
                    {
                        string thumbFile = baseFile + "_t" + w.ToInvariant() + "x" + h.ToInvariant() + ".jpg";
                        using (var ms = new MemoryStream())
                        {
                            thumbImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            ms.Seek(0, SeekOrigin.Begin);
                            storage.WriteFile(thumbFile, ms, false);
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