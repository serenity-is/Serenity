using System;
using System.Collections.Generic;
using Serenity.IO;

namespace Serenity.Web
{
    public class FilesToDelete : List<string>, IDisposable
    {
        private List<string> OldFiles;

        public FilesToDelete()
        {
            OldFiles = new List<string>();
        }

        public void Dispose()
        {
            foreach (var file in this)
                UploadHelper.DeleteFileAndRelated(file, DeleteType.TryDeleteOrMark);

            this.Clear();
        }

        public void Register(CopyTemporaryFileResult result)
        {
            RegisterNewFile(result.FilePath);
            RegisterOldFile(result.TemporaryFilePath);
        }

        public void RegisterNewFile(string file)
        {
            Add(file);
        }

        public void RegisterOldFile(string file)
        {
            OldFiles.Add(file);
        }

        public void KeepNewFiles()
        {
            this.Clear();
            this.AddRange(OldFiles);
            this.Dispose();
        }
    }

}