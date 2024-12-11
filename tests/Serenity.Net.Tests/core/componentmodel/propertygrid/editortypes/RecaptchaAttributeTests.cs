namespace Serenity.ComponentModel;

public class RecaptchaAttributeTests
{
    [Fact]
    public void EditorType_ShouldBe_Recaptcha()
    {
        var attribute = new RecaptchaAttribute();
        Assert.Equal("Recaptcha", attribute.EditorType);
    }
}

