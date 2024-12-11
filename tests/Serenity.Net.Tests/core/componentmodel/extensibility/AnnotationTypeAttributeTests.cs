namespace Serenity.ComponentModel;

public class AnnonationTypeAttributeTests()
{
    public class MyAnnonatedType
    {
    }

    [Fact]
    public void AttributeUsage_IsClassOnly()
    {
        var attributeUsage = typeof(AnnotationTypeAttribute)
            .GetCustomAttributes<AttributeUsageAttribute>(true)
            .FirstOrDefault();

        Assert.NotNull(attributeUsage);
        Assert.True(attributeUsage.ValidOn.HasFlag(AttributeTargets.Class));
    }

    [Fact]
    public void AnnotatedType_CanBePassedAs_AnyType()
    {
        var attribute = new AnnotationTypeAttribute(typeof(MyAnnonatedType));
        var attribute2 = new AnnotationTypeAttribute(typeof(string));
        var attribute3 = new AnnotationTypeAttribute(typeof(int));
        var attribute4 = new AnnotationTypeAttribute(typeof(bool));

        Assert.Equal(typeof(MyAnnonatedType), attribute.AnnotatedType);
        Assert.Equal(typeof(string), attribute2.AnnotatedType);
        Assert.Equal(typeof(int), attribute3.AnnotatedType);
        Assert.Equal(typeof(bool), attribute4.AnnotatedType);
    }

    [Fact]
    public void Constructor_ShouldThrow_ArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new AnnotationTypeAttribute(null));
    }

    [Fact]
    public void Inherited_CanBeSet_ToTrue()
    {
        var attribute = new AnnotationTypeAttribute(typeof(bool))
        {
            Inherited = true
        };
        Assert.True(attribute.Inherited);
    }

    [Fact]
    public void Inherited_CanBeSet_ToFalse()
    {
        var attribute = new AnnotationTypeAttribute(typeof(bool))
        {
            Inherited = false
        };
        Assert.False(attribute.Inherited);
    }

    [Fact]
    public void Namespaces_CanBeSet()
    {
        var attribute = new AnnotationTypeAttribute(typeof(string))
        {
            Namespaces = ["Namespaces1", "Namespaces2"]
        };
        Assert.Collection(attribute.Namespaces,
            x => Assert.Equal("Namespaces1", x),
            x => Assert.Equal("Namespaces2", x));
    }

    [Fact]
    public void Namespaces_IsNull_ByDefault()
    {
        var attribute = new AnnotationTypeAttribute(typeof(string));
        Assert.Null(attribute.Namespaces);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var attribute = new AnnotationTypeAttribute(typeof(string))
        {
            Properties = ["Properties1", "Properties2"]
        };
        Assert.Collection(attribute.Properties,
            x => Assert.Equal("Properties1", x),
            x => Assert.Equal("Properties2", x));
    }

    [Fact]
    public void Properties_IsNull_ByDefault()
    {
        var attribute = new AnnotationTypeAttribute(typeof(string));
        Assert.Null(attribute.Properties);
    }
}