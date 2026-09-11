using Microsoft.Extensions.Options;

namespace Serenity.Extensions.DependencyInjection;

public class UploadServiceCollectionExtensionsTests
{
    [Fact]
    public void AddUploadStorage_Registers_Default_Services()
    {
        var services = new ServiceCollection();
        services.AddUploadStorage();

        Assert.Contains(services, x => x.ServiceType == typeof(IFilenameFormatSanitizer));
        Assert.Contains(services, x => x.ServiceType == typeof(IUploadStorage));
        Assert.Contains(services, x => x.ServiceType == typeof(IUploadValidator));
        Assert.Contains(services, x => x.ServiceType == typeof(IImageProcessor));
        Assert.Contains(services, x => x.ServiceType == typeof(IUploadProcessor));
        Assert.Contains(services, x => x.ServiceType == typeof(IUploadFileResponder));
    }

    [Fact]
    public void AddUploadStorage_Returns_Same_Collection()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddUploadStorage());
    }

    [Fact]
    public void AddUploadStorage_With_SetupAction_Throws_When_Collection_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddUploadStorage(_ => { }));
    }

    [Fact]
    public void AddUploadStorage_With_SetupAction_Throws_When_SetupAction_Is_Null()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => services.AddUploadStorage(null!));
    }

    [Fact]
    public void AddUploadStorage_With_SetupAction_Configures_Options()
    {
        var services = new ServiceCollection();
        services.AddUploadStorage(options => options.Path = "/custom/upload");

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<UploadSettings>>();
        Assert.Equal("/custom/upload", options.Value.Path);
    }
}
