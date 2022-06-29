using Serenity.Web;

namespace Serenity.Net.Web.Upload;

[Obsolete("Use Serenity.Services.MultipleFileUploadBehavior")]
public abstract class MultipleImageUploadBehavior : MultipleFileUploadBehavior
{
    protected MultipleImageUploadBehavior(ITextLocalizer localizer, IUploadStorage storage, IExceptionLogger logger = null)
        : base(storage, localizer, logger)
    {
    }
}