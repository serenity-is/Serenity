using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Obsolete subclass of <see cref="FileUploadBehavior"/>
/// </summary>
[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class ImageUploadBehavior : FileUploadBehavior
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="uploadValidator">Upload validator</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="storage">Upload storage</param>
    public ImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage)
        : base(uploadValidator, imageProcessor, storage)
    {
    }
}