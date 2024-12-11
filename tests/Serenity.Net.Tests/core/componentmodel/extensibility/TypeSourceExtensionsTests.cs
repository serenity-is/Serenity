namespace Serenity.Abstractions;

public class TypeSourceExtensionsTests
{
    [Fact]
    public void GetAssemblyAttributes_ThrowsArgumentNullException_ForNull()
    {
        ITypeSource typeSource = null;
        Assert.Throws<ArgumentNullException>(() => TypeSourceExtensions.GetAssemblyAttributes<Attribute>(typeSource));
    }

    [Fact]
    public void GetAssemblyAttributes_ReturnsAttributes_WhenTypeSourceIsValid()
    {
        var mockTypeSource = new MockTypeSource(new ObsoleteAttribute());
        var result = TypeSourceExtensions.GetAssemblyAttributes<ObsoleteAttribute>(mockTypeSource);
        Assert.Single(result);
        Assert.IsType<ObsoleteAttribute>(result.First());
    }
}
