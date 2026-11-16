using System.ComponentModel.DataAnnotations;

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
    public void Implies_The_Email_Rule()
    {
        var rule = Assert.Single(new EmailEditorAttribute().GetImpliedValidation());
        Assert.IsType<ValidateEmailAttribute>(rule);
        Assert.Same(ValidateEmailAttribute.Pattern, EmailEditorAttribute.EmailPattern);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("user@example.com", true)]
    [InlineData("invalid", false)]
    public void Implied_Rule_Validates_The_Value(string? value, bool valid)
    {
        var rule = Assert.Single(ValidationRules.GetRules([new EmailEditorAttribute()]));
        var context = new ValidationContext(new object()) { MemberName = "Email" };
        Assert.Equal(valid, ValidationRules.Validate(rule, value, context) is null);
    }
}
