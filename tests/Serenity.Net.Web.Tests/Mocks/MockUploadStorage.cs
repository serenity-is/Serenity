namespace Serenity.Tests;

public class MockUploadStorage : DiskUploadStorage
{
    public MockDiskUploadFileSystem MockFileSystem { get; }
    
    public MockUploadStorage(DiskUploadStorageOptions options,
        MockDiskUploadFileSystem fileSystem = null) :
        base(options, fileSystem)
    {
        MockFileSystem = fileSystem;
    }

    public static IUploadStorage Create()
    {
        return new MockUploadStorage(new DiskUploadStorageOptions
        {
            RootPath = "C:/",
            RootUrl = "/upload/"
        }, new MockDiskUploadFileSystem(null, "C:/"));
    }
}