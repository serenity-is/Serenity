﻿using Serenity.Data;
using Serenity.Web;
using System;
using System.IO;
using System.Linq;

namespace Serenity.Services
{
    [Obsolete("Use MultiImage/FileUploadEditorAttribute in row image/file field and remove this attribute from your handler")]
    public class EmbeddedMultiFileUploadBehaviourAttribute : SaveRequestBehaviorAttribute
    {
        private string filesField;
        private bool copyFilesToHistory;
        private string subFolder;
        private string fileNameFormat;
        private bool storeSubFolderInDB;

        public EmbeddedMultiFileUploadBehaviourAttribute(
            string filesField,
            string fileNameFormat = "{1:00000}/{0:00000000}_{2}",
            bool copyFilesToHistory = false,
            string subFolder = null,
            bool storeSubFolderInDB = false)
        {
            if (filesField == null)
                throw new ArgumentNullException("fileNameField");

            this.filesField = filesField;
            this.fileNameFormat = fileNameFormat;
            this.copyFilesToHistory = copyFilesToHistory;
            this.subFolder = subFolder.TrimToNull();
            this.storeSubFolderInDB = storeSubFolderInDB;
        }

        private UploadedFile[] ParseAndValidate(string json, string key)
        {
            json = json.TrimToNull();

            if (json != null && (!json.StartsWith("[") || !json.EndsWith("]")))
                throw new ArgumentOutOfRangeException(key);

            var list = JSON.Parse<UploadedFile[]>(json ?? "[]");

            if (list.Any(x => string.IsNullOrEmpty(x.Filename)) ||
                list.GroupBy(x => x.Filename.Trim()).SelectMany(x => x.Skip(1)).Any())
                throw new ArgumentOutOfRangeException(key);

            return list;
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            var field = (StringField)(handler.Row.FindField(this.filesField) ?? handler.Row.FindFieldByPropertyName(filesField));

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

            var filesToDelete = new FilesToDelete();
            UploadHelper.RegisterFilesToDelete(handler.UnitOfWork, filesToDelete);
            handler.StateBag[this.GetType().FullName + "_FilesToDelete"] = filesToDelete;

            foreach (var file in oldFileList)
            {
                var filename = file.Filename.Trim();
                if (newFileList.Any(x => String.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                    continue;

                var actualOldFile = ((subFolder != null && !storeSubFolderInDB) ? (subFolder + "/") : "") + filename;
                filesToDelete.RegisterOldFile(actualOldFile);

                if (copyFilesToHistory)
                {
                    var oldFilePath = UploadHelper.ToPath(actualOldFile);
                    string date = DateTime.UtcNow.ToString("yyyyMM", Invariants.DateTimeFormat);
                    string historyFile = "history/" + date + "/" + Path.GetFileName(oldFilePath);
                    if (File.Exists(UploadHelper.DbFilePath(oldFilePath)))
                        UploadHelper.CopyFileAndRelated(UploadHelper.DbFilePath(oldFilePath), UploadHelper.DbFilePath(historyFile), overwrite: true);
                }
            }

            if (newFileList.IsEmptyOrNull())
            {
                field[handler.Row] = null;
                return;
            }

            if (handler.IsUpdate)
                field[handler.Row] = CopyTemporaryFiles(handler, oldFileList, newFileList, filesToDelete);
        }

        private string CopyTemporaryFiles(ISaveRequestHandler handler, 
            UploadedFile[] oldFileList, UploadedFile[] newFileList, FilesToDelete filesToDelete)
        {
            foreach (var file in newFileList)
            {
                var filename = file.Filename.Trim();
                if (oldFileList.Any(x => String.Compare(x.Filename.Trim(), filename, StringComparison.OrdinalIgnoreCase) == 0))
                    continue;

                if (!filename.ToLowerInvariant().StartsWith("temporary/"))
                    throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

                var uploadHelper = new UploadHelper((subFolder.IsEmptyOrNull() ? "" : (subFolder + "/")) + fileNameFormat);
                var idField = (Field)(((IIdRow)handler.Row).IdField);
                var copyResult = uploadHelper.CopyTemporaryFile(filename, idField.AsObject(handler.Row), filesToDelete);
                if (subFolder != null && !this.storeSubFolderInDB)
                    copyResult.DbFileName = copyResult.DbFileName.Substring(subFolder.Length + 1);

                file.Filename = copyResult.DbFileName;
            }

            return JSON.Stringify(newFileList);
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            if (handler.IsUpdate)
                return;

            var field = (StringField)(handler.Row.FindField(this.filesField) ?? handler.Row.FindFieldByPropertyName(filesField));
            if (!handler.Row.IsAssigned(field))
                return;

            var newFilesJSON = field[handler.Row] = field[handler.Row].TrimToNull();
            var newFileList = ParseAndValidate(newFilesJSON, "newFiles");

            if (newFileList.IsEmptyOrNull())
                return;

            var filesToDelete = handler.StateBag[this.GetType().FullName + "_FilesToDelete"] as FilesToDelete;
            var copyResult = CopyTemporaryFiles(handler, new UploadedFile[0], newFileList, filesToDelete);
            var idField = (Field)(((IIdRow)handler.Row).IdField);

            new SqlUpdate(handler.Row.Table)
                .Set(field, copyResult)
                .Where(idField == new ValueCriteria(idField.AsObject(handler.Row)))
                .Execute(handler.UnitOfWork.Connection);
        }
    }
}