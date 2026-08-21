namespace Serenity.Web;

/// <summary>
/// Parameters for image encoding. Currently only contains a JPEG quality parameter.
/// </summary>
public class ImageEncoderParams
{
    /// <summary>
    /// Gets or sets the JPEG quality.
    /// </summary>
    public int? Quality { get; set; }
}