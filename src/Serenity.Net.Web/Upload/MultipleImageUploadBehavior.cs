using Serenity.ComponentModel;
using Serenity.Data;
using Serenity.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Serenity.Services
{
    public class MultipleImageUploadBehavior : BaseSaveDeleteBehavior, IImplicitBehavior, IFieldBehavior
    {
        public Field Target { get; set; }

        private ImageUploadEditorAttribute attr;
        private string fileNameFormat;
        private const string SplittedFormat = "{1:00000}/{0:00000000}_{2}";
        private Dictionary<string, Field> replaceFields;
        private readonly ITextLocalizer localizer;
        private readonly IUploadStorage storage;

        public MultipleImageUploadBehavior(ITextLocalizer localizer, IUploadStorage storage)
        {
            this.localizer = localizer;
            this.storage = storage;
        }

        public bool ActivateFor(IRow row)
        {
            if (ReferenceEquals(null, Target))
                return false;

            attr = Target.GetAttribute<ImageUploadEditorAttribute>();
            if (attr == null || attr.DisableDefaultBehavior || attr.EditorType != "MultipleImageUpload")
                return false;

            if (!(Target is StringField))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but it is not a String field!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            if (!(row is IIdRow))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    "Field '{0}' on row type '{1}' has a UploadEditor attribute but Row type doesn't implement IIdRow!",
                        Target.PropertyName ?? Target.Name, row.GetType().FullName));

            var format = attr.FilenameFormat;

            if (format == null)
            {
                format = row.GetType().Name;
                if (format.EndsWith("Row", StringComparison.Ordinal))
                    format = format.Substring(0, format.Length - 3);
                format += "/~";
            }

            fileNameFormat = format.Replace("~", SplittedFormat, StringComparison.Ordinal);
            replaceFields = ImageUploadBehavior.ParseReplaceFields(fileNameFormat, row, Target);

            return true;
        }

        private static UploadedFile[] ParseAndValidate(string json, string key)
        {
            json = json.TrimToNull();

            if (json != null && (!json.StartsWith("[", StringComparison.Ordinal) ||
                !json.EndsWith("]", StringComparison.Ordinal)))
                throw new ArgumentOutOfRangeException(key);

            var list = JSON.Parse<UploadedFile[]>(json ?? "[]");

            if (list.Any(x => string.IsNullOrEmpty(x.Filename)) ||
                list.GroupBy(x => x.Filename.Trim()).SelectMany(x => x.Skip(1)).Any())
                throw new ArgumentOutOfRangeException(key);

            return list;
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

                ImageUploadBehavior.DeleteOldFile(storage, filesToDelete, filename, attr.CopyToHistory);
            }

            if (newFileList.IsEmptyOrNull())
            {
                field[handler.Row] = null;
                return;
            }

            if (handler.IsUpdate)
                field[handler.Row] = CopyTemporaryFiles(handler, oldFileList, newFileList, filesToDelete);
        }

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
                ImageUploadBehavior.DeleteOldFile(storage, filesToDelete, file.Filename, attr.CopyToHistory);
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

                ImageUploadBehavior.CheckUploadedImageAndCreateThumbs(attr, localizer, storage, ref filename);

                var idField = ((IIdRow)handler.Row).IdField;
                var copyResult = storage.CopyTemporaryFile(new CopyTemporaryFileOptions
                {
                    Format = fileNameFormat,
                    PostFormat = s => ImageUploadBehavior.ProcessReplaceFields(s, replaceFields, handler),
                    TemporaryFile = filename,
                    EntityId = idField.AsObject(handler.Row),
                    FilesToDelete = filesToDelete,
                    OriginalName = storage.GetOriginalName(filename)
                });

                file.Filename = copyResult.Path;
            }

            return JSON.Stringify(newFileList);
        }

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
            var copyResult = CopyTemporaryFiles(handler, Array.Empty<UploadedFile>(), newFileList, filesToDelete);
            var idField = handler.Row.IdField;

            new SqlUpdate(handler.Row.Table)
                .Set(field, copyResult)
                .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
                .Execute(handler.UnitOfWork.Connection);

            field[handler.Row] = copyResult;
        }
    }
}