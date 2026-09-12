namespace Serenity.Extensions;

public class ClientTypesAttributesTests
{
    [Fact]
    public void EnumSelectFormatterAttribute_Options_Roundtrip()
    {
        var attribute = new EnumSelectFormatterAttribute
        {
            AllowClear = true,
            EmptyItemText = "empty",
            EnumKey = "key"
        };

        Assert.True(attribute.AllowClear);
        Assert.Equal("empty", attribute.EmptyItemText);
        Assert.Equal("key", attribute.EnumKey);
    }

    [Fact]
    public void StaticTextBlockAttribute_Options_Roundtrip()
    {
        var attribute = new StaticTextBlockAttribute
        {
            HideLabel = true,
            IsHtml = true,
            IsLocalText = true,
            Text = "text"
        };

        Assert.True(attribute.HideLabel);
        Assert.True(attribute.IsHtml);
        Assert.True(attribute.IsLocalText);
        Assert.Equal("text", attribute.Text);
    }

    [Fact]
    public void SingleLineTextFormatterAttribute_Can_Be_Created()
    {
        Assert.NotNull(new SingleLineTextFormatterAttribute());
    }

    [Fact]
    public void DeleteRowActionFormatterAttribute_Can_Be_Created()
    {
        Assert.NotNull(new DeleteRowActionFormatterAttribute());
    }

    [Fact]
    public void ResetPasswordOptions_Properties_Roundtrip()
    {
        var options = new ResetPasswordOptions
        {
            minPasswordLength = 8,
            token = "t"
        };

        Assert.Equal(8, options.minPasswordLength);
        Assert.Equal("t", options.token);
    }
}
