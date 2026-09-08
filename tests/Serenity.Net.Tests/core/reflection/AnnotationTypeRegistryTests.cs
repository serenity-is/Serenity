using Serenity.ComponentModel;

namespace Serenity.Reflection;

public class AnnotationTypeRegistryTests
{
    private class TestAttribute : Attribute
    {
    }

    [AnnotationType(typeof(TestAttribute))]
    private class AttributeAnnotation
    {
    }

    [AnnotationType(typeof(ISomeInterface))]
    private class InterfaceAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass))]
    private class ClassAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Inherited = false)]
    private class ExactClassAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Namespaces = new[] { "Serenity.Reflection" })]
    private class NamespaceAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Namespaces = new[] { "Other.Namespace" })]
    private class WrongNamespaceAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Namespaces = new[] { "Serenity.*" })]
    private class WildcardNamespaceAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Properties = new[] { "Name" })]
    private class PropertyAnnotation
    {
    }

    [AnnotationType(typeof(SomeClass), Properties = new[] { "Missing" })]
    private class MissingPropertyAnnotation
    {
    }

    private interface ISomeInterface
    {
    }

    private class SomeClass : ISomeInterface
    {
        public string Name { get; set; }
    }

    private class SomeSubclass : SomeClass
    {
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException_WhenTypeSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new AnnotationTypeRegistry(null));
    }

    [Fact]
    public void GetAnnotationTypesFor_ReturnsEmpty_WhenNoAnnotations()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(Array.Empty<Type>()));
        Assert.Empty(registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByAttribute()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(AttributeAnnotation)));
        var result = registry.GetAnnotationTypesFor(typeof(SomeClassWithAttribute));
        Assert.Contains(typeof(AttributeAnnotation), result);
    }

    [Fact]
    public void GetAnnotationTypesFor_DoesNotMatchByAttribute_WhenAttributeMissing()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(AttributeAnnotation)));
        Assert.Empty(registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Test]
    private class SomeClassWithAttribute
    {
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByInterface()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(InterfaceAnnotation)));
        var result = registry.GetAnnotationTypesFor(typeof(SomeClass));
        Assert.Contains(typeof(InterfaceAnnotation), result);
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByAssignableType()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(ClassAnnotation)));
        var result = registry.GetAnnotationTypesFor(typeof(SomeSubclass));
        Assert.Contains(typeof(ClassAnnotation), result);
    }

    [Fact]
    public void GetAnnotationTypesFor_ExactMatch_DoesNotMatchSubclass()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(ExactClassAnnotation)));
        Assert.Empty(registry.GetAnnotationTypesFor(typeof(SomeSubclass)));
        Assert.Contains(typeof(ExactClassAnnotation), registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByNamespace()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(NamespaceAnnotation)));
        Assert.Contains(typeof(NamespaceAnnotation), registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_DoesNotMatchWrongNamespace()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(WrongNamespaceAnnotation)));
        Assert.Empty(registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByWildcardNamespace()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(WildcardNamespaceAnnotation)));
        Assert.Contains(typeof(WildcardNamespaceAnnotation), registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_MatchesByProperty()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(PropertyAnnotation)));
        Assert.Contains(typeof(PropertyAnnotation), registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }

    [Fact]
    public void GetAnnotationTypesFor_DoesNotMatchWhenPropertyMissing()
    {
        var registry = new AnnotationTypeRegistry(new MockTypeSource(typeof(MissingPropertyAnnotation)));
        Assert.Empty(registry.GetAnnotationTypesFor(typeof(SomeClass)));
    }
}
