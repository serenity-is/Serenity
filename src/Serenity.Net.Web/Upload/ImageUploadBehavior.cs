using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Obsolete subclass of <see cref="FileUploadBehavior"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="storage">Upload storage</param>
/// <param name="uploadProcessor">Upload processor</param>
[Obsolete("Use Serenity.Services.FileUploadBehavior")]
public abstract class ImageUploadBehavior(IUploadStorage storage, IUploadProcessor uploadProcessor) 
    : FileUploadBehavior(storage, uploadProcessor)
{
}