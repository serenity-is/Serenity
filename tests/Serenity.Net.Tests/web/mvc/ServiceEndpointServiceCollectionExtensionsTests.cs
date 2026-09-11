using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Options;

namespace Serenity.Extensions.DependencyInjection;

public class ServiceEndpointServiceCollectionExtensionsTests
{
    [Fact]
    public void AddServiceEndpointConventions_Registers_Application_Model_Provider()
    {
        var services = new ServiceCollection();

        services.AddServiceEndpointConventions();

        Assert.Contains(services, x =>
            x.ServiceType == typeof(IApplicationModelProvider) &&
            x.ImplementationType == typeof(ServiceEndpointApplicationModelProvider));
    }

    [Fact]
    public void AddServiceEndpointConventions_Adds_Binding_Metadata_Provider()
    {
        var services = new ServiceCollection();
        services.AddServiceEndpointConventions();

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>();

        Assert.Contains(options.Value.ModelMetadataDetailsProviders,
            x => x is ServiceEndpointBindingMetadataProvider);
    }

    [Fact]
    public void AddServiceEndpointConventions_Does_Not_Add_Duplicate_Metadata_Provider()
    {
        var services = new ServiceCollection();
        services.AddServiceEndpointConventions();
        services.AddServiceEndpointConventions();

        var options = services.BuildServiceProvider().GetRequiredService<IOptions<MvcOptions>>();

        Assert.Single(options.Value.ModelMetadataDetailsProviders,
            x => x is ServiceEndpointBindingMetadataProvider);
    }

    [Fact]
    public void AddServiceEndpointConventions_Throws_When_Services_Is_Null()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ((IServiceCollection)null!).AddServiceEndpointConventions());
    }
}
