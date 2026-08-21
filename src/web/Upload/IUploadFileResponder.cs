using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Abstraction for reading a file via <c>/upload/path</c>,
/// e.g. the Read action in the FilePage.
/// </summary>
public interface IUploadFileResponder
{
    /// <summary>
    /// Reads the file, e.g. writes to the response.
    /// </summary>
    /// <param name="pathInfo">The path information.</param>
    /// <param name="responseHeaders">The response headers.</param>
    /// <returns>An action result based on the file mime type.</returns>
    IActionResult Read(string pathInfo, IHeaderDictionary responseHeaders);
}