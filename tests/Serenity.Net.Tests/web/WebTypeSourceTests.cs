namespace Serenity.Web;

public class WebTypeSourceTests
{
    [Fact]
    public void IncludesExtensionsAssemblyChain()
    {
        var typeSource = new WebTypeSource([]);
        var assemblies = typeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x));
    }

    [Fact]
    public void IncludesPassedAssembliesAtEnd()
    {
        var typeSource = new WebTypeSource([typeof(WebTypeSourceTests).Assembly]);
        var assemblies = typeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal(typeof(WebTypeSourceTests).Assembly.GetName().Name, x));
    }

    [Fact]
    public void CoreAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Core", WebTypeSource.SerenityNetCoreAssembly.GetName().Name);
    }

    [Fact]
    public void ServicesAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Services", WebTypeSource.SerenityNetServicesAssembly.GetName().Name);
    }

    [Fact]
    public void WebAssemblyIsCorrect()
    {
        Assert.Equal("Serenity.Net.Web", WebTypeSource.SerenityNetWebAssembly.GetName().Name);
    }
}