namespace Serenity;

public class DefaultSectionKeyAttributeTests
{
    [Fact]
    public void SectionKey_CanBeSet_ViaConstructor()
    {
        var attribute = new DefaultSectionKeyAttribute("MySection");
        Assert.Equal("MySection", attribute.SectionKey);
    }

    [Fact]
    public void SectionKey_ShouldThrow_ArgumentNullException_ForNull()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultSectionKeyAttribute(null));
    }
}