using System.IO;

namespace Serenity.Web;

public interface IUploadValidator
{
    void ValidateFile(IUploadFileConstraints constraints, Stream stream, 
        string filename, out bool isImageExtension);

    void ValidateImage(IUploadImageContrains constraints, Stream stream, 
        string filename, out object image);
}