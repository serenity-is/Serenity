using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Serenity.Web
{
    /// <summary>
    ///   An object that holds file upload paramaters/state for a single upload and stored in session/cache to 
    ///   let file upload status and parameters passed between popup window ile file upload panels.
    ///   Also contains some helper functions.</summary>
    [Serializable]
    public class TemporaryFileUpload : IDisposable
    {
        private Guid guid;

        private string temporaryPath;
        private string temporaryUrl;
        private int minFileSize;
        private int maxFileSize;

        private bool hasFile;
        private bool isUploadFailed;
        private string errorMessage;
        private string uploadedFile;
        private string originalFile;
        private long fileSize;

        /// <summary>
        ///   Checks if uploaded file exists.</summary>
        public virtual bool HasFile
        {
            get
            {
                if (hasFile)
                {
                    if ((!FileExists(uploadedFile)))
                        hasFile = false;
                }
                return hasFile;
            }
            internal set
            {
                hasFile = value;
            }
        }

        /// <summary>
        ///   Clears temporary files if any and resets TemporaryFile slot state.</summary>
        public void ClearAll()
        {
            ClearFiles();
            ClearState();
        }

        /// <summary>
        ///   Disposes object</summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///   Called by finalizer and dispose to clear files</summary>
        /// <param name="disposing">
        ///   True if called from Dispose.</param>
        public void Dispose(bool disposing)
        {
            ClearFiles();
        }

        /// <summary>
        ///   Finalizer</summary>
        ~TemporaryFileUpload()
        {
            Dispose(false);
        }

        /// <summary>
        ///   Clears temporary files.</summary>
        protected internal virtual void ClearFiles()
        {
            hasFile = false;
            if (!uploadedFile.IsEmptyOrNull())
                try
                {
                    TemporaryFileHelper.Delete(uploadedFile, DeleteType.Delete);
                }
                catch
                {
                }
            uploadedFile = "";
        }

        /// <summary>
        ///   Clears upload states of the object.</summary>
        protected internal virtual void ClearState()
        {
            isUploadFailed = false;
            errorMessage = "";
            originalFile = "";
            fileSize = 0;
        }

        /// <summary>
        ///   Returns success status of latest upload try.</summary>
        public bool IsUploadFailed
        {
            get { return isUploadFailed; }
            internal set { isUploadFailed = value; }
        }

        /// <summary>
        ///   If last upload failed, contains the error message.</summary>
        public string ErrorMessage
        {
            get
            {
                if (isUploadFailed)
                    return errorMessage;
                else
                    return String.Empty;
            }
            internal set
            {
                errorMessage = value;
            }
        }

        /// <summary>
        ///   Contains file size after upload.</summary>
        public long FileSize
        {
            get { return fileSize; }
            internal set { fileSize = value; }
        }

        /// <summary>
        ///   Contains original file name after upload.</summary>
        public string OriginalFile
        {
            get { return originalFile; }
            internal set { originalFile = value; }
        }

        /// <summary>
        ///   Contains GUID based random file name assigned to the uploaded file.</summary>
        public string UploadedFile
        {
            get { return uploadedFile; }
            internal set { uploadedFile = value; }
        }

        /// <summary>
        ///   Gets the URL of uploaded file.</summary>
        public virtual string TemporaryFileUrl
        {
            get
            {
                if (HasFile)
                {
                    string temp = UrlHelper.Combine(TemporaryUrl, Path.GetFileName(uploadedFile));
                    return temp.Length == 0 ? "" : UrlHelper.ResolveUrl(temp);
                }
                else
                    return "";
            }
        }

        /// <summary>
        ///   Creates a new TemporaryFile object and assings a special GUID.</summary>
        public TemporaryFileUpload()
        {
            guid = Guid.NewGuid();
        }

        /// <summary>
        ///   Unique GUID for this TemporaryFile instance.</summary>
        public Guid Guid
        {
            get { return guid; }
        }

        /// <summary>
        ///   Checks if file exists.</summary>
        /// <param name="fileName">
        ///   File name (if null or empty return value is false)</param>
        /// <returns>
        ///   If file exists, true.</returns>
        protected static bool FileExists(string fileName)
        {
            return
                fileName != null &&
                fileName.Length > 0 &&
                File.Exists(fileName);
        }

        /// <summary>
        ///   Temporary path files will be uploaded to.</summary>
        public string TemporaryPath
        {
            get { return temporaryPath; }
            set { temporaryPath = value; }
        }

        /// <summary>
        ///   Temporary URL files will be uploaded to.</summary>
        public string TemporaryUrl
        {
            get { return temporaryUrl; }
            set { temporaryUrl = value; }
        }

        /// <summary>
        ///   Minimum file size allowed</summary>
        public int MinFileSize
        {
            get { return minFileSize; }
            set { minFileSize = value; }
        }

        /// <summary>
        ///   Maximum file size allowed</summary>
        public int MaxFileSize
        {
            get { return maxFileSize; }
            set { maxFileSize = value; }
        }

        /// <summary>
        ///   Tries to upload a file.</summary>
        /// <param name="uploadSlot">
        ///   Upload slot.</param>
        /// <param name="postedFile">
        ///   Stream with posted file content</param>
        /// <param name="originalFile">
        ///   Original file name uploaded by the user.</param>
        /// <remarks>
        ///   No exceptions raised if an error occurs. Any error can be determined by 
        ///   examining IsUploadFailed and ErrorMessage properties.</remarks>
        public virtual void TryUploadingFile(Stream postedFile, string originalFile)
        {
            ClearAll();

            try
            {
                IsUploadFailed = false;

                if (maxFileSize != 0 &&
                    postedFile.Length > maxFileSize)
                {
                    IsUploadFailed = true;
                    ErrorMessage = LocalText.GetFormat("cms.upload_panel.file_max_size_info",
                        maxFileSize, postedFile.Length);
                }
                else if (
                    minFileSize != 0 &&
                    postedFile.Length < minFileSize)
                {
                    IsUploadFailed = true;
                    ErrorMessage = LocalText.GetFormat("cms.upload_panel.file_min_size_info",
                        minFileSize, postedFile.Length);
                }

                FileSize = postedFile.Length;
                OriginalFile = originalFile;

                if (!IsUploadFailed)
                {
                    string fileExtension = Path.GetExtension(originalFile);

                    TemporaryFileHelper.PurgeDirectoryDefault(temporaryPath);

                    string baseFileName = System.IO.Path.Combine(temporaryPath,
                        Guid.NewGuid().ToString("N"));

                    UploadedFile = baseFileName + fileExtension;

                    using (Stream stream = new FileStream(UploadedFile, FileMode.Create))
                    {
                        CopyStream(postedFile, stream);
                    }
                    HasFile = true;
                }
            }
            catch (Exception exc)
            {
                ClearFiles();
                ErrorMessage = exc.Message;
                IsUploadFailed = true;
            }
        }

        /// <summary>
        ///   Copies a stream's content to another till the end.</summary>
        /// <param name="source">
        ///   Source stream (required).</param>
        /// <param name="dest">
        ///   Target stream (required).</param>
        public static void CopyStream(Stream source, Stream dest)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (dest == null)
                throw new ArgumentNullException("dest");

            byte[] buffer = new byte[4096];
            int read;
            do
            {
                read = source.Read(buffer, 0, buffer.Length);
                if (read != 0)
                    dest.Write(buffer, 0, read);
            } while (read != 0);
        }

        /// <summary>
        ///   Saves a temporary files content to given file.</summary>
        /// <param name="uploadSlot">
        ///   File upload slot.</param>
        /// <param name="fileName">
        ///   Target file (required).</param>
        public virtual void SaveToFile(string fileName)
        {
            if (fileName == null || fileName.Length == 0 || Path.GetFileName(fileName).IsEmptyOrNull())
                throw new ArgumentNullException("fileName");

            if (!HasFile)
                throw new InvalidOperationException("TemporaryFile has no file to save");

            try
            {
                TemporaryFileHelper.Delete(fileName, DeleteType.Delete);
                File.Copy(UploadedFile, fileName);
                TemporaryFileHelper.Delete(UploadedFile, DeleteType.TryDelete);
            }
            finally
            {
                ClearAll();
            }
        }

        /// <summary>
        ///   Saves a temporary files content to given file.</summary>
        /// <param name="uploadSlot">
        ///   File upload slot.</param>
        /// <param name="fileName">
        ///   Target file (required).</param>
        public virtual void RelaseAndKeepTemporaryFile()
        {
            UploadedFile = null;
        }
    }
}