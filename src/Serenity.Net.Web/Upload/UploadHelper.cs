using Serenity.Data;
using Serenity.IO;
using System;
using System.Collections.Generic;
using Path = System.IO.Path;

namespace Serenity.Web
{
    public static class UploadHelper
    {
        public static string GetThumbFileName(string fileName, string thumbSuffix = "_t.jpg")
        {
            if (string.IsNullOrEmpty(fileName))
                return fileName;

            return Path.ChangeExtension(fileName, null) + thumbSuffix;
        }

        public static string DbThumbUrl(this IUploadStorage storage, string dbFileName)
        {
            if (string.IsNullOrEmpty(dbFileName))
                return null;

            var dbFileThumb = GetThumbFileName(dbFileName);

            return storage.GetFileUrl(dbFileThumb);
        }

        public static string FormatDbFileName(FormatFilenameOptions options)
        {
            long l;
            object groupKey;
            string s;
            object identity = options.EntityId;
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

            var formatted = string.Format(options.Format, identity, groupKey, 
                TemporaryFileHelper.RandomFileCode(), DateTime.Now,
                Path.GetFileNameWithoutExtension(options.OriginalName)) + Path.GetExtension(options.OriginalName);

            if (options.PostFormat != null)
                formatted = options.PostFormat(formatted);

            return formatted;
        }

        public static string FindAvailableName(this IUploadStorage storage, string dbFileName)
        {
            var extension = Path.GetExtension(dbFileName);
            string baseFileName = null;
            int tries = 0;
            while (storage.FileExists(dbFileName) && ++tries < 10000)
            {
                if (baseFileName == null)
                    baseFileName = Path.ChangeExtension(dbFileName, null);

                dbFileName = baseFileName + " (" + tries + ")" + (extension ?? "");
            }

            return dbFileName;
        }

        public static CopyTemporaryFileResult CopyTemporaryFile(this IUploadStorage storage, CopyTemporaryFileOptions options)
        {
            string dbFileName = PathHelper.ToUrl(FormatDbFileName(options));
            dbFileName = FindAvailableName(storage, dbFileName);

            storage.CopyFile(storage, options.TemporaryFile, dbFileName, false);
            long size = storage.GetFileSize(options.TemporaryFile);
            bool hasThumbnail = storage.FileExists(GetThumbFileName(options.TemporaryFile));

            var result = new CopyTemporaryFileResult()
            {
                FileSize = size,
                DbFileName = dbFileName,
                OriginalName = options.OriginalName,
                HasThumbnail = hasThumbnail
            };

            options.FilesToDelete?.RegisterNewFile(dbFileName);
            options.FilesToDelete?.RegisterOldFile(options.TemporaryFile);
            return result;
        }

        public static void CheckFileNameSecurity(string fileName)
        {
            if (!PathHelper.IsSecureRelativeFile(fileName))
                throw new ArgumentOutOfRangeException("fileName");
        }

        public static string FileNameSizeDisplay(string name, int bytes)
        {
            return name + " (" + FileSizeDisplay(bytes) + ')';
        }

        public static string FileSizeDisplay(int bytes)
        {
            var byteSize = (Math.Round((decimal)bytes * 100m / 1024m) * 0.01m);
            var suffix = "KB";
            if (byteSize > 1000)
            {
                byteSize = (Math.Round((decimal)byteSize * 0.001m * 100m) * 0.01m);
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
                catch
                {
                }
            };

            unitOfWork.OnRollback += delegate()
            {
                try
                {
                    filesToDelete.Dispose();
                }
                catch
                {
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
