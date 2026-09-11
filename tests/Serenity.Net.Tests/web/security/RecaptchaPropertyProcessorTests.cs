using Microsoft.Extensions.Options;

namespace Serenity.Web;

public class RecaptchaPropertyProcessorTests
{
    private static IOptions<RecaptchaSettings> Options(string? siteKey, string? secretKey)
    {
        return Microsoft.Extensions.Options.Options.Create(new RecaptchaSettings
        {
            SiteKey = siteKey,
            SecretKey = secretKey
        });
    }

    [Fact]
    public void Priority_Is_11()
    {
        Assert.Equal(11, new RecaptchaPropertyProcessor().Priority);
    }

    [Fact]
    public void Process_Ignores_Non_Recaptcha_Editor()
    {
        var item = new PropertyItem { EditorType = "String" };
        new RecaptchaPropertyProcessor(Options("site", null)).Process(null!, item);

        Assert.Equal("String", item.EditorType);
    }

    [Fact]
    public void Process_Ignores_When_SiteKey_Already_In_EditorParams()
    {
        var item = new PropertyItem
        {
            EditorType = RecaptchaAttribute.Key,
            EditorParams = new Dictionary<string, object?> { ["siteKey"] = "existing" }
        };
        new RecaptchaPropertyProcessor(Options("site", null)).Process(null!, item);

        Assert.Equal("existing", item.EditorParams["siteKey"]);
    }

    [Fact]
    public void Process_Hides_Editor_When_No_Keys_Configured()
    {
        var item = new PropertyItem { EditorType = RecaptchaAttribute.Key };
        new RecaptchaPropertyProcessor(Options(null, null)).Process(null!, item);

        Assert.True(item.HideOnInsert);
        Assert.True(item.HideOnUpdate);
        Assert.False(item.Visible);
        Assert.False(item.Required);
        Assert.Equal("String", item.EditorType);
    }

    [Fact]
    public void Process_Hides_Editor_When_Options_Is_Null()
    {
        var item = new PropertyItem { EditorType = RecaptchaAttribute.Key };
        new RecaptchaPropertyProcessor(null).Process(null!, item);

        Assert.Equal("String", item.EditorType);
    }

    [Fact]
    public void Process_Sets_SiteKey_EditorParam()
    {
        var item = new PropertyItem { EditorType = RecaptchaAttribute.Key };
        new RecaptchaPropertyProcessor(Options("site-key", null)).Process(null!, item);

        Assert.NotNull(item.EditorParams);
        Assert.Equal("site-key", item.EditorParams["siteKey"]);
        Assert.Equal(RecaptchaAttribute.Key, item.EditorType);
    }

    [Fact]
    public void Process_Sets_SiteKey_When_Only_SecretKey_Configured()
    {
        var item = new PropertyItem { EditorType = RecaptchaAttribute.Key };
        new RecaptchaPropertyProcessor(Options(null, "secret-key")).Process(null!, item);

        Assert.NotNull(item.EditorParams);
        Assert.Null(item.EditorParams["siteKey"]);
    }
}
