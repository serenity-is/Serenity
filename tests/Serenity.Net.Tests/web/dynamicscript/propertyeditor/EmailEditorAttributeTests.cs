namespace Serenity.ComponentModel;

public class EmailEditorAttributeTests
{
    [Fact]
    public void EditorType_Is_Email()
    {
        Assert.Equal("Email", new EmailEditorAttribute().EditorType);
    }

    [Fact]
    public void Domain_And_ReadOnlyDomain_Can_Be_Set()
    {
        var attribute = new EmailEditorAttribute
        {
            Domain = "example.com",
            ReadOnlyDomain = true
        };

        Assert.Equal("example.com", attribute.Domain);
        Assert.True(attribute.ReadOnlyDomain);
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("a.b+c@sub.example.co", true)]
    [InlineData("invalid", false)]
    [InlineData("a@@b", false)]
    public void EmailPattern_Validates(string email, bool expected)
    {
        Assert.Equal(expected, EmailEditorAttribute.EmailPattern.IsMatch(email));
    }

    [Fact]
    public void Validate_Returns_Null_For_Null_Value()
    {
        var attribute = new EmailEditorAttribute();
        Assert.Null(attribute.Validate(new MockValidationContext(null)));
    }

    [Fact]
    public void Validate_Returns_Null_For_Valid_Email()
    {
        var attribute = new EmailEditorAttribute();
        Assert.Null(attribute.Validate(new MockValidationContext("user@example.com")));
    }

    [Fact]
    public void Validate_Returns_Error_For_Invalid_Email()
    {
        var attribute = new EmailEditorAttribute();
        var result = attribute.Validate(new MockValidationContext("invalid"));

        Assert.NotNull(result);
    }
}
