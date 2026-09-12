namespace Serenity.Extensions.DependencyInjection;

public class ElevationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddElevationHandler_Throws_For_Null_Collection()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ElevationServiceCollectionExtensions.AddElevationHandler(null!));
    }

    [Fact]
    public void AddElevationHandler_Registers_Handler_As_Singleton()
    {
        var services = new ServiceCollection();
        var result = services.AddElevationHandler();

        Assert.Same(services, result);
        Assert.Contains(services, x =>
            x.ServiceType == typeof(IElevationHandler) &&
            x.ImplementationType == typeof(DefaultElevationHandler) &&
            x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddElevationHandler_Does_Not_Replace_Existing_Registration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IElevationHandler>(new NullElevationHandler());

        services.AddElevationHandler();

        Assert.DoesNotContain(services, x =>
            x.ServiceType == typeof(IElevationHandler) &&
            x.ImplementationType == typeof(DefaultElevationHandler));
    }

    private class NullElevationHandler : IElevationHandler
    {
        public void AppendElevationTokenToCookies()
        {
        }

        public void DeleteToken()
        {
        }

        public void ValidateElevationToken()
        {
        }
    }
}
