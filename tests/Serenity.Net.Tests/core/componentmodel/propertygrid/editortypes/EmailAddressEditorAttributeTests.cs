namespace Serenity.ComponentModel;

public class EmailAdressEditorAttributeTests
{
    [Fact]
    public void EditorType_CanBePassed_EmailAddress()
    {
        var attribute = new EmailAddressEditorAttribute();
        Assert.Equal("EmailAddress", attribute.EditorType);
    }
}
