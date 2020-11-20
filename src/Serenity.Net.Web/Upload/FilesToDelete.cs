using System;
using System.Collections.Generic;
using Serenity.IO;

namespace Serenity.Web
{
    public class FilesToDelete : List<string>, IDisposable, IFilesToDelete
    {
        private readonly IUploadStorage storage;
        private List<string> OldFiles;

        public FilesToDelete(IUploadStorage storage)
        {
            OldFiles = new List<string>();
            this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public void Dispose()
        {
            foreach (var file in this)
                storage.DeleteFileAndRelated(file, DeleteType.TryDeleteOrMark);

            Clear();
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
            Clear();
            AddRange(OldFiles);
            Dispose();
        }
    }
}