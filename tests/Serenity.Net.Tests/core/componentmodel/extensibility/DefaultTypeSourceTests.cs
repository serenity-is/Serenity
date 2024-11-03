namespace Serenity.Tests.ComponentModel;

public class DefaultTypeSourceTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_ForNullAssemblies()
    {
        IEnumerable<Assembly> nullAssemblies = null;
        Assert.Throws<ArgumentNullException>(() => new DefaultTypeSource(nullAssemblies));
    }

    [Fact]
    public void GetAssemblies_ReturnsProvidedAssemblies()
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
        var typeSource = new DefaultTypeSource(assemblies);
        var result = typeSource.GetAssemblies();
        Assert.Equal(assemblies, result);
    }

    [Fact]
    public void GetAssemblyAttributes_ReturnsAttributesForGivenType()
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
        var typeSource = new DefaultTypeSource(assemblies);
        var attributeType = typeof(AssemblyTitleAttribute);
        var attributes = typeSource.GetAssemblyAttributes(attributeType);
        Assert.All(attributes, attr => Assert.IsType(attributeType, attr));
    }

    [Fact]
    public void GetTypes_ReturnsAllTypes()
    {
        var assemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
        var typeSource = new DefaultTypeSource(assemblies);
        var types = typeSource.GetTypes();
        Assert.Contains(typeof(DefaultTypeSourceTests), types);
    }
}

