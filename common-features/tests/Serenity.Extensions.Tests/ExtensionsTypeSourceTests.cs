using Serenity.Extensions;

namespace Serenity.Tests;

public class ExtensionsTypeSourceTests
{
    [Fact]
    public void IncludesExtensionsAssemblyChain()
    {
        var typeSource = new ExtensionsTypeSource([]);
        var assemblies = typeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Data", x),
            x => Assert.Equal("Serenity.Net.Entity", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal("Serenity.Extensions", x));
    }

    [Fact]
    public void IncludesPassedAssembliesAtEnd()
    {
        var typeSource = new ExtensionsTypeSource([typeof(ExtensionsTypeSourceTests).Assembly]);
        var assemblies = typeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Data", x),
            x => Assert.Equal("Serenity.Net.Entity", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal("Serenity.Extensions", x),
            x => Assert.Equal(typeof(ExtensionsTypeSourceTests).Assembly.GetName().Name, x));
    }

    [Fact]
    public void ExtensionsAssemblyChainIsCorrect()
    {
        Assert.Collection(ExtensionsTypeSource.SerenityExtensionsAssemblyChain
            .Select(x => x.GetName().Name),
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Data", x),
            x => Assert.Equal("Serenity.Net.Entity", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal("Serenity.Extensions", x));
    }

    [Fact]
    public void CoreAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Core", ExtensionsTypeSource.SerenityNetCoreAssembly.GetName().Name);
    }

    [Fact]
    public void DataAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Data", ExtensionsTypeSource.SerenityNetDataAssembly.GetName().Name);
    }

    [Fact]
    public void EntityAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Entity", ExtensionsTypeSource.SerenityNetEntityAssembly.GetName().Name);
    }

    [Fact]
    public void ServicesAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Services", ExtensionsTypeSource.SerenityNetServicesAssembly.GetName().Name);
    }

    [Fact]
    public void WebAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Web", ExtensionsTypeSource.SerenityNetWebAssembly.GetName().Name);
    }

    [Fact]
    public void ExtensionsAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Extensions", ExtensionsTypeSource.SerenityExtensionsAssembly.GetName().Name);
    }
}