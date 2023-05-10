using System.IO;

namespace Serenity.Web;

/// <summary>
/// Abstraction for an upload antivirus scanner, which scans temporary uploads before getting processed
/// </summary>
public interface IUploadAVScanner
{
    /// <summary>
    /// Processes a temporary upload stream, usually from the HTTP request files and
    /// raises an exception if any issues found or an error occured during scan
    /// </summary>
    /// <param name="stream">File content stream (usually from HTTP request files)</param>
    /// <param name="filename">The filename of the uploaded file (original name)</param>
    void Scan(Stream stream, string filename);
}