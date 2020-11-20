using Serenity.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Serenity.IO;
using System.Linq;
using Serenity.Abstractions;

namespace Serenity.Web
{
    public class UploadHelper
    {
        private UploadSettings settings;
        private string dbFileFormat;

        public UploadHelper(UploadSettings settings, string dbFileFormat)
        {
            this.dbFileFormat = dbFileFormat ?? throw new ArgumentNullException(nameof(dbFileFormat));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public static string DbFilePath(UploadSettings settings, string dbFileName)
        {
            return PathHelper.SecureCombine(GetRootPath(settings), ToPath(dbFileName));
        }

        public string FormatDbFileName(object entityId, string extension, string originalName = null)
        {
            return FormatDbFileName(dbFileFormat, entityId, extension, originalName);
        }

        public CopyTemporaryFileResult CopyTemporaryFile(string dbTemporaryFile, object entityId, FilesToDelete filesToDelete, Func<string, string> fileNameReplacer = null)
        {
            var result = CopyTemporaryFile(settings, dbTemporaryFile, entityId, fileNameReplacer);
            filesToDelete.Register(result);
            return result;
        }

        public CopyTemporaryFileResult CopyTemporaryFile(UploadSettings settings, string dbTemporaryFile, object entityId, Func<string, string> fileNameReplacer = null)
        {
            string temporaryFilePath = DbFilePath(settings, dbTemporaryFile);

            string originalName;
            using (var sr = new StreamReader(File.OpenRead(Path.ChangeExtension(temporaryFilePath, ".orig"))))
                originalName = sr.ReadLine();

            var extension = Path.GetExtension(dbTemporaryFile); 

            string dbFileName = ToUrl(FormatDbFileName(entityId, extension, originalName));
            if (fileNameReplacer != null)
                dbFileName = fileNameReplacer(dbFileName);

            string filePath = DbFilePath(settings, dbFileName);
            string basePath = null;
            int tries = 0;
            while (File.Exists(filePath) && ++tries < 10000)
            {
                if (basePath == null)
                    basePath = Path.ChangeExtension(filePath, null);
                
                filePath = basePath + " (" + tries + ")" + (extension ?? "");
            }

            if (tries > 0)
                dbFileName = Path.ChangeExtension(dbFileName, null) + " (" + tries + ")" + (extension ?? ""); 

            CopyFileAndRelated(temporaryFilePath, filePath);
            long size = new FileInfo(filePath).Length;
            bool hasThumbnail = File.Exists(GetThumbFileName(filePath));


            return new CopyTemporaryFileResult()
            {
                FileSize = size,
                DbFileName = dbFileName,
                OriginalName = originalName,
                FilePath = filePath,
                HasThumbnail = hasThumbnail,
                TemporaryFilePath = temporaryFilePath
            };
        }

        public static string CopyFileAndRelatedToHistory(UploadSettings settings, string sourceFilePath)
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string historyFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(sourceFilePath);

            CopyFileAndRelated(sourceFilePath, DbFilePath(settings, historyFile));

            return historyFile;
        }

        public static void CopyFileAndRelated(string sourceFilePath, string targetFilePath, bool overwrite = false)
        {
            var targetPath = Path.GetDirectoryName(targetFilePath);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            File.Copy(sourceFilePath, targetFilePath, overwrite);

            string sourcePath = Path.GetDirectoryName(sourceFilePath);
            string sourceBase = Path.GetFileNameWithoutExtension(sourceFilePath);
            string targetBase = Path.GetFileNameWithoutExtension(targetFilePath);

            foreach (var f in Directory.GetFiles(sourcePath,
                sourceBase + "_t*.jpg"))
            {
                string thumbSuffix = Path.GetFileName(f).Substring(sourceBase.Length);
                File.Copy(f, Path.Combine(targetPath, targetBase + thumbSuffix), overwrite);
            }
        }
       
        /// <summary>
        ///   Gets URL of an image file or its thumbnail under a subfolder of file upload root. 
        ///   If filename starts with "approval?", approval folder is used instead of 
        ///   the specified subfolder name. If file name is empty, then a null image
        ///   file URL is returned</summary>
        /// <param name="fileName">
        ///   File name.</param>
        /// <param name="thumbnailUrl">
        ///   True to return thumbnail URL.</param>
        /// <returns>
        ///   URL of the image file or its thumbnail, or null.</returns>       
        public static string ImageFileUrl(UploadSettings settings, string fileName, bool thumbnailUrl)
        {
            fileName = StringHelper.TrimToNull(fileName);

            if (fileName == null)
                return null;

            if (thumbnailUrl)
                fileName = GetThumbFileName(fileName);

            return settings.Url + ToUrl(fileName);
        }

