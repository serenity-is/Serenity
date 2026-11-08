using Serenity.Testing;

namespace Serenity;

public class StringEnumInfoTests
{
    [Fact]
    public void EnumKey_ComesFromAttribute()
    {
        Assert.Equal("My.WidgetCategory", StringEnum.GetInfo<WidgetCategory>().EnumKey);
    }

    [Fact]
    public void EnumKey_FallsBackToFullName()
    {
        Assert.Equal(typeof(WidgetCategoryExtended).FullName,
            StringEnum.GetInfo<WidgetCategoryExtended>().EnumKey);
    }

    [Fact]
    public void Members_OnlyIncludeStringConstants()
    {
        var names = StringEnum.GetInfo<WidgetCategory>().Members.Select(x => x.Name);

        Assert.DoesNotContain(nameof(WidgetCategory.NotAString), names);
        Assert.DoesNotContain(nameof(WidgetCategory.NotAConstant), names);
    }

    [Fact]
    public void Members_IncludeHiddenMembersButNotAsVisible()
    {
        var members = StringEnum.GetInfo<WidgetCategory>().Members;

        Assert.Equal(nameof(WidgetCategory.Internal), members.Single(x => x.Value == "internal").Name);
        Assert.Contains(members, x => x.Value == "system");
        Assert.False(members.Single(x => x.Value == "internal").IsVisible);
        Assert.False(members.Single(x => x.Value == "system").IsVisible);

        var visible = StringEnum.GetInfo<WidgetCategory>().VisibleMembers.Select(x => x.Value);
        Assert.Equal(["charts", "tables", "outlines", "charts"], visible);
    }

    [Fact]
    public void VisibleMembers_KeepDeclarationOrder()
    {
        var visible = StringEnum.GetInfo<WidgetCategory>().VisibleMembers;

        Assert.Equal([nameof(WidgetCategory.Charts), nameof(WidgetCategory.Tables),
            nameof(WidgetCategory.Outlines), nameof(WidgetCategory.ChartsAlias)],
            visible.Select(x => x.Name));
    }

    [Fact]
    public void Members_CarryTextInformation()
    {
        var members = StringEnum.GetInfo<WidgetCategory>().Members;

        var charts = members.Single(x => x.Name == nameof(WidgetCategory.Charts));
        Assert.Equal("Charts", charts.Text);

        var outlines = members.Single(x => x.Name == nameof(WidgetCategory.Outlines));
        Assert.Equal("Outlines", outlines.Text);
        Assert.Equal("Site.Outlines", outlines.TextKey);

        var tables = members.Single(x => x.Name == nameof(WidgetCategory.Tables));
        Assert.Null(tables.Text);
        Assert.Null(tables.TextKey);
    }

    [Fact]
    public void FindByValue_ReturnsFirstMember_ForAliasedValue()
    {
        var info = StringEnum.GetInfo<WidgetCategory>();

        Assert.Equal(nameof(WidgetCategory.Charts), info.FindByValue("charts")?.Name);
    }

    [Fact]
    public void FindByName_FindsHiddenMembers()
    {
        var info = StringEnum.GetInfo<WidgetCategory>();

        var member = info.FindByName(nameof(WidgetCategory.Internal));
        Assert.NotNull(member);
        Assert.Equal("internal", member.Value);
        Assert.False(member.IsVisible);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FindByValue_ReturnsNull_ForEmptyValue(string? value)
    {
        Assert.Null(StringEnum.GetInfo<WidgetCategory>().FindByValue(value));
    }

    [Fact]
    public void Lookups_AreAvailableBeforeAnyOtherMemberIsUsed()
    {
        // both lookup tables are built in the constructor, so a lookup can't see a partially filled table
        var info = new StringEnumInfoForTests().Info;

        Assert.True(info.FindByValue("charts") != null);
        Assert.True(info.FindByName("Charts") != null);
    }

    private sealed class StringEnumInfoForTests
    {
        public StringEnumInfo Info { get; } = StringEnum.GetInfo(typeof(WidgetCategory));
    }
}
