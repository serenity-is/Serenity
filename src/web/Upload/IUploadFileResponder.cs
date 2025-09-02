using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Abstraction for reading a file via /upload/path, 
/// e.g. the Read action in the FilePage
/// </summary>
public interface IUploadFileResponder
{
    /// <summary>
    /// Reads the file, e.g. writes to the response
    /// </summary>
    /// <param name="pathInfo"></param>
    /// <param name="responseHeaders">Response headers</param>
    /// <returns>An action result based on file mime type</returns>
    IActionResult Read(string pathInfo, IHeaderDictionary responseHeaders);
}