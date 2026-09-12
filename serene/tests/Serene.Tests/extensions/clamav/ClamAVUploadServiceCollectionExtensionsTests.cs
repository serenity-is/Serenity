namespace Serenity.Extensions.DependencyInjection;

public class ClamAVUploadServiceCollectionExtensionsTests
{
    [Fact]
    public void AddClamAVUploadScanner_Registers_Scanner_As_Singleton()
    {
        var services = new ServiceCollection();
        var result = services.AddClamAVUploadScanner();

        Assert.Same(services, result);
        Assert.Contains(services, x =>
            x.ServiceType == typeof(IUploadAVScanner) &&
            x.ImplementationType == typeof(ClamAVUploadScanner) &&
            x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddClamAVUploadScanner_Does_Not_Replace_Existing_Registration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IUploadAVScanner>(new MockUploadAVScanner());

        services.AddClamAVUploadScanner();

        Assert.DoesNotContain(services, x =>
            x.ServiceType == typeof(IUploadAVScanner) &&
            x.ImplementationType == typeof(ClamAVUploadScanner));
    }

    private class MockUploadAVScanner : IUploadAVScanner
    {
        public void Scan(System.IO.Stream stream, string filename)
        {
        }
    }
}
