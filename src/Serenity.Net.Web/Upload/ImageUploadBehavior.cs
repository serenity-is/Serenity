using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Obsolete subclass of <see cref="FileUploadBehavior"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="uploadValidator">Upload validator</param>
/// <param name="imageProcessor">Image processor</param>
/// <param name="storage">Upload storage</param>
[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class ImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage) : FileUploadBehavior(uploadValidator, imageProcessor, storage)
{
}