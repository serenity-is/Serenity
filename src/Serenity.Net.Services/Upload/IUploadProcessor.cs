using System.IO;

namespace Serenity.Web;

/// <summary>
/// Abstraction for an upload processor, which handles temporary uploads
/// </summary>
public interface IUploadProcessor
{
    /// <summary>
    /// Processes a temporary upload stream, usually from the HTTP request files and
    /// returns information about the result
    /// </summary>
    /// <param name="fileContent">File content stream (usually from HTTP request files)</param>
    /// <param name="filename">The filename of the uploaded file (original name)</param>
    /// <param name="options">Upload options to use for validation / thumbnail generation</param>
    ProcessedUploadInfo Process(Stream fileContent, string filename, IUploadOptions options);
}