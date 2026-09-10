namespace Serenity.Reflection;

public class AnnotationTypeExtensionsTests
{
    private class DisplayAttribute : Attribute
    {
        public DisplayAttribute(string name) { Name = name; }
        public string Name { get; }
    }

    private class EditAttribute : Attribute
    {
    }

    private class TargetClass
    {
        [Display("Original")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int Age { get; set; }
    }

    private class AnnotationClass
    {
        [Display("Annotated")]
        public string Name { get; set; }

        [Display("Annotated Description")]
        public string Description { get; set; }

        [Edit]
        public string Name2 { get; set; }
    }

    [Fact]
    public void GetAnnotatedProperty_ReturnsWrappedProperty_WhenNoAnnotation()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Age));

        var result = annotated.GetAnnotatedProperty(property);

        Assert.Equal("Age", result.Name);
        Assert.Equal(typeof(int), result.PropertyType);
        Assert.Null(result.GetAttribute<DisplayAttribute>());
    }

    [Fact]
    public void GetAnnotatedProperty_ReturnsPropertyAttribute_WhenPresent()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Name));

        var result = annotated.GetAnnotatedProperty(property);

        Assert.Equal("Name", result.Name);
        var attr = result.GetAttribute<DisplayAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("Original", attr.Name);
    }

    [Fact]
    public void GetAnnotatedProperty_ReturnsAnnotationAttribute_WhenPropertyHasNone()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Name));

        var result = annotated.GetAnnotatedProperty(property);

        // The annotation class has [Edit] on Name2, not Name, so no Edit attribute should be found.
        Assert.Null(result.GetAttribute<EditAttribute>());
    }

    [Fact]
    public void GetAnnotatedProperty_ReturnsAnnotationAttribute_FromMatchingAnnotationProperty()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Name));

        var result = annotated.GetAnnotatedProperty(property);

        // AnnotationClass.Name has [Display("Annotated")]; since TargetClass.Name also has
        // [Display("Original")], the property's own attribute wins.
        var attr = result.GetAttribute<DisplayAttribute>();
        Assert.Equal("Original", attr.Name);
    }

    [Fact]
    public void GetAnnotatedProperty_AnnotationAttribute_WhenOriginIsAnnotationOnly()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Description));

        var result = annotated.GetAnnotatedProperty(property);

        // TargetClass.Description has no [Display], so the annotation's attribute is used.
        var attr = result.GetAttribute<DisplayAttribute>(AttributeOrigin.Annotation);
        Assert.NotNull(attr);
        Assert.Equal("Annotated Description", attr.Name);
    }

    [Fact]
    public void GetAttributes_ReturnsAllMatchingAttributes()
    {
        var annotated = new[] { typeof(AnnotationClass) }.GetAnnotatedType();
        var property = typeof(TargetClass).GetProperty(nameof(TargetClass.Name));

        var result = annotated.GetAnnotatedProperty(property);
        var attrs = result.GetAttributes<DisplayAttribute>(AttributeOrigin.All).ToList();

        Assert.Equal(2, attrs.Count);
    }
}
