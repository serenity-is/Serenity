using Serenity.Web;
using System.IO;

namespace Serenity.Services;

/// <summary>
/// Behavior class that handles <see cref="FileUploadEditorAttribute"/> and
/// <see cref="ImageUploadEditorAttribute"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="storage">Upload storage</param>
/// <param name="uploadProcessor">Upload processor</param>
/// <param name="formatSanitizer">Filename format sanitizer</param>
/// <exception cref="ArgumentNullException">One of the arguments is null</exception>
public class FileUploadBehavior(IUploadStorage storage, IUploadProcessor uploadProcessor,
    IFilenameFormatSanitizer formatSanitizer = null) : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field Target { get; set; }

    private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";

    private readonly IFilenameFormatSanitizer formatSanitizer = formatSanitizer ?? DefaultFilenameFormatSanitizer.Instance;
    private readonly IUploadStorage storage = storage ?? throw new ArgumentNullException(nameof(storage));
    private readonly IUploadProcessor uploadProcessor = uploadProcessor ?? throw new ArgumentNullException(nameof(uploadProcessor));

    private IUploadEditor editorAttr;
    private string entityTable;
    private string entityType;
    private string entityProperty;
    private string entityField;
    private string fileNameFormat;
    private StringField originalNameField;
    private Dictionary<string, Field> replaceFields;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        editorAttr = Target.CustomAttributes.OfType<IUploadEditor>().FirstOrDefault();
        if (editorAttr is null || editorAttr.DisableDefaultBehavior)
            return false;

        entityField = Target.Name;
        entityProperty = Target.PropertyName ?? entityField;

        if (Target is not StringField)
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                    entityProperty, row.GetType().FullName));

        if (row is not IIdRow)
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                    entityProperty, row.GetType().FullName));

        entityType = row.GetType().FullName;
        entityTable = row.Table;

        if (!editorAttr.IsMultiple)
        {
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

    private static UploadedFile[] ParseAndValidateJson(string json, string key)
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
            var colon = fieldName.IndexOf(':');
            if (colon >= 0)
                actualName = fieldName[..colon];

            var replaceField = (row.FindFieldByPropertyName(actualName) ??
                row.FindField(actualName)) ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute that " +
                    "references field '{2}', but no such field is found!'",
                        target.PropertyName ?? target.Name,
                        row.GetType().FullName,
                        actualName));
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

        ArgumentNullException.ThrowIfNull(formatSanitizer);

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
            var colon = key.IndexOf(':');
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
        var valueField = (StringField)Target;
        if (!handler.Row.IsAssigned(valueField))
            return;

        var oldValue = (handler.IsCreate ? null : valueField[handler.Old]).TrimToNull();
        var newValue = valueField[handler.Row] = valueField[handler.Row].TrimToNull();
        if (oldValue.IsTrimmedSame(newValue))
        {
            valueField[handler.Row] = oldValue;
            return;
        }

        var filesToDelete = new FilesToDelete(storage);
        handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] = filesToDelete;
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);

        if (editorAttr.IsMultiple)
        {
            var oldFileList = ParseAndValidateJson(oldValue, "oldFiles");
            var newFileList = ParseAndValidateJson(newValue, "newFiles");

            foreach (var file in oldFileList)
            {
                var filename = file.Filename.Trim();
                if (newFileList.Any(x => string.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                    continue;

                DeleteOldFile(storage, filesToDelete, filename,
                    copyToHistory: editorAttr is IUploadFileOptions { CopyToHistory: true });
            }

            if (newFileList.IsEmptyOrNull())
            {
                valueField[handler.Row] = null;
                return;
            }

            if (handler.IsUpdate)
                valueField[handler.Row] = CopyTemporaryFiles(handler, oldFileList, newFileList, filesToDelete);
        }
        else
        {
            DeleteOldFile(storage, filesToDelete, oldValue,
                copyToHistory: editorAttr is IUploadFileOptions { CopyToHistory: true });

            if (newValue == null)
            {
                if (string.IsNullOrWhiteSpace(oldValue))
                    return;

                valueField[handler.Row] = null;

                if (originalNameField is not null)
                    originalNameField[handler.Row] = null;
            }
            else
            {
                if (originalNameField is not null)
                    originalNameField[handler.Row] = storage.GetOriginalName(newValue).TrimToNull();

                if (handler.IsUpdate)
                    valueField[handler.Row] = CopyTemporaryFile(handler, filesToDelete, newValue).Path;
            }
        }
    }

    internal static void DeleteOldFile(IUploadStorage storage, FilesToDelete filesToDelete, 
        string oldFilename, bool copyToHistory)
    {
        if (string.IsNullOrEmpty(oldFilename))
            return;
            
        filesToDelete.RegisterOldFile(oldFilename);
        if (copyToHistory && storage.FileExists(oldFilename))
            storage.ArchiveFile(oldFilename);
    }

    /// <inheritdoc/>
    public override void OnAfterDelete(IDeleteRequestHandler handler)
    {
        if (ServiceQueryHelper.UseSoftDelete(handler.Row))
            return;

        var valueField = (StringField)Target;
        var oldValue = valueField[handler.Row].TrimToNull();
        if (string.IsNullOrEmpty(oldValue) || (editorAttr.IsMultiple && oldValue == "[]"))
            return;

        var filesToDelete = new FilesToDelete(storage);
        handler.UnitOfWork.RegisterFilesToDelete(filesToDelete);

        if (editorAttr.IsMultiple)
        {
            foreach (var file in ParseAndValidateJson(oldValue, "oldFiles"))
                DeleteOldFile(storage, filesToDelete, file.Filename,
                    editorAttr is IUploadFileOptions { CopyToHistory: true });
        }
        else
        {
            DeleteOldFile(storage, filesToDelete, oldValue,
                copyToHistory: editorAttr is IUploadFileOptions { CopyToHistory: true });
        }
    }

    private string CopyTemporaryFiles(ISaveRequestHandler handler,
        UploadedFile[] oldFileList, UploadedFile[] newFileList, FilesToDelete filesToDelete)
    {
        foreach (var file in newFileList)
        {
            var filename = file.Filename.Trim();
            if (oldFileList.Any(x => string.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                continue;

            file.Filename = CopyTemporaryFile(handler, filesToDelete, filename).Path;
        }

        return JSON.Stringify(newFileList, writeNulls: false);
    }

    private CopyTemporaryFileResult CopyTemporaryFile(ISaveRequestHandler handler, IFilesToDelete filesToDelete, 
        string temporaryFile)
    {
        if (string.IsNullOrEmpty(temporaryFile))
            throw new ArgumentNullException(nameof(temporaryFile));

        if (!temporaryFile.StartsWith("temporary/", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

        UploadPathHelper.CheckFileNameSecurity(temporaryFile);
        using var fs = storage.OpenFile(temporaryFile);
        var uploadInfo = uploadProcessor.Process(fs, temporaryFile, editorAttr as IUploadOptions);
        temporaryFile = uploadInfo.TemporaryFile;

        var idField = ((IIdRow)handler.Row).IdField;
        var originalName = storage.GetOriginalName(temporaryFile);
        if (string.IsNullOrEmpty(originalName))
            originalName = Path.GetFileName(temporaryFile);

        var entityId = idField.AsObject(handler.Row);

        var copyResult = storage.CopyTemporaryFile(new CopyTemporaryFileOptions
        {
            Format = fileNameFormat,
            PostFormat = s => ProcessReplaceFields(s, replaceFields, handler,
                editorAttr as IFilenameFormatSanitizer ?? formatSanitizer),
            TemporaryFile = temporaryFile,
            EntityId = entityId,
            FilesToDelete = filesToDelete,
            OriginalName = originalName 
        });

        storage.SetFileMetadata(copyResult.Path, new Dictionary<string, string>
        {
            { FileMetadataKeys.EntityTable, entityTable },
            { FileMetadataKeys.EntityType, entityType },
            { FileMetadataKeys.EntityField, entityField },
            { FileMetadataKeys.EntityProperty, entityProperty },
            { FileMetadataKeys.EntityId, Convert.ToString(entityId, CultureInfo.InvariantCulture) }
        }, overwriteAll: false);

        return copyResult;
    }

    /// <inheritdoc/>
    public override void OnAfterSave(ISaveRequestHandler handler)
    {
        if (handler.IsUpdate)
            return;

        var valueField = (StringField)Target;
        if (!handler.Row.IsAssigned(valueField))
            return;

        var newValue = valueField[handler.Row] = valueField[handler.Row].TrimToNull();
        if (string.IsNullOrEmpty(newValue) || (editorAttr.IsMultiple && newValue == "[]"))
            return;

        var filesToDelete = handler.StateBag[GetType().FullName + "_" + Target.Name + "_FilesToDelete"] as FilesToDelete;

        if (editorAttr.IsMultiple)
        {
            var newFileList = ParseAndValidateJson(newValue, "newFiles");
            valueField[handler.Row] = CopyTemporaryFiles(handler, [], newFileList, filesToDelete);
        }
        else
        {
            valueField[handler.Row] = CopyTemporaryFile(handler, filesToDelete, newValue).Path;
        }

        var idField = handler.Row.IdField;
        new SqlUpdate(handler.Row.Table)
            .Set(valueField, valueField[handler.Row])
            .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
            .Execute(handler.UnitOfWork.Connection);
    }
}


/// <summary>
/// Obsolete. Use <see cref="FileUploadBehavior"/> instead.
/// </summary>
[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class MultipleFileUploadBehavior() : FileUploadBehavior(null, null, null)
{
}

/// <summary>
/// Obsolete. Use <see cref="FileUploadBehavior"/> instead.
/// </summary>
[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class MultipleImageUploadBehavior() : MultipleFileUploadBehavior()
{
}