namespace Serenity.Reflection;

public class WrappedPropertyTests
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    private class DisplayAttribute(string name) : Attribute
    {
        public string Name { get; } = name;
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

    private class BaseTargetClass
    {
        [Display("Base")]
        public virtual string Inherited { get; set; }
    }

    private class DerivedTargetClass : BaseTargetClass
    {
        public override string Inherited { get; set; }
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
    public void GetAttribute_DoesNotReturnIntrinsicAttribute_WithExplicitOrigin()
    {
        // intrinsic provider attributes are only included when the Intrinsic bit is requested.
        Assert.Null(GetProperty("Intrinsic").GetAttribute<DisplayAttribute>(AttributeOrigin.Explicit));
    }

    [Fact]
    public void Inherit_Origin_Does_Not_Poison_Explicit_Cache()
    {
        var wrapped = new WrappedProperty(typeof(DerivedTargetClass)
            .GetProperty(nameof(DerivedTargetClass.Inherited)));

        var inherited = wrapped.GetAttribute<DisplayAttribute>(AttributeOrigin.Inherit);
        Assert.NotNull(inherited);
        Assert.Equal("Base", inherited.Name);

        // populating the inherit cache must not leak inherited attributes
        // into the explicit-only origin
        Assert.Null(wrapped.GetAttribute<DisplayAttribute>(AttributeOrigin.Explicit));
    }
}
