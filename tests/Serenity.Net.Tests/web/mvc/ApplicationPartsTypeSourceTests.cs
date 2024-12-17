using Microsoft.AspNetCore.Builder;

namespace Serenity.Web;

public class ApplicationPartsTypeSourceTests
{
    [Fact]
    public void Includes_AssembliesWith_TypeSourceAssemblyAttribute()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            EnvironmentName = "Development",
            ApplicationName = GetType().Assembly.GetName().Name
        });

        builder.Services.AddApplicationPartsTypeSource();
        var typeSource = builder.Services.BuildServiceProvider().GetRequiredService<ITypeSource>();
        var appTypeSource = Assert.IsType<ApplicationPartsTypeSource>(typeSource);
        var assemblies = appTypeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal(GetType().Assembly.GetName().Name, x));
    }
}