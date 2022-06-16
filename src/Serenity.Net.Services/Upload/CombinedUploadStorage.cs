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
            this.subPrefix = PathHelper.ToUrl(subPrefix ?? throw new ArgumentNullException(nameof(subPrefix)));

            if (!this.subPrefix.EndsWith("/"))
                this.subPrefix += "/";
        }

        public bool UnderSubPath(string path)
        {
            return path != null && PathHelper.ToUrl(path).StartsWith(subPrefix, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSubPath(string path)
        {
            return path != null && path.Length == subPrefix.Length - 1 &&
                subPrefix.StartsWith(path, StringComparison.OrdinalIgnoreCase);
        }

        public string ArchiveFile(string path)
        {
            if (UnderSubPath(path))
                return subPrefix + subStorage.ArchiveFile(path[subPrefix.Length..]);
            
            return mainStorage.ArchiveFile(path);
        }

        public string CopyFrom(IUploadStorage source, string path, string targetPath, bool? autoRename)
        {
            if (UnderSubPath(targetPath))
                return subPrefix + subStorage.CopyFrom(source, path, targetPath[subPrefix.Length..], autoRename);
                
            return mainStorage.CopyFrom(source, path, targetPath, autoRename);
        }

        public void DeleteFile(string path)
        {
            if (UnderSubPath(path))
                subStorage.DeleteFile(path[subPrefix.Length..]);
            else
                mainStorage.DeleteFile(path);
        }

        public bool FileExists(string path)
        {
            if (UnderSubPath(path))
                return subStorage.FileExists(path[subPrefix.Length..]);
                
            return mainStorage.FileExists(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            if (UnderSubPath(path))
                return subStorage.GetFiles(path[subPrefix.Length..], searchPattern)
                    .Select(x => subPrefix + x).ToArray();
            else if (IsSubPath(path))
                return subStorage.GetFiles("", searchPattern)
                    .Select(x => subPrefix + x).ToArray();
                
            return mainStorage.GetFiles(path, searchPattern);
        }

        public long GetFileSize(string path)
        {
            if (UnderSubPath(path))
                return subStorage.GetFileSize(path[subPrefix.Length..]);
                
            return mainStorage.GetFileSize(path);
        }

        public string GetFileUrl(string path)
        {
            if (UnderSubPath(path))
                return subStorage.GetFileUrl(path[subPrefix.Length..]);
                
            return mainStorage.GetFileUrl(path);
        }

        public Stream OpenFile(string path)
        {
            if (UnderSubPath(path))
                return subStorage.OpenFile(path[subPrefix.Length..]);
            
            return mainStorage.OpenFile(path);
        }

        public void PurgeTemporaryFiles()
        {
            mainStorage.PurgeTemporaryFiles();
            subStorage.PurgeTemporaryFiles();
        }

        public string WriteFile(string path, Stream source, bool? autoRename)
        {
            if (UnderSubPath(path))
                return subPrefix + subStorage.WriteFile(path[subPrefix.Length..], source, autoRename);
                
            return mainStorage.WriteFile(path, source, autoRename);
        }

        public IDictionary<string, string> GetFileMetadata(string path)
        {
            if (UnderSubPath(path))
                return subStorage.GetFileMetadata(path[subPrefix.Length..]);
                
            return mainStorage.GetFileMetadata(path);
        }

        public void SetFileMetadata(string path, IDictionary<string, string> metadata, bool overwriteAll)
        {
            if (UnderSubPath(path))
               subStorage.SetFileMetadata(path[subPrefix.Length..], metadata, overwriteAll);
            else
               mainStorage.SetFileMetadata(path, metadata, overwriteAll);
        }
    }
}