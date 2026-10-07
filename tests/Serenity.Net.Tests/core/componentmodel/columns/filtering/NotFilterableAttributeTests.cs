namespace Serenity.ComponentModel;

public class NotFilterableAttributeTests
{
    [Fact]
    public void Value_IsFalse()
    {
        Assert.False(new NotFilterableAttribute().Value);
    }

    [Fact]
    public void DerivesFromFilterableAttribute()
    {
        Assert.IsAssignableFrom<FilterableAttribute>(new NotFilterableAttribute());
    }
}
