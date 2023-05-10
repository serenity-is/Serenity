using System.IO;

namespace Serenity.Tests;

public class MockUploadValidator : IUploadValidator
{
    public void ValidateFile(IUploadFileConstraints constraints, Stream stream, string filename, out bool isImageExtension)
    {
        isImageExtension = false;
    }

    public void ValidateImage(IUploadImageContrains constraints, Stream stream, string filename, out object image)
    {
        image = new object();
    }
}