using System.IO;

namespace Serenity.IO;

/// <summary>
///   Contains helper functions for temporary files and folders</summary>
public class TemporaryFileHelper
{
    /// <summary>
    ///   A signature file that marks a folder as a temporary file to ensure that it actually contains temporary
    ///   files and can be safely cleaned</summary>
    public const string DefaultTemporaryCheckFile = ".temporary";

    /// <summary>
    ///   By default, files older than 1 hour is cleared</summary>
    public static readonly TimeSpan DefaultAutoExpireTime = TimeSpan.FromDays(1);

    /// <summary>
    ///   By default, if more than 1000 files exists in directory, they are deleted</summary>
    public const int DefaultMaxFilesInDirectory = 1000;

    /// <summary>
    ///   Clears a folder based on default conditions</summary>
    /// <param name="directoryToClean">
    ///   Folder to be cleared</param>
    /// <remarks>
    ///   If any errors occur during cleanup, this doesn't raise an exception
    ///   and ignored. Other errors might raise an exception. As errors are
    ///   ignored, method can't guarantee that less than specified number of files
    ///   will be in the folder after it ends.</remarks>
    public static void PurgeDirectoryDefault(string directoryToClean)
    {
        PurgeDirectory(directoryToClean, DefaultAutoExpireTime, DefaultMaxFilesInDirectory, DefaultTemporaryCheckFile);
    }

    /// <summary>
    ///   Clears a folder based on specified conditions</summary>
    /// <param name="directoryToClean">
    ///   Folder to be cleared</param>
    /// <param name="autoExpireTime">
    ///   Files with creation time older than this is deleted. If passed as 0, time
    ///   based cleanup is skipped.</param>
    /// <param name="maxFilesInDirectory">
    ///   If more than this number of files exists, files will be deleted starting from 
    ///   oldest to newest. By passing 0, all files can be deleted. If passed as -1,
    ///   file count based cleanup is skipped.</param>
    /// <param name="checkFileName">
    ///   Safety file to be checked. If it is specified and it doesn't exists, operation
    ///   is aborted.</param>
    /// <remarks>
    ///   If any errors occur during cleanup, this doesn't raise an exception
    ///   and ignored. Other errors might raise an exception. As errors are
    ///   ignored, method can't guarantee that less than specified number of files
    ///   will be in the folder after it ends.</remarks>
    public static void PurgeDirectory(string directoryToClean,
        TimeSpan autoExpireTime, int maxFilesInDirectory, string checkFileName)
    {
        checkFileName ??= string.Empty;
        if (checkFileName.Length > 0)
        {
            checkFileName = System.IO.Path.GetFileName(checkFileName).Trim();
            if (!System.IO.File.Exists(System.IO.Path.Combine(directoryToClean, checkFileName)))
                return;
        }

        // get folder information
        DirectoryInfo directoryInfo = new(directoryToClean);

        // if no time condition, or all files are to be deleted (maxFilesInDirectory = 0) 
        // no need for this part
        if (autoExpireTime.Ticks != 0 && maxFilesInDirectory != 0)
        {
            // subtract limit from now and find lower limit for files to be deleted
            DateTime autoExpireLimit = DateTime.Now.Subtract(autoExpireTime);

            // traverse all files and if older than limit, try to delete
            foreach (FileInfo fiOld in directoryInfo.GetFiles()
                .Where(fi => fi.CreationTime < autoExpireLimit))
            {
                if (!checkFileName.Equals(fiOld.Name, StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        fiOld.Delete();
                    }
                    catch
                    {
                    }
                }
            }
        }

        // if maxFilesInDirectory is -1 than no count based deletion
        if (maxFilesInDirectory >= 0)
        {
            // list all files
            FileInfo[] files = directoryInfo.GetFiles();

            // if count is above limit
            if (files.Length > maxFilesInDirectory)
            {
                // if not all files are going to be deleted, sort them by date
                if (maxFilesInDirectory != 0)
                {
                    Array.Sort(files,
                        delegate (FileInfo x, FileInfo y)
                        { return x.CreationTime < y.CreationTime ? -1 : 1; });
                }

                // delete all before last "maxFilesInDirectory" files.
                for (int i = 0; i < files.Length - maxFilesInDirectory; i++)
                {
                    if (!checkFileName.Equals(files[i].Name, StringComparison.OrdinalIgnoreCase))
                        try
                        {
                            files[i].Delete();
                        }
                        catch
                        {
                        }
                }
            }
        }
    }

