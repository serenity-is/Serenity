namespace Serenity.Tests;

public class MockUploadStorage : DiskUploadStorage
{
    public MockDiskUploadFileSystem MockFileSystem { get; }
    
    public MockUploadStorage(DiskUploadStorageOptions options,
        MockDiskUploadFileSystem fileSystem) :
        base(options, fileSystem ?? throw new ArgumentNullException(nameof(fileSystem)))
    {
        MockFileSystem = fileSystem;
    }

    public static IUploadStorage Create()
    {
        return new MockUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "/",
            RootUrl = "/upload/"
        }, new MockDiskUploadFileSystem());
    }
}