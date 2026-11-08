using Serenity.ComponentModel;
using Serenity.Localization;
using Serenity.Testing;

namespace Serenity;

public class StringEnumTests
{
    [Theory]
    [InlineData(typeof(WidgetCategory), true)]
    [InlineData(typeof(WidgetCategoryExtended), true)]
    [InlineData(typeof(EmptyStringEnum), true)]
    [InlineData(typeof(StringEnum), false)]
    [InlineData(typeof(SampleEnum), false)]
    [InlineData(typeof(string), false)]
    [InlineData(null, false)]
    public void IsStringEnum_ReturnsExpected(Type? type, bool expected)
    {
        Assert.Equal(expected, StringEnum.IsStringEnum(type));
    }

    [Fact]
    public void GetInfo_ThrowsArgumentNull_IfTypeIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => StringEnum.GetInfo(null!));
    }

    [Fact]
    public void GetInfo_ThrowsArgumentOutOfRange_ForNonStringEnum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StringEnum.GetInfo(typeof(SampleEnum)));
    }

    [Fact]
    public void GetInfo_CachesPerType()
    {
        Assert.Same(StringEnum.GetInfo<WidgetCategory>(), StringEnum.GetInfo(typeof(WidgetCategory)));
        Assert.NotSame(StringEnum.GetInfo<WidgetCategory>(), StringEnum.GetInfo<WidgetCategoryExtended>());
    }

    [Fact]
    public void GetValues_ReturnsDeclaredMembersInDeclarationOrder()
    {
        Assert.Equal(["charts", "tables", "outlines", "internal", "system", "charts"],
            StringEnum.GetValues<WidgetCategory>());
    }

    [Fact]
    public void GetNames_ReturnsDeclaredMembersInDeclarationOrder()
    {
        Assert.Equal(["Charts", "Tables", "Outlines", "Internal", "System", "ChartsAlias"],
            StringEnum.GetNames<WidgetCategory>());
    }

    [Fact]
    public void GetValues_ReturnsOnlyDeclaredMembers_ForDerivedType()
    {
        Assert.Equal(["maps"], StringEnum.GetValues<WidgetCategoryExtended>());
        Assert.Equal(["Maps"], StringEnum.GetNames<WidgetCategoryExtended>());
    }

    [Fact]
    public void GetValues_ReturnsEmpty_ForTypeWithoutMembers()
    {
        Assert.Empty(StringEnum.GetValues<EmptyStringEnum>());
        Assert.Empty(StringEnum.GetNames<EmptyStringEnum>());
        Assert.False(StringEnum.IsDefined<EmptyStringEnum>("x"));
    }

    [Fact]
    public void GetValues_DoesNotExposeMutableCache()
    {
        var values = StringEnum.GetValues<WidgetCategory>();
        var names = StringEnum.GetNames<WidgetCategory>();
        var members = StringEnum.GetInfo<WidgetCategory>().Members;

        Assert.Throws<NotSupportedException>(() => ((IList<string>)values).Add("oops"));
        Assert.Throws<NotSupportedException>(() => ((IList<string>)names).Add("Oops"));
        Assert.Throws<NotSupportedException>(() => ((IList<StringEnumMember>)members)
            .Add(new StringEnumMember("Oops", "oops", null, null, true)));

        Assert.Equal(6, StringEnum.GetValues<WidgetCategory>().Count);
    }

    [Fact]
    public void GetName_ReturnsMemberName_ForValue()
    {
        Assert.Equal("Charts", StringEnum.GetName<WidgetCategory>("charts"));
        Assert.Equal("Tables", StringEnum.GetName(typeof(WidgetCategory), "tables"));
    }

    [Fact]
    public void GetName_ReturnsFirstMember_ForAliasedValue()
    {
        Assert.Equal("Charts", StringEnum.GetName<WidgetCategory>("charts"));
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("")]
    [InlineData(null)]
    public void GetName_ReturnsNull_ForUnknownValue(string? value)
    {
        Assert.Null(StringEnum.GetName<WidgetCategory>(value));
    }

    [Fact]
    public void GetValue_ReturnsValue_ForMemberName()
    {
        Assert.Equal("charts", StringEnum.GetValue<WidgetCategory>("Charts"));
        Assert.Equal("internal", StringEnum.GetValue(typeof(WidgetCategory), "Internal"));
    }

    [Theory]
    [InlineData("Missing")]
    [InlineData("")]
    [InlineData(null)]
    public void GetValue_ReturnsNull_ForUnknownName(string? name)
    {
        Assert.Null(StringEnum.GetValue<WidgetCategory>(name));
    }

    [Fact]
    public void IsDefined_ReturnsTrue_ForMembers()
    {
        Assert.True(StringEnum.IsDefined<WidgetCategory>("charts"));
        Assert.True(StringEnum.IsDefined(typeof(WidgetCategory), "tables"));
    }

    [Fact]
    public void IsDefined_ReturnsTrue_ForMembersHiddenFromClient()
    {
        // the attributes only hide the members from the client, they are still valid values on the server
        Assert.True(StringEnum.IsDefined<WidgetCategory>("internal"));
        Assert.True(StringEnum.IsDefined<WidgetCategory>("system"));
    }

    [Theory]
    [InlineData("Charts")]
    [InlineData("missing")]
    [InlineData("")]
    [InlineData(null)]
    public void IsDefined_ReturnsFalse_ForNonMembers(string? value)
    {
        Assert.False(StringEnum.IsDefined<WidgetCategory>(value));
    }

    [Fact]
    public void GetText_UsesDescriptionAttribute()
    {
        Assert.Equal("Charts", StringEnum.GetText<WidgetCategory>("charts"));
    }

    [Fact]
    public void GetText_UsesDescriptionTextAttribute()
    {
        Assert.Equal("Outlines", StringEnum.GetText(typeof(WidgetCategory), "outlines"));
    }

    [Fact]
    public void GetText_FallsBackToMemberName()
    {
        Assert.Equal("Tables", StringEnum.GetText<WidgetCategory>("tables"));
    }

    [Fact]
    public void GetText_ReturnsValue_ForUnknownValue()
    {
        Assert.Equal("missing", StringEnum.GetText<WidgetCategory>("missing"));
    }

    [Fact]
    public void GetText_ReturnsEmpty_ForEmptyValue()
    {
        Assert.Equal("", StringEnum.GetText<WidgetCategory>(null));
        Assert.Equal("", StringEnum.GetText<WidgetCategory>(""));
    }

    [Fact]
    public void GetText_PrefersLocalTextOfEnumsKey()
    {
        var registry = new MockLocalTextRegistry();
        registry.Add(System.Globalization.CultureInfo.CurrentUICulture.Name,
            "Enums.My.WidgetCategory.Tables", "Tabular");
        var localizer = new DefaultTextLocalizer(registry);

        Assert.Equal("Tabular", StringEnum.GetText<WidgetCategory>("tables", localizer));
        // a member without a local text or a text attribute keeps its name
        Assert.Equal("Charts", StringEnum.GetText<WidgetCategory>("charts", localizer));
    }

    [Fact]
    public void GetText_PrefersLocalTextOfTextKey()
    {
        var registry = new MockLocalTextRegistry();
        registry.Add(System.Globalization.CultureInfo.CurrentUICulture.Name,
            "Site.Outlines", "Outlines (localized)");
        var localizer = new DefaultTextLocalizer(registry);

        Assert.Equal("Outlines (localized)", StringEnum.GetText<WidgetCategory>("outlines", localizer));
    }
}
