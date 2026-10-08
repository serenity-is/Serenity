using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Serenity.Services;

public class DefaultHandlerRegistryTests
{
    private class TestHandler : IRequestHandler
    {
    }

    private class OtherHandler : IRequestHandler
    {
    }

    private class CountingChangeTypeSource(params Type[] types) : ITypeSource, IChangeTokenProvider, IDisposable
    {
        private CancellationTokenSource cts = new();

        public Type[] Types { get; set; } = types;
        public int ScanCount { get; private set; }
        public int ThrowCount { get; set; }

        public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType) => [];
        public IEnumerable<Type> GetTypes() => Types;
        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType) =>
            Types.Where(t => t.GetCustomAttributes(attributeType).Any());

        public IEnumerable<Type> GetTypesWithInterface(Type interfaceType)
        {
            ScanCount++;
            if (ThrowCount > 0)
            {
                ThrowCount--;
                throw new InvalidOperationException("scan failed");
            }
            return Types.Where(interfaceType.IsAssignableFrom);
        }

        public IChangeToken GetChangeToken() => new CancellationChangeToken(cts.Token);

        public void Fire()
        {
            var old = cts;
            cts = new CancellationTokenSource();
            old.Cancel();
            old.Dispose();
        }

        public void Dispose() => cts.Dispose();
    }

    [Fact]
    public void Constructor_Throws_For_Null_TypeSource()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultHandlerRegistry(null));
    }

    [Fact]
    public void GetTypes_Scans_Type_Source_Lazily_Once_And_Caches()
    {
        var typeSource = new CountingChangeTypeSource(typeof(TestHandler), typeof(OtherHandler));
        using var registry = new DefaultHandlerRegistry(typeSource);

        Assert.Equal(0, typeSource.ScanCount);
        Assert.Equal(2, registry.GetTypes().Count());
        Assert.Equal(2, registry.GetTypes().Count());
        Assert.Equal(1, typeSource.ScanCount);
    }

    [Fact]
    public void GetTypes_Rebuilds_On_ChangeToken()
    {
        var typeSource = new CountingChangeTypeSource(typeof(TestHandler));
        using var registry = new DefaultHandlerRegistry(typeSource);

        Assert.Single(registry.GetTypes());

        typeSource.Types = [typeof(TestHandler), typeof(OtherHandler)];
        typeSource.Fire();

        Assert.Equal(2, registry.GetTypes().Count());
    }

    [Fact]
    public void GetTypes_By_HandlerType_Filters_Cached_Types()
    {
        var typeSource = new CountingChangeTypeSource(typeof(TestHandler));
        using var registry = new DefaultHandlerRegistry(typeSource);

        Assert.Single(registry.GetTypes(typeof(object)));
        Assert.Empty(registry.GetTypes(typeof(IDisposable)));
    }

    [Fact]
    public void GetTypes_Retries_After_Failed_Scan()
    {
        var typeSource = new CountingChangeTypeSource(typeof(TestHandler)) { ThrowCount = 1 };
        using var registry = new DefaultHandlerRegistry(typeSource);

        Assert.Throws<InvalidOperationException>(() => registry.GetTypes());
        Assert.Single(registry.GetTypes());
    }
}
