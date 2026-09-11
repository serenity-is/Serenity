namespace Serenity.Extensions.DependencyInjection;

public class EsBuildMinifierExtensionsTests
{
    [Fact]
    public void AddEsBuildCssMinifier_Registers_Css_Minifier()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddEsBuildCssMinifier());

        Assert.Contains(services, x => x.ServiceType == typeof(ICssMinifier));
    }

    [Fact]
    public void AddEsBuildScriptMinifier_Registers_Script_Minifier()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddEsBuildScriptMinifier());

        Assert.Contains(services, x => x.ServiceType == typeof(IScriptMinifier));
    }

    [Fact]
    public void AddEsBuildMinifiers_Registers_Both()
    {
        var services = new ServiceCollection();
        Assert.Same(services, services.AddEsBuildMinifiers());

        Assert.Contains(services, x => x.ServiceType == typeof(ICssMinifier));
        Assert.Contains(services, x => x.ServiceType == typeof(IScriptMinifier));
    }
}
