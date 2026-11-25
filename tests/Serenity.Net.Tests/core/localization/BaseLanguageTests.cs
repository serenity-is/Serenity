using Microsoft.Extensions.Options;
using Serenity.Web;

namespace Serenity.Localization;

public class BaseLanguageTests
{
    [NestedLocalTexts(Prefix = "BaseLanguageTests.")]
    public static class TextsOfThisAssembly
    {
        public static readonly LocalText Hello = "Merhaba";
    }

    // the texts of Serenity.Net.Core are written in English, see its LocalTextLanguageAttribute
    private const string EnglishKey = "Authorization.AccessDenied";
    private const string EnglishOnlyKey = "Authorization.NotLoggedIn";
    private const string EnglishText = "Authorization has been denied for this request!";

    private static LocalTextRegistry TurkishBase()
    {
        var registry = new LocalTextRegistry { BaseLanguageID = "tr" };
        registry.AddNestedTexts(new MockTypeSource(typeof(CoreTexts)));
        // the Turkish texts of a feature, e.g. from its texts.tr.json file
        registry.Add("tr", EnglishKey, "Erişim reddedildi!");
        return registry;
    }

    [Theory]
    [InlineData("en", "en", true)]
    [InlineData("en", "EN", true)]
    [InlineData("en", "en-US", true)]
    [InlineData("en-GB", "en", true)]
    [InlineData("en-GB", "en-US", false)]
    [InlineData("en", "eng", false)]
    [InlineData("en", "tr", false)]
    [InlineData("", null, true)]
    [InlineData("en", "", false)]
    public void IsSameLanguage_Compares_A_Language_With_Its_Regions(string languageID, string otherID, bool expected)
    {
        Assert.Equal(expected, LocalTextLanguages.IsSameLanguage(languageID, otherID));
    }

    [Fact]
    public void The_Texts_Of_The_Base_Language_Are_Stored_As_Invariant_Ones()
    {
        var registry = new LocalTextRegistry { BaseLanguageID = "tr" };
        registry.Add("tr", "Key", "Türkçe");

        Assert.True(registry.GetAllTexts(false).ContainsKey(new LanguageIdKeyPair("", "Key")));
        Assert.Equal("Türkçe", registry.TryGet("tr", "Key", false));
        Assert.Equal("Türkçe", registry.TryGet("tr-TR", "Key", false));
        Assert.Equal("Türkçe", registry.TryGet("de", "Key", false));
        // a regional language is not the base one
        registry.Add("tr-CY", "Key", "Kıbrıs");
        Assert.Equal("Kıbrıs", registry.TryGet("tr-CY", "Key", false));
        Assert.Equal("Türkçe", registry.TryGet("tr", "Key", false));
    }

    [Fact]
    public void Without_A_Base_Language_The_Texts_Stay_As_They_Were()
    {
        var registry = new LocalTextRegistry();
        registry.Add("en", "Key", "English");

        Assert.Null(registry.TryGet("de", "Key", false));
        Assert.Empty(registry.SourceLanguages);
    }

    [Fact]
    public void English_Code_Texts_Are_Invariant_Ones_When_The_Base_Language_Is_English_Or_Not_Set()
    {
        foreach (var baseLanguage in new[] { null, "en", "en-US" })
        {
            var registry = new LocalTextRegistry { BaseLanguageID = baseLanguage };
            registry.AddNestedTexts(new MockTypeSource(typeof(CoreTexts)));

            Assert.True(registry.GetAllTexts(false).ContainsKey(new LanguageIdKeyPair("", EnglishKey)), baseLanguage);
            Assert.Equal(EnglishText, registry.TryGet("de", EnglishKey, false));
            Assert.Empty(registry.SourceLanguages);
        }
    }

    [Fact]
    public void English_Code_Texts_Are_English_Ones_When_The_Base_Language_Is_Another()
    {
        var registry = TurkishBase();

        Assert.True(registry.GetAllTexts(false).ContainsKey(new LanguageIdKeyPair("en", EnglishKey)));
        Assert.Equal(["en"], registry.SourceLanguages);

        // the Turkish translation is the base text, which other languages fall back to
        Assert.Equal("Erişim reddedildi!", registry.TryGet("tr", EnglishKey, false));
        Assert.Equal("Erişim reddedildi!", registry.TryGet("de", EnglishKey, false));
        // English has the text it is written in
        Assert.Equal(EnglishText, registry.TryGet("en", EnglishKey, false));
        Assert.Equal(EnglishText, registry.TryGet("en-GB", EnglishKey, false));
    }

    [Fact]
    public void A_Text_The_Base_Language_Has_None_Of_Is_Taken_From_The_Language_Of_The_Code()
    {
        var registry = TurkishBase();
        var english = registry.TryGet("en", EnglishOnlyKey, false);
        Assert.NotNull(english);

        Assert.Equal(english, registry.TryGet("tr", EnglishOnlyKey, false));
        Assert.Equal(english, registry.TryGet("de-DE", EnglishOnlyKey, false));
        Assert.Equal(english, registry.GetAllAvailableTextsInLanguage("de", false)[EnglishOnlyKey]);
        Assert.Equal("Erişim reddedildi!", registry.GetAllAvailableTextsInLanguage("de", false)[EnglishKey]);

        // a key without a text in the invariant language, e.g. of a text file, does not hide it
        registry.Add("", "Only.Key", null);
        registry.Add("en", "Only.Key", "Only English");
        Assert.Equal("Only English", registry.TryGet("tr", "Only.Key", false));
    }

    [Fact]
    public void The_Texts_Of_An_Assembly_Without_A_Language_Are_In_The_Base_Language()
    {
        var registry = new LocalTextRegistry { BaseLanguageID = "tr" };
        registry.AddNestedTexts(new MockTypeSource(typeof(TextsOfThisAssembly)));

        Assert.True(registry.GetAllTexts(false).ContainsKey(new LanguageIdKeyPair("", "BaseLanguageTests.Hello")));
        Assert.Empty(registry.SourceLanguages);
    }

    [Fact]
    public void A_Language_Asked_For_Is_Kept()
    {
        var registry = new LocalTextRegistry { BaseLanguageID = "tr" };
        Assert.Equal("de", LocalTextLanguages.ForType(registry, typeof(CoreTexts), "de"));
        Assert.Equal("en", LocalTextLanguages.ForType(registry, typeof(CoreTexts)));
        Assert.Equal("", LocalTextLanguages.ForType(registry, typeof(TextsOfThisAssembly)));
        Assert.Equal("", LocalTextLanguages.ForType(registry, null));
    }

    [Fact]
    public void RemoveAll_Forgets_The_Source_Languages_But_Not_The_Base_Language()
    {
        var registry = TurkishBase();
        registry.RemoveAll();

        Assert.Empty(registry.SourceLanguages);
        Assert.Equal("tr", registry.BaseLanguageID);
    }

    [Fact]
    public void The_Initializer_Sets_The_Base_Language_Of_The_Settings()
    {
        var registry = new LocalTextRegistry();
        new DefaultLocalTextInitializer(new MockTypeSource(typeof(CoreTexts)),
            localizationSettings: Options.Create(new LocalizationSettings { BaseLanguage = "tr" })).Initialize(registry);

        Assert.Equal("tr", registry.BaseLanguageID);
        Assert.True(registry.GetAllTexts(false).ContainsKey(new LanguageIdKeyPair("en", EnglishKey)));
    }
}
