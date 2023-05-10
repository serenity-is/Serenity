namespace Serenity.Tests.Localization;

public class LocalTextTests
{
    [Fact]
    public void InvariantLanguageID_IsEmptyString()
    {
        Assert.Equal(string.Empty, LocalText.InvariantLanguageID);
    }

    [Fact]
    public void Empty_Is_A_LocalTextInstance_With_EmptyKey()
    {
        Assert.NotNull(LocalText.Empty);
        Assert.Equal(string.Empty, LocalText.Empty.Key);
    }

    [Fact]
    public void Empty_ToString_Returns_EmptyString()
    {
        Assert.NotNull(LocalText.Empty);
        Assert.Equal(string.Empty, LocalText.Empty.ToString(localizer: null));
    }

    [Fact]
    public void Constructor_AcceptsNullAndEmptyString()
    {
        _ = new LocalText(null);
        _ = new LocalText(string.Empty);
    }

    [Fact]
    public void Key_Returns_KeySet_In_Constructor_AsIs()
    {
        Assert.Equal(string.Empty, new LocalText(null).Key);
        Assert.Equal(string.Empty, new LocalText(string.Empty).Key);
        Assert.Equal("ABC", new LocalText("ABC").Key);
        Assert.Equal("  dEf ", new LocalText("  dEf ").Key);
        Assert.Equal("  dEf ", new LocalText("  dEf ").Key);
    }

    [Fact]
    public void ImplicitConversionFromString_ReturnsLocalTextInstanceWithKey()
    {
        LocalText actual1 = "ABC";
        Assert.Equal("ABC", actual1.Key);

        LocalText actual2 = "";
        Assert.Equal("", actual2.Key);

        LocalText actual3 = "  dEf ";
        Assert.Equal("  dEf ", actual3.Key);
    }

    [Fact]
    public void ImplicitConversionFromString_Returns_Empty_For_Null_Or_EmptyString()
    {
        LocalText actual1 = "";
        Assert.Equal(LocalText.Empty, actual1);

        string a = null;
        LocalText actual2 = a;
        Assert.Equal(LocalText.Empty, actual2);
    }

    [Fact]
    public void ToString_ReturnsNull_If_Key_IsNull()
    {
        LocalText text1 = new(null);
        string actual1 = text1.ToString(localizer: null);
        Assert.Equal(string.Empty, actual1);
    }

    [Fact]
    public void ToString_ReturnsEmpty_If_Key_IsEmpty()
    {
        LocalText text2 = new LocalText(string.Empty);
        string actual2 = text2.ToString(localizer: null);
        Assert.Equal(string.Empty, actual2);
    }

    [Fact]
    public void ToString_DoesntThrowIfNoLocalTextProvider()
    {
        _ = new LocalText("Dummy").ToString(localizer: null);
    }

    [Fact]
    public void ToString_Returns_KeyAsIs_If_NoLocalTextProvider()
    {
        Assert.Equal("Dummy", new LocalText("Dummy").ToString(localizer: null));
        Assert.Equal(string.Empty, new LocalText(null).ToString(localizer: null));
        Assert.Equal(string.Empty, new LocalText(string.Empty).ToString(localizer: null));
    }

    [Fact]
    public void ToString_Returns_Key_If_NoTranslationIsFound()
    {
        const string key = "Db.MissingTable.MissingField";
        var text = new LocalText(key);

        string translation = text.ToString(NullTextLocalizer.Instance);
        Assert.Equal(key, translation);
    }

    [Fact]
    public void ToString_Returns_Translation_FromRegistry()
    {
        var registry = new MockLocalTextRegistry();
        registry.Add("es", "Translation1", "es:Translation1");
        registry.Add("es", "Translation2", "es:Translation2");
        var localizer = new MockTextLocalizer(registry, getLanguageId: () => "es");

        string translation1 = new LocalText("Translation1").ToString(localizer);
        string translation2 = new LocalText("Translation2").ToString(localizer);
        Assert.Equal("es:Translation1", translation1);
        Assert.Equal("es:Translation2", translation2);
    }
}