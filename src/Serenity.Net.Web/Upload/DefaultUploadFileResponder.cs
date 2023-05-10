using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web;

/// <summary>
/// Default implementation for upload file responder
/// </summary>
public class DefaultUploadFileResponder : IUploadFileResponder
{
    private readonly IUploadStorage uploadStorage;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="uploadStorage">Upload storage</param>
    /// <exception cref="ArgumentNullException">One of arguments is null</exception>
    public DefaultUploadFileResponder(IUploadStorage uploadStorage)
    {
        this.uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));
    }

    /// <inheritdoc />
    public IActionResult Read(string pathInfo, IHeaderDictionary responseHeaders)
    {
        UploadPathHelper.CheckFileNameSecurity(pathInfo);

        if (!uploadStorage.FileExists(pathInfo))
            return new NotFoundResult();

        var mimeType = KnownMimeTypes.Get(pathInfo);

        var stream = uploadStorage.OpenFile(pathInfo);
        responseHeaders.Add("X-Content-Type-Options", "nosniff");

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

        responseHeaders.Add("Content-Disposition", cd.ToString());

        return new FileStreamResult(stream, "application/octet-stream");
    }
}