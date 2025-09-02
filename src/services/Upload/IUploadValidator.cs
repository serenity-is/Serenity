using System.IO;

namespace Serenity.Web;

/// <summary>
/// An upload validator abstraction for checking <see cref="IUploadFileConstraints"/>
/// and <see cref="IUploadImageContrains"/>
/// </summary>
public interface IUploadValidator
{
    /// <summary>
    /// Validates file constraints
    /// </summary>
    /// <param name="constraints">The file constraints</param>
    /// <param name="stream">Source stream</param>
    /// <param name="filename">File name</param>
    /// <param name="isImageExtension">Returns true if the file extension matches
    /// with constraints.ImageExtensions.</param>
    void ValidateFile(IUploadFileConstraints constraints, Stream stream, 
        string filename, out bool isImageExtension);

    /// <summary>
    /// Validates image constraints
    /// </summary>
    /// <param name="constraints">The image constraints</param>
    /// <param name="stream">Source stream</param>
    /// <param name="filename">File name</param>
    /// <param name="image">Image object</param>
    void ValidateImage(IUploadImageContrains constraints, Stream stream, 
        string filename, out object image);
}