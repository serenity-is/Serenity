namespace Serenity.Web;

/// <summary>
///   Thumbnail scaling modes</summary>
public enum ImageScaleMode
{
    /// <summary>
    ///   In this mode, thumbnail is generated just at the size requested.
    ///   If thumbnail width or height specified only, than thumbnail will have same aspect ratio,
    ///   otherwise if thumbnail and source is not at same ratio, thumbnail will have a 
    ///   narrowed or widened look.</summary>
    StretchToFit = 0,
    /// <summary>
    ///   Try to keep original aspect ratio of source image while generating the thumbnail.
    ///   If only width or height is set, it works same way with StretchToFit.
    ///   When both set, thumbnail width or height will be decreased suitably.</summary>
    PreserveRatioNoFill = 1,
    /// <summary>
    ///   It is logically same with PreserveRatioNoFill but this time instead of decreasing
    ///   thumbnail width or height, empty parts are filled.</summary>
    PreserveRatioWithFill = 2,
    /// <summary>
    ///   In this mode only central part of source image (horizontally or vertically) is taken to keep 
    ///   thumbnail at requested size if aspect ratio of source image is different.</summary>
    CropSourceImage = 3
}