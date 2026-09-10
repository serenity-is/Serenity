namespace Serenity.Extensions.DependencyInjection;

public class ServiceCollectionExtensions_AddServiceHandlersTests
{
    private interface ICustomHandler : IRequestHandler
    {
    }

    private class CustomHandler : ICustomHandler
    {
    }

    [DefaultHandler]
    private class DefaultCustomHandler : ICustomHandler
    {
    }

    [DefaultHandler(false)]
    private class NonDefaultCustomHandler : ICustomHandler
    {
    }

    [Fact]
    public void AddServiceBehaviors_Registers_Defaults()
    {
        var collection = new ServiceCollection();
        var result = collection.AddServiceBehaviors();

        Assert.Same(collection, result);
        Assert.Contains(collection, x => x.ServiceType == typeof(IBehaviorFactory));
        Assert.Contains(collection, x => x.ServiceType == typeof(IImplicitBehaviorRegistry));
        Assert.Contains(collection, x => x.ServiceType == typeof(IBehaviorProvider));
    }

    [Fact]
    public void AddServiceHandlerFactory_Registers_Defaults()
    {
        var collection = new ServiceCollection();
        var result = collection.AddServiceHandlerFactory();

        Assert.Same(collection, result);
        Assert.Contains(collection, x => x.ServiceType == typeof(IHandlerActivator));
        Assert.Contains(collection, x => x.ServiceType == typeof(IDefaultHandlerRegistry));
        Assert.Contains(collection, x => x.ServiceType == typeof(IDefaultHandlerFactory));
    }

    [Fact]
    public void AddProxyRequestHandlers_Registers_Open_Generics()
    {
        var collection = new ServiceCollection();
        var result = collection.AddProxyRequestHandlers();

        Assert.Same(collection, result);
        Assert.Contains(collection, x => x.ServiceType == typeof(ICreateHandler<,,>));
        Assert.Contains(collection, x => x.ServiceType == typeof(IListHandlerAsync<,,>));
        Assert.Contains(collection, x => x.ServiceType == typeof(IRetrieveHandlerAsync<>));
    }

    [Fact]
    public void AddCustomRequestHandlers_Does_Not_Override_Existing_Registration()
    {
        var typeSource = new MockTypeSource(typeof(CustomHandler));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);
        var before = collection.Count;

        collection.AddCustomRequestHandlers(typeSource);

        Assert.Equal(before, collection.Count);
    }

    [Fact]
    public void AddCustomRequestHandlers_Uses_Handler_Without_DefaultFalse()
    {
        var typeSource = new MockTypeSource(typeof(CustomHandler), typeof(NonDefaultCustomHandler));
        var collection = new ServiceCollection();
        collection.AddCustomRequestHandlers(typeSource);

        var descriptor = Assert.Single(collection, x => x.ServiceType == typeof(ICustomHandler));
        Assert.Equal(typeof(CustomHandler), descriptor.ImplementationType);
    }

    [Fact]
    public void AddServiceHandlers_Registers_RequestContext()
    {
        var collection = new ServiceCollection();
        var result = collection.AddServiceHandlers(new MockTypeSource());

        Assert.Same(collection, result);
        Assert.Contains(collection, x => x.ServiceType == typeof(IRequestContext));
    }

    [Fact]
    public void AddBaseTexts_Provider_Returns_Registry()
    {
        var typeSource = new MockTypeSource();
        var services = new ServiceCollection();
        services.AddSingleton<ITypeSource>(typeSource);
        services.AddTextRegistry();
        services.AddSingleton<IRowTypeRegistry>(new DefaultRowTypeRegistry(typeSource));
        using var provider = services.BuildServiceProvider();

        var registry = provider.AddBaseTexts();

        Assert.NotNull(registry);
        Assert.Same(provider.GetRequiredService<ILocalTextRegistry>(), registry);
    }

    [Fact]
    public void AddBaseTexts_Registry_Returns_Registry()
    {
        var typeSource = new MockTypeSource();
        using var provider = BuildTextProvider(typeSource);
        var registry = provider.GetRequiredService<ILocalTextRegistry>();

        var result = registry.AddBaseTexts(typeSource);

        Assert.Same(registry, result);
    }

    [Fact]
    public void AddBaseTexts_Registry_Skips_Resources_When_False()
    {
        var typeSource = new MockTypeSource();
        using var provider = BuildTextProvider(typeSource);
        var registry = provider.GetRequiredService<ILocalTextRegistry>();

        var result = registry.AddBaseTexts(typeSource, includeResources: false);

        Assert.Same(registry, result);
    }

    private static ServiceProvider BuildTextProvider(ITypeSource typeSource)
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeSource);
        services.AddTextRegistry();
        services.AddSingleton<IRowTypeRegistry>(new DefaultRowTypeRegistry(typeSource));
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddJsonTexts_Throws_For_Nulls()
    {
        var registry = new MockLocalTextRegistry();
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(null, new PhysicalFileProvider(AppContext.BaseDirectory), ""));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(registry, null, ""));
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddJsonTexts(registry, new PhysicalFileProvider(AppContext.BaseDirectory), null));
    }

    [Fact]
    public void AddJsonTexts_Reads_Json_Files_Recursively()
    {
        var root = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "serenity-texts-" + Guid.NewGuid().ToString("N"));
        var sub = System.IO.Path.Combine(root, "sub");
        System.IO.Directory.CreateDirectory(sub);
        System.IO.File.WriteAllText(System.IO.Path.Combine(root, "en.json"), "{\"a\":\"A\"}");
        System.IO.File.WriteAllText(System.IO.Path.Combine(root, "ignore.txt"), "x");
        System.IO.File.WriteAllText(System.IO.Path.Combine(sub, "tr.json"), "{\"b\":\"B\"}");
        System.IO.File.WriteAllText(System.IO.Path.Combine(root, "bad.json"), " ");

        try
        {
            using var provider = new PhysicalFileProvider(root);
            var registry = new MockLocalTextRegistry();

            var result = registry.AddJsonTexts(provider, "");

            Assert.Same(registry, result);
        }
        finally
        {
            System.IO.Directory.Delete(root, true);
        }
    }

    [Fact]
    public void AddJsonTexts_Returns_When_Directory_Missing()
    {
        using var provider = new PhysicalFileProvider(AppContext.BaseDirectory);
        var registry = new MockLocalTextRegistry();

        var result = registry.AddJsonTexts(provider, "not-existing-folder-" + Guid.NewGuid().ToString("N"));

        Assert.Same(registry, result);
    }
}
