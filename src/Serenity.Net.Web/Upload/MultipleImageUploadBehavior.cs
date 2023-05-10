using Serenity.Web;

namespace Serenity.Services;

/// <summary>
/// Obsolete subclass of the <see cref="MultipleFileUploadBehavior"/>
/// </summary>
[Obsolete("Use Serenity.Services.MultipleFileUploadBehavior")]
public abstract class MultipleImageUploadBehavior : MultipleFileUploadBehavior
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="uploadValidator">Upload validator</param>
    /// <param name="imageProcessor">Image processor</param>
    /// <param name="storage">Upload storage</param>
    public MultipleImageUploadBehavior(IUploadValidator uploadValidator, IImageProcessor imageProcessor, IUploadStorage storage)
        : base(uploadValidator, imageProcessor, storage)
    {
    }
}