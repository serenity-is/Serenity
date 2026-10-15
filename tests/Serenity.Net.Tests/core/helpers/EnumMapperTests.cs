namespace Serenity;

public class EnumMapperTests
{
    [EnumKey("TestEnum")]
    private enum TestEnum
    {
        [Description("First Value")]
        First = 1,
        Second = 2,
        Third = 3
    }

    [EnumKey("TextKeyEnum")]
    private enum TextKeyEnum
    {
        [DescriptionText("Invariant description", TextKey = "Site.EnumDescription")]
        ExplicitTextKey,
        [DescriptionText("Site.LiteralDescription")]
        KeyShapedLiteral,
        [Description("Site.LegacyDescription")]
        LegacyKeyCandidate
    }

    [Fact]
    public void TryParse_ReturnsTrue_ForValidName()
    {
        Assert.True(EnumMapper.TryParse<TestEnum>("First", out var value));
        Assert.Equal(TestEnum.First, value);
    }

    [Fact]
    public void TryParse_IsCaseInsensitive()
    {
        Assert.True(EnumMapper.TryParse<TestEnum>("first", out var value));
        Assert.Equal(TestEnum.First, value);
    }

    [Fact]
    public void TryParse_ReturnsFalse_ForInvalidName()
    {
        Assert.False(EnumMapper.TryParse<TestEnum>("Missing", out var value));
        Assert.Equal(default, value);
    }

    [Fact]
    public void Parse_ReturnsValue_ForValidName()
    {
        Assert.Equal(TestEnum.Second, EnumMapper.Parse<TestEnum>("Second"));
    }

    [Fact]
    public void Parse_ThrowsArgumentOutOfRangeException_ForInvalidName()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => EnumMapper.Parse<TestEnum>("Missing"));
    }

    [Fact]
    public void ToString_ReturnsName_ForKnownValue()
    {
        Assert.Equal("Second", EnumMapper.ToString(typeof(TestEnum), TestEnum.Second));
    }

    [Fact]
    public void ToString_ReturnsNumber_ForUnknownValue()
    {
        Assert.Equal("99", EnumMapper.ToString(typeof(TestEnum), (TestEnum)99));
    }

    [Fact]
    public void GetName_ReturnsName_ForValue()
    {
        Assert.Equal("Third", TestEnum.Third.GetName());
    }

    [Fact]
    public void GetName_ReturnsEmpty_ForNull()
    {
        Assert.Equal(string.Empty, ((TestEnum?)null).GetName());
    }

    [Fact]
    public void GetText_ReturnsDescription_WhenPresent()
    {
        var text = TestEnum.First.GetText(NullTextLocalizer.Instance);
        Assert.Equal("First Value", text);
    }

    [Fact]
    public void GetText_ReturnsEnumName_WhenNoDescription()
    {
        var text = TestEnum.Second.GetText(NullTextLocalizer.Instance);
        Assert.Equal("Second", text);
    }

    [Fact]
    public void GetText_ReturnsEmpty_ForNull()
    {
        Assert.Equal(string.Empty, ((TestEnum?)null).GetText(NullTextLocalizer.Instance));
    }

    [Fact]
    public void GetEnumTypeKey_ReturnsAttributeValue_WhenPresent()
    {
        Assert.Equal("TestEnum", EnumMapper.GetEnumTypeKey(typeof(TestEnum)));
    }

    [Fact]
    public void GetEnumTypeKey_ReturnsFullName_WhenNoAttribute()
    {
        Assert.Equal("TestEnum", EnumMapper.GetEnumTypeKey(typeof(TestEnum)));
    }

    [Fact]
    public void FormatEnum_ReturnsLocalizedText_WhenAvailable()
    {
        var localizer = new MockTextLocalizer(key =>
            key == "Enums.TestEnum.First" ? "Localized First" : null);
        Assert.Equal("Localized First", localizer.FormatEnum(typeof(TestEnum), TestEnum.First));
    }

    [Fact]
    public void FormatEnum_ReturnsDescription_WhenNoLocalizedText()
    {
        var text = NullTextLocalizer.Instance.FormatEnum(typeof(TestEnum), TestEnum.First);
        Assert.Equal("First Value", text);
    }

    [Fact]
    public void FormatEnum_ReturnsEnumName_WhenNoLocalizedTextOrDescription()
    {
        var text = NullTextLocalizer.Instance.FormatEnum(typeof(TestEnum), TestEnum.Second);
        Assert.Equal("Second", text);
    }

    [Fact]
    public void FormatEnum_Uses_DescriptionText_TextKey()
    {
        var localizer = new MockTextLocalizer(key =>
            key == "Site.EnumDescription" ? "Localized description" : null);

        Assert.Equal("Localized description", localizer.FormatEnum(typeof(TextKeyEnum),
            TextKeyEnum.ExplicitTextKey));
    }

    [Fact]
    public void FormatEnum_Does_Not_Infer_Key_Shaped_DescriptionText()
    {
        var localizer = new MockTextLocalizer(key =>
            key == "Site.LiteralDescription" ? "Should not localize" : null);

        Assert.Equal("Site.LiteralDescription", localizer.FormatEnum(typeof(TextKeyEnum),
            TextKeyEnum.KeyShapedLiteral));
    }

    [Fact]
    public void FormatEnum_Continues_To_Infer_Legacy_Description_Key()
    {
        var localizer = new MockTextLocalizer(key =>
            key == "Site.LegacyDescription" ? "Localized legacy description" : null);

        Assert.Equal("Localized legacy description", localizer.FormatEnum(typeof(TextKeyEnum),
            TextKeyEnum.LegacyKeyCandidate));
    }

    [Fact]
    public void FormatEnum_ReturnsEmpty_ForNullValue()
    {
        Assert.Equal(string.Empty, NullTextLocalizer.Instance.FormatEnum(typeof(TestEnum), null));
    }

    [Fact]
    public void FormatEnum_ReturnsToString_ForNonEnumType()
    {
        Assert.Equal("42", NullTextLocalizer.Instance.FormatEnum(typeof(int), 42));
    }
}
