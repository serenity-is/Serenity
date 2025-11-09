namespace Serenity.ComponentModel;

public class TransformIgnoreAttributeTests
{
    [Fact]
    public void TransformIgnoreAttribute_ShouldInheritFromAttribute()
    {
        {
            var attribute = new TransformIgnoreAttribute();
            Assert.IsType<Attribute>(attribute, exactMatch: false);
        }
    }
}