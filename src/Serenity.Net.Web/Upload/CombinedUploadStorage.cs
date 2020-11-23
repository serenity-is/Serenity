using System;
using System.IO;

namespace Serenity.Web
{
    public class CombinedUploadStorage : IUploadStorage
    {
        private readonly IUploadStorage mainStorage;
        private readonly IUploadStorage subStorage;
        private readonly string subPrefix;

        public CombinedUploadStorage(IUploadStorage mainStorage, IUploadStorage subStorage, string subPrefix)
        {
            this.mainStorage = mainStorage ?? throw new ArgumentNullException(nameof(mainStorage));
            this.subStorage = subStorage ?? throw new ArgumentNullException(nameof(subStorage));
            this.subPrefix = subPrefix ?? throw new ArgumentNullException(nameof(subPrefix));
        }

        public bool IsSubPath(string path)
        {
            return path != null && PathHelper.ToUrl(path).StartsWith(subPrefix);
        }

        public string ArchiveFile(string path)
        {
            if (IsSubPath(path))
                subStorage.ArchiveFile(path.Substring(subPrefix.Length));
            
            return mainStorage.ArchiveFile(path);
        }

        public void CopyFile(IUploadStorage source, string path, string targetPath, bool overwrite)
        {
            if (IsSubPath(targetPath))
                subStorage.CopyFile(source, path, targetPath.Substring(subPrefix.Length), overwrite);
            else
                mainStorage.CopyFile(source, path, targetPath, overwrite);
        }

        public void DeleteFile(string path)
        {
            if (IsSubPath(path))
                subStorage.DeleteFile(path.Substring(subPrefix.Length));
            else
                mainStorage.DeleteFile(path);
        }

        public bool FileExists(string path)
        {
            if (IsSubPath(path))
                return subStorage.FileExists(path.Substring(subPrefix.Length));
                
            return mainStorage.FileExists(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            if (IsSubPath(path))
                return subStorage.GetFiles(path.Substring(subPrefix.Length), searchPattern);
                
            return mainStorage.GetFiles(path, searchPattern);
        }

        public long GetFileSize(string path)
        {
            if (IsSubPath(path))
                return subStorage.GetFileSize(path.Substring(subPrefix.Length));
                
            return mainStorage.GetFileSize(path);
        }

        public string GetFileUrl(string path)
        {
            if (IsSubPath(path))
                return subStorage.GetFileUrl(path.Substring(subPrefix.Length));
                
            return mainStorage.GetFileUrl(path);
        }

        public string GetOriginalName(string path)
        {
            if (IsSubPath(path))
                return subStorage.GetOriginalName(path.Substring(subPrefix.Length));

            return mainStorage.GetOriginalName(path);
        }

        public Stream OpenFile(string path)
        {
            if (IsSubPath(path))
                return subStorage.OpenFile(path.Substring(subPrefix.Length));
            
            return mainStorage.OpenFile(path);
        }

        public void PurgeTemporaryFiles()
        {
            mainStorage.PurgeTemporaryFiles();
            subStorage.PurgeTemporaryFiles();
        }

        public void WriteFile(string path, Stream source, bool overwrite)
        {
            if (IsSubPath(path))
                subStorage.WriteFile(path, source, overwrite);
            else
                mainStorage.WriteFile(path, source, overwrite);
        }
    }
}