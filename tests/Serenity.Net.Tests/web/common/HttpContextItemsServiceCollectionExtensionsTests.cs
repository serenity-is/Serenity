namespace Serenity.Extensions.DependencyInjection;

public class HttpContextItemsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddHttpContextItemsAccessor_Returns_Same_Collection()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddHttpContextItemsAccessor());
    }

    [Fact]
    public void AddHttpContextItemsAccessor_Registers_Accessor_And_ItemsAccessor()
    {
        var services = new ServiceCollection();
        services.AddHttpContextItemsAccessor();
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IHttpContextAccessor>());
        Assert.IsType<HttpContextItemsAccessor>(provider.GetRequiredService<IHttpContextItemsAccessor>());
    }

    [Fact]
    public void AddHttpContextItemsAccessor_Is_Idempotent()
    {
        var services = new ServiceCollection();
        services.AddHttpContextItemsAccessor();
        services.AddHttpContextItemsAccessor();

        Assert.Single(services, x => x.ServiceType == typeof(IHttpContextItemsAccessor));
    }
}
