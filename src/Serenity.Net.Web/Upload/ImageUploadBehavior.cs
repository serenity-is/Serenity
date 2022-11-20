using Serenity.Web;

namespace Serenity.Services;

[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class ImageUploadBehavior : FileUploadBehavior
{
    public ImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage)
        : base(uploadValidator, imageProcessor, storage)
    {
    }
}