        public static string FormatDbFileName(string format, object identity, string extension, string originalName = null)
        {
            long l;
            object groupKey;
            string s;
            if (identity == null)
                groupKey = "_";
            else if (identity is Guid)
            {
                s = ((Guid)identity).ToString("N");
                identity = s;
                groupKey = s.Substring(0, 2);
            }
            else
            {
                s = identity.ToString();
                if (long.TryParse(s, out l))
                    groupKey = l / 1000;
                else if (s.Length == 0)
                    groupKey = "_";
                else
                    groupKey = s.SafeSubstring(0, 2);
            }

            return string.Format(format, identity, groupKey, TemporaryFileHelper.RandomFileCode(), DateTime.Now, 
                Path.GetFileNameWithoutExtension(originalName)) + extension;
        }

        public static void DeleteFileAndRelated(UploadSettings settings, string dbFileName, DeleteType deleteType)
        {
            dbFileName = StringHelper.TrimToEmpty(dbFileName);
            if (dbFileName.Length > 0)
            {
                var fileName = PathHelper.SecureCombine(GetRootPath(settings), ToPath(dbFileName));

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
        }

        static readonly char[] invalidChars = Path.GetInvalidFileNameChars()
            .Where(x => x != '/' && x != '\\').ToArray();

        public static bool IsSecureRelativeFile(string fileName)
        {
            var trim = fileName.TrimToNull();

            return !(trim == null ||
                trim == "." ||
                trim == ".." ||
                fileName.IndexOf("../") >= 0 ||
                fileName.IndexOf("..\\") >= 0 ||
                fileName.IndexOf(':') >= 0 ||
                trim.StartsWith("/") ||
                trim.StartsWith("\\") ||
                trim.EndsWith("/") ||
                trim.EndsWith("\\") ||
                Path.IsPathRooted(fileName) ||
                fileName.IndexOfAny(invalidChars) >= 0 ||
                !Path.Combine("a/", fileName).StartsWith("a/"));
        }

        public static void CheckFileNameSecurity(string fileName)
        {
            if (!IsSecureRelativeFile(fileName))
#if !ASPNETMVC
                throw new ArgumentOutOfRangeException("fileName");
#else
                throw new HttpException(0x194, "Invalid_Request");
#endif
        }

        public static string GetThumbFileName(string fileName, string thumbSuffix = "_t.jpg")
        {
            if (fileName != null && fileName.Length > 0)
            {
                if (fileName.IndexOf('/') >= 0)
                    fileName = fileName.Replace('/', '\\');
                string path = Path.GetDirectoryName(fileName);
                return Path.Combine(path, Path.GetFileNameWithoutExtension(fileName) + thumbSuffix);
            }
            else
                return fileName;
        }

        public static string FileNameSizeDisplay(string name, int bytes)
        {
            return name + " (" + FileSizeDisplay(bytes) + ')';
        }

        public static string FileSizeDisplay(int bytes)
        {
            var byteSize = (Math.Round((Decimal)bytes * 100m / 1024m) * 0.01m);
            var suffix = "KB";
            if (byteSize > 1000)
            {
                byteSize = (Math.Round((Decimal)byteSize * 0.001m * 100m) * 0.01m);
                suffix = "MB";
            }
            var sizeParts = byteSize.ToString(Invariants.NumberFormat).Split('.');
            string value;
            if (sizeParts.Length > 1)
            {
                value = sizeParts[0] + "." + sizeParts[1].Substring(0, 2);
            }
            else
            {
                value = sizeParts[0];
            }
            return value + " " + suffix;
        }

        public static void RegisterFilesToDelete(IUnitOfWork unitOfWork, FilesToDelete filesToDelete, IExceptionLogger logger)
        {
            unitOfWork.OnCommit += delegate()
            {
                try
                {
                    filesToDelete.KeepNewFiles();
                    filesToDelete.Dispose();
                }
                catch (Exception ex)
                {
                    logger?.Log(ex);
                }
            };

            unitOfWork.OnRollback += delegate()
            {
                try
                {
                    filesToDelete.Dispose();
                }
                catch (Exception ex)
                {
                    logger?.Log(ex);
                }
            };
        }

        /// <summary>
        ///   (extension -> mime type) pairs for known mime types.</summary>
        private static Dictionary<string, string> KnownMimeTypes = new Dictionary<string, string>() 
        {
            { ".bmp", "image/bmp" },
            { ".css", "text/css" },
            { ".gif", "image/gif" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".jpe", "image/jpeg" },
            { ".js", "text/javascript" },
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".swf", "application/x-shockwave-flash" },          
            { ".tiff", "image/tiff" },
            { ".txt", "text/plain" }
        };

        /// <summary>
        ///   Gets MIME type for a given file using information in Win32 HKEY_CLASSES_ROOT 
        ///   registry key.</summary>
        /// <param name="fileName">
        ///   File name whose MIME type will be determined. Its only extension part will be used.</param>
        /// <returns>
        ///   Determined mime type for given file. "application/unknown" otherwise.</returns>
        public static string GetMimeType(string fileName)
        {
            string mimeType;
            string ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            if (KnownMimeTypes.TryGetValue(ext, out mimeType))
                return mimeType;
            else
                mimeType = "application/unknown";
            return mimeType;

        }
    }
}