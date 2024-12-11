namespace Serenity.ComponentModel;

public class PasswordEditorAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Password()
    {
        var attribute = new PasswordEditorAttribute();
        Assert.Equal("Password", attribute.EditorType);
    }
}