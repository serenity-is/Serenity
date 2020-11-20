using Microsoft.Extensions.Options;
using Serenity.IO;
using System;
using System.IO;

namespace Serenity.Web
{
    public class DiskUploadStorage : IUploadStorage
    {
        public string RootPath { get; private set; }
        public string RootUrl { get; private set; }
        public string TempPath { get; private set; }

        public DiskUploadStorage(IOptions<DiskUploadStorageOptions> options)
        {
            var opt = (options ?? throw new ArgumentNullException(nameof(options))).Value;

            if (string.IsNullOrWhiteSpace(opt.RootPath))
                throw new ArgumentNullException(nameof(opt.RootPath));

            if (string.IsNullOrWhiteSpace(opt.RootUrl))
                throw new ArgumentNullException(nameof(opt.RootUrl));

            RootUrl = opt.RootUrl;

            RootPath = opt.RootPath;
            if (!Path.IsPathRooted(RootPath))
                RootPath = Path.Combine(AppContext.BaseDirectory, PathHelper.ToPath(RootPath));

            var tempPath = string.IsNullOrEmpty(opt.TempPath) ? Path.Combine(RootPath, "temporary") :
                opt.TempPath;

            if (string.IsNullOrWhiteSpace(tempPath))
                throw new ArgumentNullException(nameof(opt.TempPath));

            tempPath = PathHelper.ToPath(tempPath);

            try
            {
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                    File.WriteAllText(Path.Combine(tempPath, ".temporary"), "");
                }
            }
            catch (Exception)
            {
            }
        }

        public string DbFilePath(string dbFileName)
        {
            return PathHelper.SecureCombine(RootPath, PathHelper.ToPath(dbFileName));
        }

        public string DbFileUrl(string dbFileName)
        {
            return PathHelper.SecureCombine(RootUrl, PathHelper.ToUrl(dbFileName));
        }

        public bool FileExists(string dbFileName)
        {
            return File.Exists(DbFilePath(dbFileName));
        }

        public string GetOriginalName(string dbTemporaryFile)
        {
            var temporaryFilePath = DbFilePath(dbTemporaryFile);
            var origFile = Path.ChangeExtension(temporaryFilePath, ".orig");
            using (var sr = new StreamReader(File.OpenRead(origFile)))
                return sr.ReadLine();
        }

        public void CopyFileAndRelated(string dbSourceFile, string dbTargetFile, bool overwrite)
        {
            var sourceFile = DbFilePath(dbSourceFile);
            var targetFile = DbFilePath(dbTargetFile);
            var sourcePath = Path.GetDirectoryName(sourceFile);
            var targetPath = Path.GetDirectoryName(targetFile);
            
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            File.Copy(sourceFile, targetFile, overwrite);

            var sourceBase = Path.GetFileNameWithoutExtension(sourceFile);
            var targetBase = Path.GetFileNameWithoutExtension(targetFile);

            foreach (var f in Directory.GetFiles(sourcePath,
                sourceBase + "_t*.jpg"))
            {
                string thumbSuffix = Path.GetFileName(f).Substring(sourceBase.Length);
                File.Copy(f, Path.Combine(targetPath, targetBase + thumbSuffix), overwrite);
            }
        }

        public string ArchiveFileAndRelated(string dbSourceFile)
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string dbHistoryFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(dbSourceFile);
            CopyFileAndRelated(dbSourceFile, dbHistoryFile, false);
            return dbHistoryFile;
        }

        public void DeleteFileAndRelated(string dbFileName, DeleteType deleteType)
        {
            if (string.IsNullOrEmpty(dbFileName))
                return;

            var fileName = Path.Combine(RootPath, PathHelper.ToPath(dbFileName));

            if (deleteType == DeleteType.TryDeleteOrMark)
            {
                var folder = Path.GetDirectoryName(fileName);
                TemporaryFileHelper.TryDeleteMarkedFiles(folder);
            }

            TemporaryFileHelper.Delete(fileName, deleteType);

            string sourcePath = Path.GetDirectoryName(fileName);
            string sourceBase = Path.GetFileNameWithoutExtension(fileName);

            foreach (var f in Directory.GetFiles(sourcePath,
                sourceBase + "_t*.jpg"))
            {
                TemporaryFileHelper.Delete(f, deleteType);
            }

            TemporaryFileHelper.Delete(fileName + ".meta", deleteType);
            TemporaryFileHelper.Delete(fileName + ".orig", deleteType);
        }

        public long GetFileSize(string dbFileName)
        {
            return new FileInfo(DbFilePath(dbFileName)).Length;
        }
    }
}