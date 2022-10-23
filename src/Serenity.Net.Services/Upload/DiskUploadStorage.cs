using Serenity.IO;

namespace Serenity.Web
{
    public class DiskUploadStorage : IUploadStorage
    {
        protected readonly IDiskUploadFileSystem fileSystem;

        public string RootPath { get; private set; }
        public string RootUrl { get; private set; }
        
        public DiskUploadStorage(DiskUploadStorageOptions options, IDiskUploadFileSystem fileSystem = null)
        {
            this.fileSystem = fileSystem ?? new PhysicalDiskUploadFileSystem();
            var opt = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(opt.RootPath))
                throw new ArgumentNullException(nameof(opt.RootPath));

            if (string.IsNullOrWhiteSpace(opt.RootUrl))
                throw new ArgumentNullException(nameof(opt.RootUrl));

            RootUrl = opt.RootUrl;
            RootPath = opt.RootPath;

            if (!this.fileSystem.IsPathRooted(RootPath))
                RootPath = this.fileSystem.Combine(AppContext.BaseDirectory, PathHelper.ToPath(RootPath));
        }

        protected string FilePath(string path)
        {
            return PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));
        }

        public string GetFileUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            return PathHelper.SecureCombine(RootUrl, PathHelper.ToUrl(path));
        }

        private bool IsInternalFile(string path)
        {
            return path != null && fileSystem.GetExtension(path)?.ToLowerInvariant()?.Trim() == ".meta";
        }

        public bool FileExists(string path)
        {
            return fileSystem.FileExists(FilePath(path)) && !IsInternalFile(path);
        }

        public virtual IDictionary<string, string> GetFileMetadata(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            var filePath = FilePath(path);
            var metaFile = filePath + ".meta";
            if (fileSystem.FileExists(metaFile))
            {
                var json = fileSystem.ReadAllText(metaFile);
                if (!string.IsNullOrEmpty(json) &&
                    json[0] == '{' &&
                    json[^1] == '}')
                {
                    return JSON.Parse<Dictionary<string, string>>(json);
                }
            }

            return new Dictionary<string, string>();
        }

        public virtual void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            var filePath = FilePath(path);
            var metaFile = filePath + ".meta";

            if (overwriteAll && metadata.Count == 0)
            {
                if (fileSystem.FileExists(metaFile))
                    fileSystem.TryDeleteOrMark(metaFile);
            }
            else if (metadata.Count == 0)
                return;
            
            if (!overwriteAll && fileSystem.FileExists(metaFile))
            {
                var existing = GetFileMetadata(path);
                foreach (var x in metadata)
                    existing[x.Key] = x.Value;
                metadata = existing;
            }

            fileSystem.WriteAllText(metaFile, JSON.StringifyIndented(metadata));
        }

        public string CopyFrom(IUploadStorage store, string sourcePath, string targetPath, bool? autoRename)
        {
            if (string.IsNullOrEmpty(sourcePath))
                throw new ArgumentNullException(sourcePath);

            if (IsInternalFile(sourcePath))
                throw new ArgumentOutOfRangeException(nameof(sourcePath));

            if (string.IsNullOrEmpty(targetPath))
                throw new ArgumentNullException(nameof(targetPath));

            if (IsInternalFile(targetPath))
                throw new ArgumentOutOfRangeException(nameof(targetPath));

            var newFiles = new List<string>();
            using var source = store.OpenFile(sourcePath);
            targetPath = WriteFile(targetPath, source, autoRename);
            newFiles.Add(targetPath);
            try
            {
                var sourceBasePath = fileSystem.ChangeExtension(sourcePath, null);
                var sourceBaseName = fileSystem.GetFileNameWithoutExtension(sourceBasePath);
                var targetBasePath = fileSystem.ChangeExtension(targetPath, null);

                var sourceDir = fileSystem.GetDirectoryName(sourcePath);
                foreach (var f in store.GetFiles(sourceDir,
                     sourceBaseName + "_t*.jpg"))
                {
                    string thumbSuffix = fileSystem.GetFileName(f)[sourceBaseName.Length..];
                    using var src = store.OpenFile(f);
                    newFiles.Add(WriteFile(targetBasePath + thumbSuffix, src, autoRename: null));
                }

                return targetPath;
            }
            catch
            {
                foreach (var newFile in newFiles)
                    fileSystem.TryDeleteOrMark(FilePath(newFile));
                throw;
            }
        }

        public string ArchiveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string dbHistoryFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + fileSystem.GetExtension(path);
            CopyFrom(this, path, dbHistoryFile, autoRename: false);
            return dbHistoryFile;
        }

        public void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            var fileName = PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));

            var folder = fileSystem.GetDirectoryName(fileName);
            fileSystem.TryDeleteMarkedFiles(folder);

            fileSystem.Delete(fileName, DeleteType.TryDeleteOrMark);

            string sourcePath = fileSystem.GetDirectoryName(fileName);
            string sourceBase = fileSystem.GetFileNameWithoutExtension(fileName);

            foreach (var f in fileSystem.GetFiles(sourcePath,
                sourceBase + "_t*.jpg", recursive: true))
            {
                fileSystem.Delete(f, DeleteType.TryDeleteOrMark);
            }

            fileSystem.Delete(fileName + ".meta", DeleteType.TryDeleteOrMark);
        }

        public long GetFileSize(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            return fileSystem.GetFileSize(FilePath(path));
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return fileSystem.GetFiles(FilePath(path), searchPattern, recursive: true)
                .Where(x => !IsInternalFile(x))
                .Select(x => fileSystem.GetRelativePath(RootPath, x))
                .ToArray();
        }

        public System.IO.Stream OpenFile(string path)
        {
            return fileSystem.OpenRead(FilePath(path));
        }

        public virtual void PurgeTemporaryFiles()
        {
        }

        public string WriteFile(string path, System.IO.Stream source, bool? autoRename)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            var targetFullPath = FilePath(path);

            if (fileSystem.FileExists(targetFullPath))
            {
                if (autoRename == false)
                    throw new System.IO.IOException($"Target file {path} exists in storage {GetType().FullName}");

                if (autoRename == true)
                {
                    path = UploadPathHelper.FindAvailableName(path, FileExists);
                    targetFullPath = FilePath(path);
                }
            }

            var targetDir = fileSystem.GetDirectoryName(targetFullPath);
            if (!fileSystem.DirectoryExists(targetDir))
                fileSystem.CreateDirectory(targetDir);

            using var targetStream = fileSystem.CreateFile(targetFullPath, overwrite: autoRename == null);
            source.CopyTo(targetStream);

            return path;
        }
    }
}