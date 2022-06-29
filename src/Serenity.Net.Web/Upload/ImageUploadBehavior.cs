using Serenity.Web;

namespace Serenity.Net.Web.Upload;

[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class ImageUploadBehavior : FileUploadBehavior
{
    protected ImageUploadBehavior(IUploadStorage storage, ITextLocalizer localizer, IExceptionLogger logger = null)
        : base(storage, localizer, logger)
    {
    }
}