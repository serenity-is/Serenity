using Serenity.Web;

namespace Serenity.Services;

[Obsolete("Use Serenity.Services.MultipleFileUploadBehavior")]
public abstract class MultipleImageUploadBehavior : MultipleFileUploadBehavior
{
    public MultipleImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage)
        : base(uploadValidator, imageProcessor, storage)
    {
    }
}