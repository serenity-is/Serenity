namespace Serenity.ComponentModel;

public class GenerateInterfaceAttributeTests
{
    [Fact]
    public void RequireFeatures_GetSet_Works()
    {
        var attr = new GenerateInterfaceAttribute { RequireFeatures = ["Feature1"] };
        Assert.Equal(new[] { "Feature1" }, attr.RequireFeatures);
    }

    [Fact]
    public void RequireFeatures_DefaultsToNull()
    {
        var attr = new GenerateInterfaceAttribute();
        Assert.Null(attr.RequireFeatures);
    }
}

public class TypeSourceAssemblyAttributeTests
{
    [Fact]
    public void Ctor_Works()
    {
        var attr = new TypeSourceAssemblyAttribute();
        Assert.NotNull(attr);
    }
}