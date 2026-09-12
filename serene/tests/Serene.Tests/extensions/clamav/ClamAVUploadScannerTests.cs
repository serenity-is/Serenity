namespace Serenity.Extensions;

public class ClamAVUploadScannerTests
{
    [Fact]
    public void Constructor_Throws_For_Null_Options()
    {
        Assert.Throws<ArgumentNullException>(() => new ClamAVUploadScanner(null!));
    }

    [Fact]
    public void Scan_Returns_When_Disabled()
    {
        var scanner = new ClamAVUploadScanner(new MockOptionsMonitor<ClamAVSettings>(
            new ClamAVSettings { Enabled = false }));
        using var stream = new System.IO.MemoryStream([1, 2, 3]);
        scanner.Scan(stream, "file.txt");
    }

    [Fact]
    public void Scan_Throws_FailedScan_When_Connection_Fails()
    {
        var scanner = new ClamAVUploadScanner(new MockOptionsMonitor<ClamAVSettings>(
            new ClamAVSettings { Enabled = true, Host = "127.0.0.1", Port = 1 }),
            NullTextLocalizer.Instance, new MockLogger<ClamAVUploadScanner>());

        using var stream = new System.IO.MemoryStream([1, 2, 3]);
        var error = Assert.Throws<ValidationError>(() => scanner.Scan(stream, "file.txt"));
        Assert.Equal("FailedScan", error.ErrorCode);
    }
}

