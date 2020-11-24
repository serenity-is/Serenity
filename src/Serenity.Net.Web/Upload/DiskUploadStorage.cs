using Serenity.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace Serenity.Web
{
    public class DiskUploadStorage : IUploadStorage
    {
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

        public string FilePath(string path)
        {
            return PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));
        }

        public string GetFileUrl(string path)
        {
            return PathHelper.SecureCombine(RootUrl, PathHelper.ToUrl(path));
        }

        public bool FileExists(string path)
        {
            return File.Exists(FilePath(path));
        }

        public virtual string GetOriginalName(string path)
        {
            var temporaryFilePath = FilePath(path);
            var origFile = Path.ChangeExtension(temporaryFilePath, ".orig");
            if (File.Exists(origFile))
                using (var sr = new StreamReader(File.OpenRead(origFile)))
                    return sr.ReadLine();

            return Path.GetFileName(path);
        }

        public string CopyFrom(IUploadStorage store, string sourcePath, string targetPath, bool autoRename)
        {
            var newFiles = new List<string>();
            using var source = store.OpenFile(sourcePath);
            targetPath = WriteFile(targetPath, source, autoRename);
            newFiles.Add(targetPath);
            try
            {
                var sourceBase = Path.ChangeExtension(sourcePath, null);
                var targetBase = Path.ChangeExtension(targetPath, null);

                var sourceDir = Path.GetDirectoryName(sourcePath);
                foreach (var f in store.GetFiles(sourceDir, sourceBase + "_t*.jpg"))
                {
                    string thumbSuffix = Path.GetFileName(f).Substring(sourceBase.Length);
                    using var src = store.OpenFile(f);
                    newFiles.Add(WriteFile(targetBase + thumbSuffix, src, false));
                }

                return targetPath;
            }
            catch
            {
                foreach (var newFile in newFiles)
                    TemporaryFileHelper.TryDeleteOrMark(FilePath(newFile));
                throw;
            }
        }

        public string ArchiveFile(string path)
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string dbHistoryFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(path);
            CopyFrom(this, path, dbHistoryFile, false);
            return dbHistoryFile;
        }

        public void DeleteFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var fileName = PathHelper.SecureCombine(RootPath, PathHelper.ToPath(path));

            var folder = Path.GetDirectoryName(fileName);
            TemporaryFileHelper.TryDeleteMarkedFiles(folder);

            TemporaryFileHelper.Delete(fileName, DeleteType.TryDeleteOrMark);

            string sourcePath = Path.GetDirectoryName(fileName);
            string sourceBase = Path.GetFileNameWithoutExtension(fileName);

            foreach (var f in Directory.GetFiles(sourcePath,
                sourceBase + "_t*.jpg"))
            {
                TemporaryFileHelper.Delete(f, DeleteType.TryDeleteOrMark);
            }

            TemporaryFileHelper.Delete(fileName + ".meta", DeleteType.TryDeleteOrMark);
            TemporaryFileHelper.Delete(fileName + ".orig", DeleteType.TryDeleteOrMark);
        }

        public long GetFileSize(string path)
        {
            return new FileInfo(FilePath(path)).Length;
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(FilePath(path), searchPattern);
        }

        public Stream OpenFile(string path)
        {
            return File.OpenRead(FilePath(path));
        }

        public virtual void PurgeTemporaryFiles()
        {
        }

        public string WriteFile(string path, Stream source, bool autoRename)
        {
            var targetFile = FilePath(path);

            if (File.Exists(targetFile))
            {
                if (!autoRename)
                    throw new IOException($"Target file {path} exists in storage {GetType().FullName}");

                path = UploadPathHelper.FindAvailableName(path, FileExists);
            }

            var targetDir = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            using var target = File.Create(targetFile);
            source.CopyTo(target);

            return path;
        }
    }
}