using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Default implementation for upload file responder
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="uploadStorage">Upload storage</param>
/// <exception cref="ArgumentNullException">One of arguments is null</exception>
public class DefaultUploadFileResponder(IUploadStorage uploadStorage) : IUploadFileResponder
{
    private readonly IUploadStorage uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));

    /// <inheritdoc />
    public IActionResult Read(string pathInfo, IHeaderDictionary responseHeaders)
    {
        UploadPathHelper.CheckFileNameSecurity(pathInfo);

        if (!uploadStorage.FileExists(pathInfo))
            return new NotFoundResult();

        var mimeType = KnownMimeTypes.Get(pathInfo);

        var stream = uploadStorage.OpenFile(pathInfo);
        responseHeaders.Append("X-Content-Type-Options", "nosniff");

        if (mimeType == "application/pdf" ||
            mimeType == "text/plain" ||
            mimeType.StartsWith("image/"))
        {
            return new FileStreamResult(stream, mimeType);
        }

        var cd = new System.Net.Mime.ContentDisposition
        {
            FileName = System.IO.Path.GetFileName(pathInfo),
            Inline = false
        };

        responseHeaders.Append("Content-Disposition", cd.ToString());

        return new FileStreamResult(stream, "application/octet-stream");
    }
}