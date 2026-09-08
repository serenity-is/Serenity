namespace Serenity.Reflection;

public class WrappedPropertyTests
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    private class DisplayAttribute : Attribute
    {
        public DisplayAttribute(string name) { Name = name; }
        public string Name { get; }
    }

    private class EditAttribute : Attribute
    {
    }

    private class IntrinsicAttribute : Attribute, IIntrinsicPropertyAttributeProvider
    {
        [Display("Intrinsic")]
        public object PropertyAttributes { get; set; }
    }

    private class TargetClass
    {
        [Display("Name")]
        public string Name { get; set; }

        [Display("A")]
        [Display("B")]
        public string Multiple { get; set; }

        [Intrinsic]
        public string Intrinsic { get; set; }

        public int NoAttributes { get; set; }
    }

    private static WrappedProperty GetProperty(string name)
    {
        return new WrappedProperty(typeof(TargetClass).GetProperty(name));
    }

    [Fact]
    public void Name_ReturnsPropertyName()
    {
        Assert.Equal("Name", GetProperty("Name").Name);
    }

    [Fact]
    public void PropertyType_ReturnsPropertyType()
    {
        Assert.Equal(typeof(string), GetProperty("Name").PropertyType);
        Assert.Equal(typeof(int), GetProperty("NoAttributes").PropertyType);
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenPresent()
    {
        var attr = GetProperty("Name").GetAttribute<DisplayAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("Name", attr.Name);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenAbsent()
    {
        Assert.Null(GetProperty("NoAttributes").GetAttribute<DisplayAttribute>());
    }

    [Fact]
    public void GetAttribute_ThrowsAmbiguousMatch_WhenMultiple()
    {
        Assert.Throws<AmbiguousMatchException>(() => GetProperty("Multiple").GetAttribute<DisplayAttribute>());
    }

    [Fact]
    public void GetAttributes_ReturnsAllMatching()
    {
        var attrs = GetProperty("Multiple").GetAttributes<DisplayAttribute>().ToList();
        Assert.Equal(2, attrs.Count);
    }

    [Fact]
    public void GetAttributes_ReturnsEmpty_WhenNone()
    {
        Assert.Empty(GetProperty("NoAttributes").GetAttributes<DisplayAttribute>());
    }

    [Fact]
    public void GetAttribute_ReturnsIntrinsicAttribute_FromProvider()
    {
        var attr = GetProperty("Intrinsic").GetAttribute<DisplayAttribute>(AttributeOrigin.Intrinsic);
        Assert.NotNull(attr);
        Assert.Equal("Intrinsic", attr.Name);
    }

    [Fact]
    public void GetAttribute_ReturnsIntrinsicAttribute_EvenWithExplicitOrigin()
    {
        // WrappedProperty always includes intrinsic attributes in its cached list,
        // regardless of the requested origin.
        var attr = GetProperty("Intrinsic").GetAttribute<DisplayAttribute>(AttributeOrigin.Explicit);
        Assert.NotNull(attr);
        Assert.Equal("Intrinsic", attr.Name);
    }
}
