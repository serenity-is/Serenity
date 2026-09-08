namespace Serenity;

#pragma warning disable CS0612 // Type or member is obsolete
public class ReflectionExtensionsTests
{
    [AttributeUsage(AttributeTargets.All)]
    private class TestAttribute : Attribute
    {
    }

    private class TestClass
    {
        [Test]
        public string Name { get; set; }
    }

    [Fact]
    public void GetAttribute_ReturnsAttribute_WhenPresent()
    {
        var member = typeof(TestClass).GetProperty(nameof(TestClass.Name));
        var attr = member.GetAttribute<TestAttribute>();
        Assert.NotNull(attr);
    }

    [Fact]
    public void GetAttribute_ReturnsNull_WhenAbsent()
    {
        var member = typeof(TestClass).GetProperty(nameof(TestClass.Name));
        var attr = member.GetAttribute<ObsoleteAttribute>();
        Assert.Null(attr);
    }
}
#pragma warning restore CS0612 // Type or member is obsolete
