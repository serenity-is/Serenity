using System.IO;

namespace Serenity.TestUtils;

public class MockUploadProcessor : IUploadProcessor
{
    private readonly IUploadStorage uploadStorage;

    public MockUploadProcessor(IUploadStorage uploadStorage = null)
    {
        this.uploadStorage = uploadStorage;
    }

    public ProcessedUploadInfo Process(Stream fileContent, string filename, IUploadOptions options)
    {
        if (filename?.StartsWith("temporary/") == true)
            return new ProcessedUploadInfo() { TemporaryFile = filename, IsImage = false };

        return new ProcessedUploadInfo() { TemporaryFile = "temporary/" +
            Guid.NewGuid().ToString("N") + Path.GetExtension(filename), IsImage = false };
    }
}