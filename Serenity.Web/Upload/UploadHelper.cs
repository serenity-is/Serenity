using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace Serenity.Web
{
    public class CopyTemporaryFileResult
    {
        public string TemporaryFilePath { get; set; }
        public string DbFileName { get; set; }
        public string DbFilePath { get; set; }
        public bool HasThumbnail { get; set; }
        public long FileSize { get; set; }
    }

    public enum UploadRoot
    {
        Default,
    }

    public enum ImageDisplayMode
    {
        None = 0,
        Image = 1,
        Thumbnail = 2
    }

    public class UploadSettings
    {
        private static UploadSettings current;

        public static UploadSettings Current
        {
            get 
            {
                current = current ?? JsonConvert.DeserializeObject<UploadSettings>(
                    ConfigurationManager.AppSettings["UploadSettings"].TrimToNull() ?? "{}", JsonSettings.Tolerant);

                return current;
            }
        }

        public string Url { get; set; }
        public string Path { get; set; }
    }

    public class UploadHelper
    {
        public const string HistoryFolder = "history/";
        public const string TemporaryFolder = "temporary/";

        private UploadRoot uploadRoot;
        private string subFolder;
        private string fileNameFormat;

        public UploadHelper(UploadRoot root, string subFolder, string fileNameFormat)
        {
            this.uploadRoot = root;
            this.subFolder = subFolder;
            this.fileNameFormat = fileNameFormat;
        }

        public string SubFolder
        {
            get { return subFolder; }
        }

        public string DbFilePath(string dbFileName)
        {
            return DbFilePath(uploadRoot, subFolder, ToPath(dbFileName));
        }

        public static string DbFilePath(UploadRoot root, string subFolder, string file)
        {
            return Path.Combine(UploadPath(root, subFolder), ToPath(file));
        }

        public string FormatDbFileName(Int64 entityId, string extension)
        {
            return FormatUploadFileName(fileNameFormat, entityId, extension);
        }

        public CopyTemporaryFileResult CopyTemporaryFile(string temporaryFileName, Int64 entityId, FilesToDelete filesToDelete)
        {
            var result = CopyTemporaryFile(temporaryFileName, entityId);
            filesToDelete.Register(result);
            return result;
        }

        public CopyTemporaryFileResult CopyTemporaryFile(string temporaryFileName, Int64 entityId)
        {
            temporaryFileName = Path.GetFileName(temporaryFileName);
            string dbFileName = ToUrl(FormatDbFileName(entityId, temporaryFileName));
            string dbFilePath = DbFilePath(dbFileName);
            string temporaryFilePath = Path.Combine(UploadHelper.TemporaryPath(uploadRoot), temporaryFileName);
            UploadHelper.CopyFileAndRelated(temporaryFilePath, dbFilePath);
            long size = new FileInfo(dbFilePath).Length;
            bool hasThumbnail = File.Exists(TemporaryImageUpload.GetThumbFileName(dbFilePath, null));
            return new CopyTemporaryFileResult()
            {
                FileSize = size,
                DbFileName = dbFileName,
                DbFilePath = dbFilePath,
                HasThumbnail = hasThumbnail,
                TemporaryFilePath = temporaryFilePath
            };
        }

        /// <summary>
        ///   Copies a file (and if exists its thumb file) to history folder and returns auto-generated
        ///   file name that is created.</summary>
        /// <param name="root">
        ///   Upload root for files.</param>       
        /// <param name="filePath">
        ///   Source file, required, must exist.</param>
        /// <returns>
        ///   Auto generated history filename (actual filename, without special "history?" prefix and
        ///   folder information).</returns>
        public static string CopyFileAndRelatedToHistory(UploadRoot root, string sourceFilePath)
        {
            string date = DateTime.UtcNow.ToString("yyyyMMdd", Invariants.DateTimeFormat);
            string historyFile = date + "/" + Guid.NewGuid().ToString("N") + Path.GetExtension(sourceFilePath);

            CopyFileAndRelated(sourceFilePath, UploadFilePath(UploadHelper.HistoryFolder, historyFile));

            return historyFile;
        }

        /// <summary>
        ///   Copies a file and its related files to a target file</summary>
        /// <param name="root">
        ///   Upload root</param>
        /// <param name="sourceFileName">
        ///   Source file</param>
        /// <param name="targetFileName">
        ///   Target file</param>
        /// <returns>
        ///   Target file name</returns>
        public static void CopyFileAndRelated(string sourceFilePath, string targetFilePath)
        {
            var targetPath = Path.GetDirectoryName(targetFilePath);
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            string targetFilename = Path.GetFileName(targetFilePath);

            File.Copy(sourceFilePath, targetFilePath);

            string metaPath = sourceFilePath + ".meta";
            if (File.Exists(metaPath))
                File.Copy(metaPath, Path.Combine(targetPath, targetFilename + ".meta"));

            string sourcePath = Path.GetDirectoryName(sourceFilePath);
            string sourceBase = Path.GetFileNameWithoutExtension(sourceFilePath);
            string targetBase = Path.GetFileNameWithoutExtension(targetFilePath);

            foreach (var f in Directory.GetFiles(sourcePath,
                sourceBase + "_t*.jpg"))
            {
                string thumbSuffix = Path.GetFileName(f).Substring(sourceBase.Length);
                File.Copy(f, Path.Combine(targetPath, targetBase + thumbSuffix));
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
            if (fileName != null && fileName.IndexOf('/') >= 0)
                return fileName.Replace('/', '\\');
            else
                return fileName;
        }

        /// <summary>
        ///   Gets full path to the temporary files folder.</summary>
        /// <param name="root">
        ///   Root folder for files (Default)</param>
        /// <returns>
        ///   Full path to the temporary files folder.</returns>
        public static string TemporaryPath(UploadRoot root)
        {
            return UploadPath(root, UploadHelper.TemporaryFolder);
        }

        /// <summary>
        ///   Gets URL for the temporary files folder.</summary>
        /// <returns>
        ///   Temporary files folder URL.</returns>
        public static string TemporaryUrl()
        {
            return UploadUrl(UploadHelper.TemporaryFolder);
        }

        /// <summary>
        ///   Gets full path to a folder under root upload folder.</summary>
        /// <param name="root">
        ///   Upload root (Default)</param>
        /// <param name="subFolder">
        ///   Subfolder name (may contain "/", as they are replaced with "\").</param>
        /// <returns>
        ///   Full path to the upload subfolder.</returns>
        public static string UploadPath(UploadRoot root, string subFolder)
        {
            if (root == UploadRoot.Default)
                return Path.Combine(UploadSettings.Current.Path, ToPath(subFolder));
            else
                throw new ArgumentOutOfRangeException("root");
        }

        /// <summary>
        ///   Gets full path to a folder under root upload folder.</summary>
        /// <param name="root">
        ///   Upload root (Default)</param>
        /// <param name="subFolder">
        ///   Subfolder name (may contain "/", as they are replaced with "\").</param>
        /// <returns>
        ///   Full path to the upload subfolder.</returns>
        public static string UploadPath(string subFolder)
        {
            return UploadPath(UploadRoot.Default, subFolder);
        }

        /// <summary>
        ///   Gets full path to a folder under root upload folder.</summary>
        /// <param name="root">
        ///   Upload root (Default )</param>
        /// <param name="subFolder">
        ///   Subfolder name (may contain "/", as they are replaced with "\").</param>
        /// <returns>
        ///   Full path to the upload subfolder.</returns>
        public static string UploadFilePath(string subFolder, string file)
        {
            return Path.Combine(UploadPath(UploadRoot.Default, subFolder), ToPath(file));
        }

        /// <summary>
        ///   Gets full path to a folder under root upload folder.</summary>
        /// <param name="root">
        ///   Upload root (Default)</param>
        /// <param name="subFolder">
        ///   Subfolder name (may contain "/", as they are replaced with "\").</param>
        /// <returns>
        ///   Full path to the upload subfolder.</returns>
        public static string UploadFilePath(UploadRoot root, string subFolder, string file)
        {
            return Path.Combine(UploadPath(root, subFolder), ToPath(file));
        }

        /// <summary>
        ///   Gets URL of a folder under root upload folder.</summary>
        /// <param name="subFolder">
        ///   Subfolder (may contain "/").</param>
        /// <returns>
        ///   URL of the upload subfolder.</returns>
        public static string UploadUrl(string subFolder)
        {
            return UploadSettings.Current.Url + ToUrl(subFolder);
        }

        /// <summary>
        ///   Gets URL of an image file or its thumbnail under a subfolder of file upload root. 
        ///   If filename starts with "approval?", approval folder is used instead of 
        ///   the specified subfolder name. If file name is empty, then a null image
        ///   file URL is returned</summary>
        /// <param name="subFolder">
        ///   Subfolder (might contain "/").</param>
        /// <param name="fileName">
        ///   File name.</param>
        /// <param name="thumbnailUrl">
        ///   True to return thumbnail URL.</param>
        /// <returns>
        ///   URL of the image file or its thumbnail, or null.</returns>       
        public static string ImageFileUrl(string subFolder, string fileName, bool thumbnailUrl)
        {
            fileName = StringHelper.TrimToNull(fileName);

            if (fileName == null)
                return null;

            if (thumbnailUrl)
                fileName = TemporaryImageUpload.GetThumbFileName(fileName, null);

            return UploadUrl(subFolder) + ToUrl(fileName);
        }


        /// <summary>
        ///   Checks if file name ends with .SWF.</summary>
        /// <param name="fileName">
        ///   File name.</param>
        /// <returns>
        ///   True if file has .SWF extension.</returns>
        public static bool IsSWF(string fileName)
        {
            return System.IO.Path.GetExtension(fileName ?? String.Empty)
                .Equals(".swf", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        ///   Gets a file name for given identity and format</summary>
        /// <returns>
        ///   Formatted file name, may include a random file code and division (1000's) depending on format.</returns>
        public static string FormatUploadFileName(string format, Int64 identity)
        {
            return String.Format(format, identity, identity / 1000, TemporaryFileHelper.RandomFileCode());
        }

        /// <summary>
        ///   Gets a file name for given identity and format</summary>
        /// <returns>
        ///   Formatted file name, may include a random file code and division (1000's) depending on format.</returns>
        public static string FormatUploadFileName(string format, Int64 identity, string extension)
        {
            return String.Format(format, identity, identity / 1000, TemporaryFileHelper.RandomFileCode()) + Path.GetExtension(extension);
        }

        /// <summary>
        ///   Deletes a file under a subfolder of the file upload root. If file name starts with
        ///   "approval?" prefix, approval folder is used instead of the subfolder specified.</summary>
        /// <param name="root">
        ///   Root folder for files (Default)</param>
        /// <param name="subFolder">
        ///   Upload subfolder.</param>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <param name="deleteType">
        ///   File deletion type.</param>
        /// <remarks>
        ///   This method doesn't try to delete image thumbnails, but if upload root,
        ///   tries to delete ".meta" files too. Use DeleteImageFile for image deletion</remarks>
        public static void DeleteFileAndRelated(string filePath, DeleteType deleteType)
        {
            DeleteFileAndRelated(UploadRoot.Default, null, filePath, deleteType);
        }

        /// <summary>
        ///   Deletes a file under a subfolder of the file upload root. If file name starts with
        ///   "approval?" prefix, approval folder is used instead of the subfolder specified.</summary>
        /// <param name="root">
        ///   Root folder for files (Default)</param>
        /// <param name="subFolder">
        ///   Upload subfolder.</param>
        /// <param name="fileName">
        ///   Filename.</param>
        /// <param name="deleteType">
        ///   File deletion type.</param>
        /// <remarks>
        ///   This method doesn't try to delete image thumbnails. Use DeleteImageFile for image deletion</remarks>
        public static void DeleteFileAndRelated(UploadRoot root, string subFolder, string fileName,
            DeleteType deleteType)
        {
            fileName = StringHelper.TrimToEmpty(fileName);
            if (fileName.Length > 0)
            {
                if (subFolder != null)
                    fileName = Path.Combine(UploadPath(root, subFolder), ToPath(fileName));

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
                throw new HttpException(0x194, "Invalid_Request");
        }
    }

}