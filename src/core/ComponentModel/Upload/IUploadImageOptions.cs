namespace Serenity.ComponentModel;

/// <summary>
/// Options for the uploaded image and thumbnails
/// </summary>
public interface IUploadImageOptions : IUploadOptions
{
    /// <summary>
    /// Quality of scaled image (not thumb).
    /// </summary>
    int ScaleQuality { get; }

    /// <summary>
    /// What width image should be scaled to. Default value of 0 disables it.
    /// </summary>
    public int ScaleWidth { get; }

    /// <summary>
    /// What height image should be scaled to. Default value of 0 disables it.
    /// </summary>
    public int ScaleHeight { get; }

    /// <summary>
    /// Should image be scaled up to requested size when its smaller
    /// </summary>
    public bool ScaleSmaller { get; }

    /// <summary>
    /// What kind of image scaling should be used to generate image.
    /// </summary>
    public Web.ImageScaleMode ScaleMode { get; }
    
    /// <summary>
    /// Background color to use when padding image
    /// </summary>
    public string? ScaleBackColor { get; set; }

    /// <summary>
    /// Thumbnail width for the default thumbnail. Default is 0, e.g. disabled.
    /// </summary>
    public int ThumbWidth { get; set; }

    /// <summary>
    /// Thumbnail height for the default thumbnail. Default is 0, e.g. disabled.
    /// </summary>
    public int ThumbHeight { get; set; }

    /// <summary>
    /// List of thumbnail sizes requested. Something like
    /// "96x96;128x128;200x200"
    /// </summary>
    public string? ThumbSizes { get; }

    /// <summary>
    /// What kind of image scaling should be used to generate thumbnails.
    /// </summary>
    public Web.ImageScaleMode ThumbMode { get; }

    /// <summary>
    /// Quality of thumbnails.
    /// </summary>
    public int ThumbQuality { get; }
    
    /// <summary>
    /// Background color to use when padding thumbnails
    /// </summary>
    public string? ThumbBackColor { get; set; }
}