using Serenity.ComponentModel;
using Serenity.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Serenity.IO;
using System.Web.Hosting;
#if !ASPNETCORE
using System.Web;
#endif

namespace Serenity.Web
{
    public class CopyTemporaryFileResult
    {
        public string TemporaryFilePath { get; set; }
        public string DbFileName { get; set; }
        public string OriginalName { get; set; }
        public string FilePath { get; set; }
        public bool HasThumbnail { get; set; }
        public long FileSize { get; set; }
    }

    [SettingScope("Application"), SettingKey("UploadSettings")]
    public class UploadSettings
    {
        public string Url { get; set; }
        public string Path { get; set; }
    }

    public class UploadHelper
    {

        private static UploadSettings settings;

        public static UploadSettings Settings
        {
            get
            {
                if (settings == null)
                    settings = Config.Get<UploadSettings>();

                return settings;
            }
        }

        private string dbFileFormat;

        public UploadHelper(string dbFileFormat)
        {
            this.dbFileFormat = dbFileFormat;
        }

        public static string DbFilePath(string dbFileName)
        {
            return Path.Combine(RootPath, ToPath(dbFileName));
        }

        public string FormatDbFileName(object entityId, string extension, string originalName = null)
        {
            return FormatDbFileName(dbFileFormat, entityId, extension, originalName);
        }

        public CopyTemporaryFileResult CopyTemporaryFile(string dbTemporaryFile, object entityId, FilesToDelete filesToDelete, Func<string, string> fileNameReplacer = null)
        {
            var result = CopyTemporaryFile(dbTemporaryFile, entityId, fileNameReplacer);
            filesToDelete.Register(result);
            return result;
        }

        public CopyTemporaryFileResult CopyTemporaryFile(string dbTemporaryFile, object entityId, Func<string, string> fileNameReplacer = null)
        {
            string temporaryFilePath = DbFilePath(dbTemporaryFile);

            string originalName;
            using (var sr = new StreamReader(File.OpenRead(Path.ChangeExtension(temporaryFilePath, ".orig"))))
                originalName = sr.ReadLine();

            var extension = Path.GetExtension(dbTemporaryFile); 

            string dbFileName = ToUrl(FormatDbFileName(entityId, extension, originalName));
            if (fileNameReplacer != null)
                dbFileName = fileNameReplacer(dbFileName);

            string filePath = DbFilePath(dbFileName);
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

            UploadHelper.CopyFileAndRelated(temporaryFilePath, filePath);
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

        public static string CopyFileAndRelatedToHistory(string sourceFilePath)
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string historyFile = "history/" + date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(sourceFilePath);

            CopyFileAndRelated(sourceFilePath, DbFilePath(historyFile));

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
        ///   Converts backslashes to forward slashes</summary>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <returns>
        ///   Converted filename.</returns>
        public static string ToUrl(string fileName)
        {
            if (fileName != null && fileName.IndexOf('\\') >= 0)
                return fileName.Replace('\\', '/');
            else
                return fileName;
        }

        /// <summary>
        ///   Converts forward slashes to backslashes</summary>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <returns>
        ///   Converted filename.</returns>
        public static string ToPath(string fileName)
        {
            var separator = Path.DirectorySeparatorChar;
            var opposite = separator == '/' ? '\\' : '/';
            if (fileName != null && fileName.IndexOf(opposite) >= 0)
                return fileName.Replace(opposite, separator);
            else
                return fileName;
        }

        public static string RootPath
        {
            get
            {
                var path = Settings.Path;
                if (path.IsEmptyOrNull())
                    throw new InvalidOperationException("Please make sure Path in appSettings\\UploadSettings is configured!");

                string pre1 = @"~\";
                string pre2 = @"~/";
                if (path.StartsWith(pre1) || path.StartsWith(pre2))
                {
#if COREFX
                    path = Path.Combine(Path.GetDirectoryName(HostingEnvironment.MapPath(pre2)), ToPath(path.Substring(pre2.Length)));
#else
                    path = Path.Combine(HostingEnvironment.MapPath(pre2), ToPath(path.Substring(pre2.Length)));
#endif
                    settings.Path = path;
                }
                
                return path;
            }
        }

        public static string TemporaryPath
        {
            get
            {
                var path = Path.Combine(RootPath, @"temporary/".Replace('/', Path.DirectorySeparatorChar));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    File.WriteAllText(Path.Combine(path, ".temporary"), "");
                }
                return path;
            }
        }

        public static string RootUrl
        {
            get
            {
                return Settings.Url;
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
        public static string ImageFileUrl(string fileName, bool thumbnailUrl)
        {
            fileName = StringHelper.TrimToNull(fileName);

            if (fileName == null)
                return null;

            if (thumbnailUrl)
                fileName = UploadHelper.GetThumbFileName(fileName);

            return RootUrl + ToUrl(fileName);
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

            return String.Format(format, identity, groupKey, TemporaryFileHelper.RandomFileCode(), DateTime.Now, 
                Path.GetFileNameWithoutExtension(originalName)) + extension;
        }

        public static void DeleteFileAndRelated(string dbFileName, DeleteType deleteType)
        {
            dbFileName = StringHelper.TrimToEmpty(dbFileName);
            if (dbFileName.Length > 0)
            {
                var fileName = Path.Combine(RootPath, ToPath(dbFileName));

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

        public static void CheckFileNameSecurity(string fileName)
        {
            if (fileName == null ||
                fileName.Length == 0 ||
                fileName.IndexOf("..") >= 0 ||
                fileName.StartsWith("/") ||
                fileName.StartsWith("\\") ||
                fileName.EndsWith("/") ||
                fileName.EndsWith("\\"))
#if ASPNETCORE
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

        public static void RegisterFilesToDelete(IUnitOfWork unitOfWork, FilesToDelete filesToDelete)
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
                    ex.Log();
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
                    ex.Log();
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

            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (regKey != null && regKey.GetValue("Content Type") != null)
                    mimeType = regKey.GetValue("Content Type").ToString();
            }
            catch (Exception)
            {
                return "application/unknown";
            }

            return mimeType;
        }
    }
}
