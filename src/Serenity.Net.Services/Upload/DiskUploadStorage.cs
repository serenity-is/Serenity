using System.IO;
using Serenity.IO;

namespace Serenity.Web
{
    public class DiskUploadStorage : IUploadStorage
    {
        protected readonly IDiskUploadFileSystem FileSystem;
        public string RootPath { get; private set; }
        public string RootUrl { get; private set; }
        
        public DiskUploadStorage(DiskUploadStorageOptions options)
        {
            var opt = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(opt.RootPath))
                throw new ArgumentNullException(nameof(opt.RootPath));

            if (string.IsNullOrWhiteSpace(opt.RootUrl))
                throw new ArgumentNullException(nameof(opt.RootUrl));

            RootUrl = opt.RootUrl;
            RootPath = opt.RootPath;

            if (!Path.IsPathRooted(RootPath))
                RootPath = Path.Combine(AppContext.BaseDirectory, PathHelper.ToPath(RootPath));
        }

        public DiskUploadStorage(DiskUploadStorageOptions options,
            IDiskUploadFileSystem fileSystem) : this(options)
        {
            this.FileSystem = fileSystem ?? new PhysicalDiskUploadFileSystem();
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
            return path != null && Path.GetExtension(path)?.ToLowerInvariant()?.Trim() == ".meta";
        }

        public bool FileExists(string path)
        {
            return FileSystem.FileExists(FilePath(path)) && !IsInternalFile(path);
        }

        public virtual IDictionary<string, string> GetFileMetadata(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            var filePath = FilePath(path);
            var metaFile = filePath + ".meta";
            if (FileSystem.FileExists(metaFile))
            {
                var json = FileSystem.ReadAllText(metaFile);
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
                if (FileSystem.FileExists(metaFile))
                    FileSystem.TryDeleteOrMark(metaFile);
            }
            else if (metadata.Count == 0)
                return;
            
            if (!overwriteAll && FileSystem.FileExists(metaFile))
            {
                var existing = GetFileMetadata(path);
                foreach (var x in metadata)
                    existing[x.Key] = x.Value;
                metadata = existing;
            }

            FileSystem.WriteAllText(metaFile, JSON.StringifyIndented(metadata));
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
                var sourceBasePath = Path.ChangeExtension(sourcePath, null);
                var sourceBaseName = Path.GetFileNameWithoutExtension(sourceBasePath);
                var targetBasePath = Path.ChangeExtension(targetPath, null);

                var sourceDir = Path.GetDirectoryName(sourcePath);
                foreach (var f in store.GetFiles(sourceDir,
                     sourceBaseName + "_t*.jpg"))
                {
                    string thumbSuffix = Path.GetFileName(f)[sourceBaseName.Length..];
                    using var src = store.OpenFile(f);
                    newFiles.Add(WriteFile(targetBasePath + thumbSuffix, src, autoRename: null));
                }

                return targetPath;
            }
            catch
            {
                foreach (var newFile in newFiles)
                    FileSystem.TryDeleteOrMark(FilePath(newFile));
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
            string dbHistoryFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(path);
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

            var folder = Path.GetDirectoryName(fileName);
            FileSystem.TryDeleteMarkedFiles(folder);

            FileSystem.Delete(fileName, DeleteType.TryDeleteOrMark);

            string sourcePath = Path.GetDirectoryName(fileName);
            string sourceBase = Path.GetFileNameWithoutExtension(fileName);

            foreach (var f in FileSystem.DirectoryGetFiles(sourcePath,
                sourceBase + "_t*.jpg", SearchOption.AllDirectories))
            {
                FileSystem.Delete(f, DeleteType.TryDeleteOrMark);
            }

            FileSystem.Delete(fileName + ".meta", DeleteType.TryDeleteOrMark);
        }

        public long GetFileSize(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (IsInternalFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));

            return FileSystem.GetFileSize(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return FileSystem.DirectoryGetFiles(FilePath(path), searchPattern, SearchOption.AllDirectories)
                .Where(x => !IsInternalFile(x))
                .Select(x => Path.GetRelativePath(RootPath, x))
                .ToArray();
        }

        public System.IO.Stream OpenFile(string path)
        {
            return FileSystem.FileOpenRead(FilePath(path));
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

            var targetFile = FilePath(path);

            if (FileSystem.FileExists(targetFile))
            {
                if (autoRename == false)
                    throw new IOException($"Target file {path} exists in storage {GetType().FullName}");

                if (autoRename == true)
                    path = UploadPathHelper.FindAvailableName(path, FileExists);
            }

            var targetDir = Path.GetDirectoryName(targetFile);
            if (!FileSystem.DirectoryExists(targetDir))
                FileSystem.CreateDirectory(targetDir);

            FileSystem.CopyFile(source, targetFile, true);

            return path;
        }
    }
}