using System;
using System.Collections.Generic;
using Serenity.IO;

namespace Serenity.Web
{
    public class FilesToDelete : List<string>, IDisposable
    {
        private List<string> OldFiles;
        private readonly UploadSettings settings;

        public FilesToDelete(UploadSettings settings)
        {
            OldFiles = new List<string>();
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void Dispose()
        {
            foreach (var file in this)
                UploadHelper.DeleteFileAndRelated(settings, file, DeleteType.TryDeleteOrMark);

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