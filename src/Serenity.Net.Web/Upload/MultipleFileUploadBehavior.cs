using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="MultipleFileUploadEditorAttribute"/> and
/// <see cref="MultipleImageUploadEditorAttribute"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="uploadValidator">Upload validator</param>
/// <param name="imageProcessor">Image processor</param>
/// <param name="storage">Upload storage</param>
/// <param name="formatSanitizer">Filename format sanitizer</param>
/// <exception cref="ArgumentNullException">One of the arguments is null</exception>
public class MultipleFileUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor,
    IUploadStorage storage,
    IFilenameFormatSanitizer formatSanitizer = null) : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private IUploadEditor editorAttr;
    private string fileNameFormat;
    private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";
    private Dictionary<string, Field> replaceFields;
    private readonly IFilenameFormatSanitizer formatSanitizer = formatSanitizer ?? DefaultFilenameFormatSanitizer.Instance;
    private readonly IUploadValidator uploadValidator = uploadValidator ?? throw new ArgumentNullException(nameof(uploadValidator));
    private readonly IImageProcessor imageProcessor = imageProcessor ?? throw new ArgumentNullException(nameof(imageProcessor));
    private readonly IUploadStorage storage = storage ?? throw new ArgumentNullException(nameof(storage));

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        editorAttr = Target.CustomAttributes.OfType<IUploadEditor>().FirstOrDefault();
        if (editorAttr is null || editorAttr.DisableDefaultBehavior || !editorAttr.IsMultiple)
            return false;

        if (Target is not StringField)
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

        if (row is not IIdRow)
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                    Target.PropertyName ?? Target.Name, row.GetType().FullName));

        var format = (editorAttr as IUploadFileOptions)?.FilenameFormat;

        if (format == null)
        {
            format = row.GetType().Name;
            if (format.EndsWith("Row", StringComparison.Ordinal))
                format = format[..^3];
            format += "/~";
        }

        fileNameFormat = format.Replace("~", SplittedFormat, StringComparison.Ordinal);
        replaceFields = FileUploadBehavior.ParseReplaceFields(fileNameFormat, row, Target);

        return true;
    }

    private static UploadedFile[] ParseAndValidate(string json, string key)
    {
        json = json.TrimToNull();

        if (json != null && (!json.StartsWith('[') ||
            !json.EndsWith(']')))
            throw new ArgumentOutOfRangeException(key);

        var list = JSON.Parse<UploadedFile[]>(json ?? "[]");

        if (list.Any(x => string.IsNullOrEmpty(x.Filename)) ||
            list.GroupBy(x => x.Filename.Trim()).SelectMany(x => x.Skip(1)).Any())
            throw new ArgumentOutOfRangeException(key);

        return list;
    }

    /// <inheritdoc/>
    public override void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
    {
        base.OnPrepareQuery(handler, query);

        if (replaceFields == null) 
            return;
        
        foreach (var field in replaceFields.Values)
        {
            if (!field.IsTableField() &&
                (query is not ISqlQueryExtensible ex ||
                 ex.GetSelectIntoIndex(field) <= 0))
                query.Select(field);
        }
    }

    /// <inheritdoc/>
    public override void OnBeforeSave(ISaveRequestHandler handler)
    {
        var field = (StringField)Target;

        if (!handler.Row.IsAssigned(field))
            return;

        var oldFilesJSON = (handler.IsCreate ? null : field[handler.Old]).TrimToNull();
        var newFilesJSON = field[handler.Row] = field[handler.Row].TrimToNull();

        if (oldFilesJSON.IsTrimmedSame(newFilesJSON))
        {
            field[handler.Row] = oldFilesJSON;
            return;
        }

        var oldFileList = ParseAndValidate(oldFilesJSON, "oldFiles");
        var newFileList = ParseAndValidate(newFilesJSON, "newFiles");

        var filesToDelete = new FilesToDelete(storage);
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);
        handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] = filesToDelete;

        foreach (var file in oldFileList)
        {
            var filename = file.Filename.Trim();
            if (newFileList.Any(x => string.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                continue;

            FileUploadBehavior.DeleteOldFile(storage, filesToDelete, filename, 
                copyToHistory: editorAttr is IUploadFileOptions { CopyToHistory: true });
        }

        if (newFileList.IsEmptyOrNull())
        {
            field[handler.Row] = null;
            return;
        }

        if (handler.IsUpdate)
            field[handler.Row] = CopyTemporaryFiles(handler, oldFileList, newFileList, filesToDelete);
    }

    /// <inheritdoc/>
    public override void OnAfterDelete(IDeleteRequestHandler handler)
    {
        if (ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var field = (StringField)Target;
        var oldFilesJSON = field[handler.Row].TrimToNull();
        var oldFileList = ParseAndValidate(oldFilesJSON, "oldFiles");

        var filesToDelete = new FilesToDelete(storage);
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);

        foreach (var file in oldFileList)
            FileUploadBehavior.DeleteOldFile(storage, filesToDelete, file.Filename, 
                editorAttr is IUploadFileOptions { CopyToHistory: true });
    }

    private string CopyTemporaryFiles(ISaveRequestHandler handler,
        UploadedFile[] oldFileList, UploadedFile[] newFileList, FilesToDelete filesToDelete)
    {
        foreach (var file in newFileList)
        {
            var filename = file.Filename.Trim();
            if (oldFileList.Any(x => string.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                continue;

            if (!filename.StartsWith("temporary/", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

            FileUploadBehavior.CheckUploadedImageAndCreateThumbs(editorAttr as IUploadOptions, uploadValidator, imageProcessor,
                storage, ref filename);

            var idField = ((IIdRow)handler.Row).IdField;

            var originalName = storage.GetOriginalName(filename);
            if (string.IsNullOrEmpty(originalName))
                originalName = System.IO.Path.GetFileName(filename);

            var copyResult = storage.CopyTemporaryFile(new CopyTemporaryFileOptions
            {
                Format = fileNameFormat,
                PostFormat = s => FileUploadBehavior.ProcessReplaceFields(s, replaceFields, handler,
                    editorAttr as IFilenameFormatSanitizer ?? formatSanitizer),
                TemporaryFile = filename,
                EntityId = idField.AsObject(handler.Row),
                FilesToDelete = filesToDelete,
                OriginalName = originalName
            });

            file.Filename = copyResult.Path;
        }

        return JSON.Stringify(newFileList, writeNulls: false);
    }

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        if (handler.IsUpdate)
            return;

        var field = (StringField)Target;
        if (!handler.Row.IsAssigned(field))
            return;

        var newFilesJSON = field[handler.Row] = field[handler.Row].TrimToNull();
        var newFileList = ParseAndValidate(newFilesJSON, "newFiles");

        if (newFileList.IsEmptyOrNull())
            return;

        var filesToDelete = handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] as FilesToDelete;
        var copyResult = CopyTemporaryFiles(handler, [], newFileList, filesToDelete);
        var idField = handler.Row.IdField;

        new SqlUpdate(handler.Row.Table)
            .Set(field, copyResult)
            .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
            .Execute(handler.UnitOfWork.Connection);

        field[handler.Row] = copyResult;
    }
}