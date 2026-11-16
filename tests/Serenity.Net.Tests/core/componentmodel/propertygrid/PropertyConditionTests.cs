namespace Serenity.ComponentModel;

public class PropertyConditionTests
{
    private enum Action
    {
        SetFilter = 1,
        OpenLink = 2
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(false, false)]
    [InlineData("a", true)]
    [InlineData(0, true)]
    [InlineData(true, true)]
    public void HasValue_Is_False_For_Null_Empty_And_False(object value, bool expected)
    {
        Assert.Equal(expected, PropertyCondition.HasValue(value));
        Assert.False(PropertyCondition.HasValue(Array.Empty<string>()));
        Assert.True(PropertyCondition.HasValue(new[] { "a" }));
    }

    [Fact]
    public void IsMet_Compares_Values_As_Text_Like_The_Forms()
    {
        var condition = new VisibleWhenAttribute("Action", Action.OpenLink).ToCondition();
        Assert.True(condition.IsMet(2));
        Assert.True(condition.IsMet("2"));
        Assert.True(condition.IsMet(Action.OpenLink));
        Assert.False(condition.IsMet(Action.SetFilter));
        Assert.False(condition.IsMet(null));

        var flag = new RequiredWhenAttribute("Flag", true).ToCondition();
        Assert.True(flag.IsMet(true));
        Assert.True(flag.IsMet("true"));
        Assert.False(flag.IsMet(false));

        var number = new RequiredWhenAttribute("Rate", 1.5m).ToCondition();
        Assert.True(number.IsMet(1.5));
        Assert.True(number.IsMet("1.5"));
    }

    [Fact]
    public void IsMet_Matches_A_List_By_Any_Item_And_Is_Opposite_With_IsNot()
    {
        var condition = new VisibleWhenAttribute("Tags", "a") { IsNot = false }.ToCondition();
        Assert.True(condition.IsMet(new[] { "b", "a" }));
        Assert.False(condition.IsMet(new[] { "b" }));

        var empty = new VisibleWhenAttribute("Column") { IsNot = true }.ToCondition();
        Assert.True(empty.IsMet(""));
        Assert.True(empty.IsMet(null));
        Assert.False(empty.IsMet("Day"));
    }

    [Fact]
    public void Attribute_Requires_A_Field()
    {
        Assert.Throws<ArgumentNullException>(() => new VisibleWhenAttribute(" "));
    }
}
