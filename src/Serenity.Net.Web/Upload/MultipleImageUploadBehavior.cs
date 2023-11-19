using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Obsolete subclass of the <see cref="MultipleFileUploadBehavior"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="uploadValidator">Upload validator</param>
/// <param name="imageProcessor">Image processor</param>
/// <param name="storage">Upload storage</param>
[Obsolete("Use Serenity.Services.MultipleFileUploadBehavior")]
public abstract class MultipleImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage) : MultipleFileUploadBehavior(uploadValidator, imageProcessor, storage)
{
}