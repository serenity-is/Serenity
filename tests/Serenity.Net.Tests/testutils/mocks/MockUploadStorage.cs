namespace Serenity.TestUtils;

public class MockUploadStorage(
    DiskUploadStorageOptions options = null,
    MockDiskUploadFileSystem fs = null)
    : DiskUploadStorage(options ?? new DiskUploadStorageOptions
    {
        RootPath = "/",
        RootUrl = "/upload/"
    }, fs ?? new MockDiskUploadFileSystem())
{
    public MockDiskUploadFileSystem MockFileSystem => (MockDiskUploadFileSystem)fileSystem;
}