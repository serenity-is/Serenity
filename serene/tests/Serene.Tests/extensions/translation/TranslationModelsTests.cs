namespace Serenity.Extensions;

public class TranslationModelsTests
{
    [Fact]
    public void TranslateTextRequest_Properties_Roundtrip()
    {
        var request = new TranslateTextRequest
        {
            SourceLanguageID = "en",
            Inputs = [new TranslateTextInput { TextKey = "k", TargetLanguageID = "tr", SourceText = "text" }]
        };

        Assert.Equal("en", request.SourceLanguageID);
        var input = Assert.Single(request.Inputs!);
        Assert.Equal("k", input.TextKey);
        Assert.Equal("tr", input.TargetLanguageID);
        Assert.Equal("text", input.SourceText);
    }

    [Fact]
    public void TranslateTextResponse_Properties_Roundtrip()
    {
        var response = new TranslateTextResponse
        {
            Translations = [new TranslateTextOutput { TextKey = "k", TargetLanguageID = "tr", TranslatedText = "ceviri" }]
        };

        var output = Assert.Single(response.Translations!);
        Assert.Equal("k", output.TextKey);
        Assert.Equal("tr", output.TargetLanguageID);
        Assert.Equal("ceviri", output.TranslatedText);
    }

    [Fact]
    public void TranslationItem_Properties_Roundtrip()
    {
        var item = new TranslationItem
        {
            Key = "k",
            SourceText = "s",
            TargetText = "t",
            CustomText = "c",
            HasTranslation = true,
            UserTranslated = true
        };

        Assert.Equal("k", item.Key);
        Assert.Equal("s", item.SourceText);
        Assert.Equal("t", item.TargetText);
        Assert.Equal("c", item.CustomText);
        Assert.True(item.HasTranslation);
        Assert.True(item.UserTranslated);
    }

    [Fact]
    public void TranslationListRequest_Properties_Roundtrip()
    {
        var request = new TranslationListRequest { SourceLanguageID = "en", TargetLanguageID = "tr" };
        Assert.Equal("en", request.SourceLanguageID);
        Assert.Equal("tr", request.TargetLanguageID);
    }

    [Fact]
    public void TranslationListResponse_KeysByAssembly_Is_Not_Null()
    {
        var response = new TranslationListResponse();
        Assert.NotNull(response.KeysByAssembly);
        response.KeysByAssembly["a"] = ["k"];
        Assert.Equal(["k"], response.KeysByAssembly["a"]);
    }

    [Fact]
    public void TranslationUpdateRequest_Properties_Roundtrip()
    {
        var request = new TranslationUpdateRequest
        {
            TargetLanguageID = "tr",
            Translations = new Dictionary<string, string> { ["k"] = "v" }
        };

        Assert.Equal("tr", request.TargetLanguageID);
        Assert.Equal("v", request.Translations!["k"]);
    }

    [Fact]
    public void TranslationUpdateResponse_Properties_Roundtrip()
    {
        var response = new TranslationUpdateResponse { SavedPath = "/p" };
        Assert.Equal("/p", response.SavedPath);
    }
}
