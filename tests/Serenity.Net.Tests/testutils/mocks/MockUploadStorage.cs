namespace Serenity.Tests;

public class MockUploadStorage(DiskUploadStorageOptions options,
    MockDiskUploadFileSystem fileSystem) : DiskUploadStorage(options, fileSystem ?? throw new ArgumentNullException(nameof(fileSystem)))
{
    public MockDiskUploadFileSystem MockFileSystem { get; } = fileSystem;

    public static IUploadStorage Create()
    {
        return new MockUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "/",
            RootUrl = "/upload/"
        }, new MockDiskUploadFileSystem());
    }
}