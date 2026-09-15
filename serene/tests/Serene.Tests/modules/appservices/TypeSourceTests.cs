using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Serene;

public class TypeSourceTests
{
    [Fact]
    public void IncludesCorrectAssemblyList()
    {
        var typeSource = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            ApplicationName = typeof(Startup).Assembly.GetName().Name
        }).Services.AddApplicationPartsTypeSource();
        var assemblies = typeSource.GetAssemblies()
            .Select(x => x.GetName().Name);
        Assert.Collection(assemblies,
            x => Assert.Equal("Serenity.Net.Core", x),
            x => Assert.Equal("Serenity.Net.Services", x),
            x => Assert.Equal("Serenity.Net.Web", x),
            x => Assert.Equal("Serenity.Extensions", x),
            x => Assert.Equal("Serenity.Demo.Northwind", x),
            x => Assert.Equal("Serenity.Demo.BasicSamples", x),
            x => Assert.Equal("Serene.Web", x));
    }

    [Fact]
    public void RecoversApplicationPartsWhenAttributesAreMissing()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            ApplicationName = typeof(Startup).Assembly.GetName().Name
        });

        // Simulate a build where the generated MvcApplicationPartsAssemblyInfo.cs was not
        // compiled: the part manager contains only the entry assembly and the entry assembly
        // has no ApplicationPartAttribute (this test assembly is not a Web SDK app).
        var partManager = new ApplicationPartManager();
        partManager.ApplicationParts.Add(new AssemblyPart(typeof(TypeSourceTests).Assembly));

        var typeSource = builder.Services.AddApplicationPartsTypeSource(partManager);
        var names = typeSource.GetAssemblies().Select(x => x.GetName().Name).ToList();

        Assert.Contains("Serenity.Net.Web", names);
        Assert.Contains("Serenity.Extensions", names);
        Assert.Contains("Serenity.Demo.Northwind", names);
        Assert.Contains("Serenity.Demo.BasicSamples", names);
        Assert.Contains("Serene.Web", names);

        // it should also repair the part manager itself
        Assert.Contains(partManager.ApplicationParts.OfType<AssemblyPart>(),
            x => x.Assembly.GetName().Name == "Serenity.Net.Web");

        // assemblies that don't reference MVC (and thus are not normally application parts)
        // should not be added to the part manager
        Assert.DoesNotContain(partManager.ApplicationParts.OfType<AssemblyPart>(),
            x => x.Assembly.GetName().Name == "Serenity.Net.Core");
        Assert.DoesNotContain(partManager.ApplicationParts.OfType<AssemblyPart>(),
            x => x.Assembly.GetName().Name == "Serenity.Net.Services");
    }

    [Fact]
    public void DoesNotRecoverApplicationPartsWhenDisabled()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            ApplicationName = typeof(Startup).Assembly.GetName().Name
        });

        var partManager = new ApplicationPartManager();
        partManager.ApplicationParts.Add(new AssemblyPart(typeof(TypeSourceTests).Assembly));

        var typeSource = builder.Services.AddApplicationPartsTypeSource(partManager, tryPartRecovery: false);
        var names = typeSource.GetAssemblies().Select(x => x.GetName().Name).ToList();

        Assert.DoesNotContain("Serenity.Extensions", names);
        Assert.DoesNotContain("Serenity.Demo.Northwind", names);
        Assert.DoesNotContain("Serenity.Demo.BasicSamples", names);

        // the part manager should be left untouched
        Assert.DoesNotContain(partManager.ApplicationParts.OfType<AssemblyPart>(),
            x => x.Assembly.GetName().Name == "Serenity.Net.Web");
    }

    [Fact]
    public void RecoversApplicationPartsConcurrently()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            ApplicationName = typeof(Startup).Assembly.GetName().Name
        });

        var partManager = new ApplicationPartManager();
        partManager.ApplicationParts.Add(new AssemblyPart(typeof(TypeSourceTests).Assembly));

        var typeSource = builder.Services.AddApplicationPartsTypeSource(partManager);

        var results = new System.Collections.Concurrent.ConcurrentBag<List<string>>();
        Parallel.For(0, 16, _ =>
        {
            results.Add(typeSource.GetAssemblies().Select(x => x.GetName().Name!).ToList());
        });

        Assert.Equal(16, results.Count);
        Assert.All(results, names =>
        {
            Assert.Contains("Serenity.Net.Web", names);
            Assert.Contains("Serenity.Extensions", names);
            Assert.Contains("Serenity.Demo.Northwind", names);
            Assert.Contains("Serenity.Demo.BasicSamples", names);
            Assert.Contains("Serene.Web", names);
        });
    }
}
