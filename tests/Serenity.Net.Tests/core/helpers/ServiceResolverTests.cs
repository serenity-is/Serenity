namespace Serenity.Extensions.DependencyInjection;

public class ServiceResolverTests
{
    private interface ITestService
    {
    }

    private class TestService : ITestService
    {
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenServiceProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceResolver<ITestService>(null));
    }

    [Fact]
    public void Resolve_ReturnsRegisteredService()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITestService, TestService>();
        var provider = services.BuildServiceProvider();

        var resolver = new ServiceResolver<ITestService>(provider);
        var resolved = resolver.Resolve();

        Assert.IsType<TestService>(resolved);
    }

    [Fact]
    public void Resolve_Throws_WhenServiceNotRegistered()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        var resolver = new ServiceResolver<ITestService>(provider);

        Assert.Throws<InvalidOperationException>(() => resolver.Resolve());
    }
}