    /// <summary>
    ///   Tries to delete a file with given path.</summary>
    /// <param name="filePath">
    ///   File to be deleted (can be null).</param>
    public static void TryDelete(string filePath)
    {
        if (File.Exists(filePath))
            try
            {
                Delete(filePath);
            }
            catch
            {

            }
    }

    /// <summary>
    ///   Deletes a file.</summary>
    /// <param name="filePath">
    ///   File to be deleted (can be null).</param>
    public static void Delete(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        filePath += ".delete";
        if (File.Exists(filePath))
            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }
    }

    /// <summary>
    ///   Deletes, tries to delete or marks a file for deletion depending on type.</summary>
    /// <param name="filePath">
    ///   File to be deleted (can be null).</param>
    /// <param name="type">
    ///   Delete type.</param>
    public static void Delete(string filePath, DeleteType type)
    {
        if (type == DeleteType.Delete)
            Delete(filePath);
        else if (type == DeleteType.TryDelete)
            TryDelete(filePath);
        else
            TryDeleteOrMark(filePath);
    }

    /// <summary>
    ///   Tries to delete a file or marks it for deletion by DeleteMarkedFiles method by
    ///   creating a ".delete" file.</summary>
    /// <param name="filePath">
    ///   File to be deleted</param>
    public static void TryDeleteOrMark(string filePath)
    {
        TryDelete(filePath);
        if (File.Exists(filePath))
        {
            try
            {
                string deleteFile = filePath + ".delete";
                long fileTime = File.GetLastWriteTimeUtc(filePath).ToFileTimeUtc();
                using var sw = new StreamWriter(File.OpenWrite(deleteFile));
                sw.Write(fileTime);
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///   Tries to delete all files that is marked for deletion by TryDeleteOrMark in a folder.</summary>
    /// <param name="path">
    ///   Path of marked files to be deleted</param>
    public static void TryDeleteMarkedFiles(string path)
    {
        if (Directory.Exists(path))
        {
            foreach (var name in Directory.GetFiles(path, "*.delete"))
            {
                try
                {
                    string readLine;
                    using (var sr = new StreamReader(File.OpenRead(name)))
                        readLine = sr.ReadToEnd();
                    string actualFile = name[0..^7];
                    if (File.Exists(actualFile))
                    {
                        if (long.TryParse(readLine, out long fileTime))
                        {
                            if (fileTime == File.GetLastWriteTimeUtc(actualFile).ToFileTimeUtc())
                                TryDelete(actualFile);
                        }
                        TryDelete(name);
                    }
                    else
                        TryDelete(name);
                }
                catch
                {
                }
            }
        }
    }

    private class TempFile
    {
        public string? Filename;
        public DateTime? Expiry;
        public bool RemoveFolder;
    }

    private static readonly List<TempFile> _tempFiles = new();

    /// <summary>
    /// Clears the temporary files.
    /// </summary>
    /// <param name="ignoreExpiry">if set to <c>true</c> ignore expiry dates.</param>
    public static void ClearTempFiles(bool ignoreExpiry)
    {
        lock (_tempFiles)
        {
            DateTime utcNow = DateTime.UtcNow;
            for (var i = _tempFiles.Count - 1; i >= 0; i--)
            {
                var tf = _tempFiles[i];
                if (ignoreExpiry || (tf.Expiry != null && tf.Expiry <= utcNow))
                {
                    TryDelete(tf.Filename!);
                    if (!File.Exists(tf.Filename))
                    {
                        _tempFiles.RemoveAt(i);
                        if (tf.RemoveFolder)
                            try
                            {
                                Directory.Delete(Path.GetDirectoryName(tf.Filename));
                            }
                            catch
                            {
                            }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Registers the temporary file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="expiry">The expiry.</param>
    /// <param name="removeFolder">if set to <c>true</c> [remove folder].</param>
    public static void RegisterTempFile(string filename, DateTime? expiry, bool removeFolder)
    {
        TempFile tf = new()
        {
            Filename = filename,
            Expiry = expiry,
            RemoveFolder = removeFolder
        };
        lock (_tempFiles)
        {
            _tempFiles.Add(tf);
        }
    }

    /// <summary>
    ///   Gets a 13 character random code that can be used safely in a filename</summary>
    /// <returns>
    ///   A random code.</returns>
    public static string RandomFileCode()
    {
        Guid guid = Guid.NewGuid();
        var guidBytes = guid.ToByteArray();
        var eightBytes = new byte[8];
        for (int i = 0; i < 8; i++)
            eightBytes[i] = (byte)(guidBytes[i] ^ guidBytes[i + 8]);
        return Base32.Encode(eightBytes);
    }
}