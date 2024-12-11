namespace Serenity.Abstractions;

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

    [Fact]
    public void Takes_Distinct_Over_Assemblies_If_Passed_An_Array()
    {
        var assemblies = new[]
        { 
            Assembly.GetExecutingAssembly(),
            Assembly.GetExecutingAssembly()
        };
        var typeSource = new DefaultTypeSource(assemblies);
        var assembly = Assert.Single(typeSource.GetAssemblies());
        Assert.Equal(Assembly.GetExecutingAssembly(), assembly);
    }

    [Fact]
    public void Does_Not_Take_Distinct_Over_Assemblies_If_Not_Passed_An_Array()
    {
        var assemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetExecutingAssembly()
        };
        var typeSource = new DefaultTypeSource(assemblies);
        Assert.Collection(typeSource.GetAssemblies(),
            a => Assert.Equal(Assembly.GetExecutingAssembly(), a),
            a => Assert.Equal(Assembly.GetExecutingAssembly(), a));
    }
}

