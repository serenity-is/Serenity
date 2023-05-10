using Serenity.Web;
using System.IO;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="FileUploadEditorAttribute"/> and
/// <see cref="ImageUploadEditorAttribute"/>.
/// </summary>
public class FileUploadBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private IUploadEditor editorAttr;
    private string fileNameFormat;
    private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";
    private readonly IUploadValidator uploadValidator;
    private readonly IImageProcessor imageProcessor;
    private readonly IUploadStorage storage;
    private readonly IFilenameFormatSanitizer formatSanitizer;
    private StringField originalNameField;
    private Dictionary<string, Field> replaceFields;

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="uploadValidator">Upload validator</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="storage">Upload storage</param>
    /// <param name="formatSanitizer">Filename format sanitizer</param>
    /// <exception cref="ArgumentNullException">One of the arguments is null</exception>
    public FileUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor,
        IUploadStorage storage, 
        IFilenameFormatSanitizer formatSanitizer = null)
    {
        this.uploadValidator = uploadValidator ?? throw new ArgumentNullException(nameof(uploadValidator));
        this.imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        this.formatSanitizer = formatSanitizer ?? DefaultFilenameFormatSanitizer.Instance;
    }

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        editorAttr = Target.CustomAttributes.OfType<IUploadEditor>().FirstOrDefault();
        if (editorAttr is null || editorAttr.DisableDefaultBehavior || editorAttr.IsMultiple)
            return false;

        if (Target is not StringField)
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

        if (row is not IIdRow)
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

        var originalNameProperty = (editorAttr as IUploadFileOptions)?.OriginalNameProperty;
        if (!string.IsNullOrEmpty(originalNameProperty))
        {
            var nameField = row.FindFieldByPropertyName(originalNameProperty) ??
                row.FindField(originalNameProperty);

            originalNameField = (StringField)nameField ?? throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but " +
                "a field with OriginalNameProperty '{2}' is not found!",
                Target.PropertyName ?? Target.Name, 
                row.GetType().FullName,
                originalNameProperty));
        }

        var format = (editorAttr as IUploadFileOptions)?.FilenameFormat;
        if (format == null)
        {
            format = row.GetType().Name;
            if (format.EndsWith("Row", StringComparison.Ordinal))
                format = format[..^3];
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
                actualName = fieldName[..colon];

            var replaceField = row.FindFieldByPropertyName(actualName) ??
                row.FindField(actualName);

            if (replaceField is null)
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

    internal static string ProcessReplaceFields(string s,
        Dictionary<string, Field> replaceFields, 
        ISaveRequestHandler handler,
        IFilenameFormatSanitizer formatSanitizer)
    {
        var result = s;

        if (replaceFields == null)
            return result;

        if (formatSanitizer is null)
            throw new ArgumentNullException(nameof(formatSanitizer));

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
            string value;

            string key = p.Key;
            var colon = key.IndexOf(":", StringComparison.Ordinal);
            if (colon >= 0)
            {
                key = key[1..colon];
                value = string.Format(CultureInfo.InvariantCulture, string.Concat("{0:", p.Key.AsSpan(colon + 1, p.Key.Length - colon - 2), "}"), val);
            }
            else
            {
                key = key[1..^1];
                value = Convert.ToString(val ?? "", CultureInfo.InvariantCulture);
            }

            value = formatSanitizer.SanitizePlaceholder(key, value);

            result = result.Replace(p.Key, value, StringComparison.Ordinal);
        }

        result = formatSanitizer.SanitizeResult(result);

        return result;
    }

    /// <inheritdoc/>
    public override void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
    {
        base.OnPrepareQuery(handler, query);

        if (replaceFields != null)
        {
            foreach (var field in replaceFields.Values)
            {
                if (!field.IsTableField() &&
                    (query is not ISqlQueryExtensible ex ||
                      ex.GetSelectIntoIndex(field) <= 0))
                    query.Select(field);
            }
        }
    }

    /// <inheritdoc/>
    public override void OnBeforeSave(ISaveRequestHandler handler)
    {
        var filesToDelete = new FilesToDelete(storage);
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);
        handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] = filesToDelete;

        var filename = (StringField)Target;
        var oldFilename = handler.IsCreate ? null : filename[handler.Old];
        var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();

        if (oldFilename.IsTrimmedSame(newFilename))
        {
            filename[handler.Row] = oldFilename;
            return;
        }

        DeleteOldFile(storage, filesToDelete, oldFilename, 
            copyToHistory: (editorAttr as IUploadFileOptions)?.CopyToHistory == true);

        if (newFilename == null)
        {
            if (oldFilename.IsTrimmedEmpty())
                return;

            filename[handler.Row] = null;

            if (originalNameField is not null)
                originalNameField[handler.Row] = null;

            return;
        }

        if (!newFilename.StartsWith("temporary/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

        if (originalNameField is not null)
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

    /// <inheritdoc/>
    public override void OnAfterDelete(IDeleteRequestHandler handler)
    {
        if (handler.Row is IIsActiveDeletedRow or IIsDeletedRow or IDeleteLogRow)
            return;

        var filename = (StringField)Target;
        var oldFilename = filename[handler.Row];
        var filesToDelete = new FilesToDelete(storage);
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);

        DeleteOldFile(storage, filesToDelete, oldFilename, 
            copyToHistory: (editorAttr as IUploadFileOptions)?.CopyToHistory == true);
    }

    private CopyTemporaryFileResult CopyTemporaryFile(ISaveRequestHandler handler, IFilesToDelete filesToDelete)
    {
        var fileName = (StringField)Target;
        var newFilename = fileName[handler.Row] = fileName[handler.Row].TrimToNull();
        CheckUploadedImageAndCreateThumbs(editorAttr as IUploadOptions, uploadValidator, imageProcessor, 
            storage, ref newFilename);

        var idField = ((IIdRow)handler.Row).IdField;
        var originalName = storage.GetOriginalName(newFilename);
        if (string.IsNullOrEmpty(originalName))
            originalName = Path.GetFileName(newFilename);

        var copyResult = storage.CopyTemporaryFile(new CopyTemporaryFileOptions
        {
            Format = fileNameFormat,
            PostFormat = s => ProcessReplaceFields(s, replaceFields, handler,
                editorAttr as IFilenameFormatSanitizer ?? formatSanitizer),
            TemporaryFile = newFilename,
            EntityId = idField.AsObject(handler.Row),
            FilesToDelete = filesToDelete,
            OriginalName = originalName 
        });

        return copyResult;
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Checks the uploaded image and creates thumbs if required based on options
    /// </summary>
    /// <param name="uploadOptions">Upload options</param>
    /// <param name="validator">Upload validator</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="storage">Upload storage</param>
    /// <param name="temporaryFile">Temporary file</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void CheckUploadedImageAndCreateThumbs(
        IUploadOptions uploadOptions, IUploadValidator validator, IImageProcessor imageProcessor,
        IUploadStorage storage, ref string temporaryFile)
    {
        if (storage == null)
            throw new ArgumentNullException(nameof(storage));

        UploadPathHelper.CheckFileNameSecurity(temporaryFile);

        using var fs = storage.OpenFile(temporaryFile);

        storage.PurgeTemporaryFiles();

        validator.ValidateFile(uploadOptions as IUploadFileConstraints ?? new UploadOptions(), 
            fs, temporaryFile, out bool isImageExtension);

        if (!isImageExtension)
            return;

        validator.ValidateImage(uploadOptions as IUploadImageContrains ?? new UploadOptions(),
            fs, temporaryFile, out object image);

        if (image != null)
        {
            try
            {

                fs.Close();
                temporaryFile = UploadStorageExtensions.ScaleImageAndCreateAllThumbs(image, imageProcessor,
                    uploadOptions as IUploadImageOptions ?? new UploadOptions(), 
                    storage, temporaryFile, OverwriteOption.Overwrite);
            }
            finally
            {
                (image as IDisposable)?.Dispose();
            }
        }
    }
}