using Microsoft.AspNetCore.Http;

namespace Serene.Common.Pages;

public class FilePage(IUploadStorage uploadStorage, IUploadProcessor uploadProcessor) : Controller
{
    private readonly IUploadStorage uploadStorage = uploadStorage ?? throw new ArgumentNullException(nameof(uploadStorage));
    private readonly IUploadProcessor uploadProcessor = uploadProcessor ?? throw new ArgumentNullException(nameof(uploadProcessor));

    [Route("upload/{*pathInfo}")]
    public IActionResult Read(string pathInfo,
        [FromServices] IUploadFileResponder responder)
    {
        return responder.Read(pathInfo, Response.Headers);
    }

    [Route("File/TemporaryUpload")]
    [AcceptVerbs("POST")]
    public ActionResult TemporaryUpload()
    {
        var response = this.ExecuteMethod(() => HandleUploadRequest(HttpContext));

        if (!((string)Request.Headers.Accept ?? "").Contains("json", StringComparison.Ordinal))
            response.ContentType = "text/plain";

        return response;
    }

    [Route("File/TemporaryUploadCK")]
    [AcceptVerbs("POST"), IgnoreAntiforgeryToken]
    public ActionResult TemporaryUploadCK()
    {
        var response = (UploadResponse)HandleUploadRequest(HttpContext);
        if (response.Error != null)
            return new JsonResult(new
            {
                uploaded = 0,
                error = new
                {
                    message = response.Error.Message
                }
            });

        return new JsonResult(new
        {
            uploaded = 1,
            fileName = response.TemporaryFile,
            url = VirtualPathUtility.ToAbsolute(HttpContext,
                uploadStorage.GetFileUrl(response.TemporaryFile))
        });
    }

    [NonAction]
    private UploadResponse HandleUploadRequest(HttpContext context)
    {
        if (context.Request.Form.Files.Count != 1)
            throw ArgumentExceptions.OutOfRange(context.Request.Form.Files);

        var file = context.Request.Form.Files[0];
        ArgumentException.ThrowIfNullOrEmpty(file?.FileName);

        IUploadOptions uploadOptions = new UploadOptions
        {
            ThumbWidth = 128,
            ThumbHeight = 96,
            ThumbMode = ImageScaleMode.PreserveRatioNoFill
        };

        var uploadIntent = Request.Query["uploadIntent"];
        if (!string.IsNullOrEmpty(uploadIntent))
        {
            // if desired modify uploadOptions here based on uploadIntent
        }

        var uploadInfo = uploadProcessor.Process(file.OpenReadStream(),
            file.FileName, uploadOptions);

        uploadStorage.SetOriginalName(uploadInfo.TemporaryFile, file.FileName);

        return new UploadResponse()
        {
            TemporaryFile = uploadInfo.TemporaryFile,
            Size = uploadInfo.FileSize,
            IsImage = uploadInfo.IsImage,
            Width = uploadInfo.ImageWidth,
            Height = uploadInfo.ImageHeight
        };
    }

    private class UploadResponse : ServiceResponse
    {
        public string TemporaryFile { get; set; }
        public long Size { get; set; }
        public bool IsImage { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
