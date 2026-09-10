namespace Serenity.ComponentModel;

public class RegisterTransientAttributeTests
{
    [Fact]
    public void Ctor_Default_SetsTransientLifetime()
    {
        var attr = new RegisterTransientAttribute();
        Assert.Equal(ServiceLifetime.Transient, attr.Lifetime);
    }

    [Fact]
    public void Ctor_WithTypes_SetsTypes()
    {
        var attr = new RegisterTransientAttribute(typeof(IService), typeof(IService2));
        Assert.Equal([typeof(IService), typeof(IService2)], attr.Types);
    }

    [Fact]
    public void Ctor_WithNullTypes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RegisterTransientAttribute(null));
    }

    private interface IService { }
    private interface IService2 { }
}

public class RegisterScopedAttributeTests
{
    [Fact]
    public void Ctor_Default_SetsScopedLifetime()
    {
        var attr = new RegisterScopedAttribute();
        Assert.Equal(ServiceLifetime.Scoped, attr.Lifetime);
    }

    [Fact]
    public void Ctor_WithTypes_SetsTypes()
    {
        var attr = new RegisterScopedAttribute(typeof(IService), typeof(IService2));
        Assert.Equal([typeof(IService), typeof(IService2)], attr.Types);
    }

    [Fact]
    public void Ctor_WithNullTypes_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RegisterScopedAttribute(null));
    }

    private interface IService { }
    private interface IService2 { }
}