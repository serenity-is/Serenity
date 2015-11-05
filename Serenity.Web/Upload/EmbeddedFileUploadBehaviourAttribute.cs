﻿using Serenity.Data;
using Serenity.Web;
using System;
using System.IO;

namespace Serenity.Services
{
    public class EmbeddedFileUploadBehaviourAttribute : SaveRequestBehaviorAttribute
    {
        private string fileNameField;
        private bool copyFileToHistory;
        private string subFolder;
        private string fileNameFormat;
        private bool storeSubFolderInDB;

        public EmbeddedFileUploadBehaviourAttribute(
            string fileNameField,
            string fileNameFormat = "{1:00000}/{0:00000000}_{2}",
            bool copyFileToHistory = false,
            string subFolder = null,
            bool storeSubFolderInDB = false)
        {
            if (fileNameField == null)
                throw new ArgumentNullException("fileNameField");

            this.fileNameField = fileNameField;
            this.fileNameFormat = fileNameFormat;
            this.copyFileToHistory = copyFileToHistory;
            this.subFolder = subFolder.TrimToNull();
            this.storeSubFolderInDB = storeSubFolderInDB;
        }

        public override void OnBeforeSave(ISaveRequestHandler handler)
        {
            var filesToDelete = new FilesToDelete();
            UploadHelper.RegisterFilesToDelete(handler.UnitOfWork, filesToDelete);
            handler.StateBag[this.GetType().FullName + "_FilesToDelete"] = filesToDelete;

            var filename = (StringField)(handler.Row.FindField(this.fileNameField) ?? handler.Row.FindFieldByPropertyName(fileNameField));
            var oldFilename = handler.Old == null ? null : filename[handler.Old];
            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();

            if (oldFilename.IsTrimmedSame(newFilename))
            {
                filename[handler.Row] = oldFilename;
                return;
            }

            if (!oldFilename.IsEmptyOrNull())
            {
                var actualOldFile = ((subFolder != null && !storeSubFolderInDB) ? (subFolder + "/") : "") + oldFilename;
                filesToDelete.RegisterOldFile(actualOldFile);

                if (copyFileToHistory)
                {
                    var oldFilePath = UploadHelper.ToPath(actualOldFile);
                    string date = DateTime.UtcNow.ToString("yyyyMM", Invariants.DateTimeFormat);
                    string historyFile = "history/" + date + "/" + Path.GetFileName(oldFilePath);
                    if (File.Exists(UploadHelper.DbFilePath(oldFilePath)))
                        UploadHelper.CopyFileAndRelated(UploadHelper.DbFilePath(oldFilePath), UploadHelper.DbFilePath(historyFile), overwrite: true);
                }
            }


            if (newFilename == null)
            {
                if (oldFilename.IsTrimmedEmpty())
                    return;

                filename[handler.Row] = null;
                return;
            }

            if (!newFilename.ToLowerInvariant().StartsWith("temporary/"))
                throw new InvalidOperationException("For security reasons, only temporary files can be used in uploads!");

            if (handler.Old != null)
            {
                var copyResult = CopyTemporaryFile(handler, filesToDelete);
                filename[handler.Row] = copyResult.DbFileName;
            }
        }

        private CopyTemporaryFileResult CopyTemporaryFile(ISaveRequestHandler handler, FilesToDelete filesToDelete)
        {
            var filename = (StringField)(handler.Row.FindField(this.fileNameField) ?? handler.Row.FindFieldByPropertyName(fileNameField));
            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();
            var uploadHelper = new UploadHelper((subFolder.IsEmptyOrNull() ? "" : (subFolder + "/")) + fileNameFormat);
            var copyResult = uploadHelper.CopyTemporaryFile(newFilename, ((IIdRow)handler.Row).IdField[handler.Row].Value, filesToDelete);
            if (subFolder != null && !this.storeSubFolderInDB)
                copyResult.DbFileName = copyResult.DbFileName.Substring(subFolder.Length + 1);
            return copyResult;
        }

        public override void OnAfterSave(ISaveRequestHandler handler)
        {
            var filename = (StringField)(handler.Row.FindField(this.fileNameField) ?? handler.Row.FindFieldByPropertyName(fileNameField));

            if (handler.Old != null)
                return;

            var newFilename = filename[handler.Row] = filename[handler.Row].TrimToNull();
            if (newFilename == null)
                return;

            var filesToDelete = handler.StateBag[this.GetType().FullName + "_FilesToDelete"] as FilesToDelete;
            var copyResult = CopyTemporaryFile(handler, filesToDelete);

            new SqlUpdate(handler.Row.Table)
                .Set(filename, copyResult.DbFileName)
                .WhereEqual((Field)((IIdRow)handler.Row).IdField, ((IIdRow)handler.Row).IdField[handler.Row].Value)
                .Execute(handler.UnitOfWork.Connection);
        }
    }
}