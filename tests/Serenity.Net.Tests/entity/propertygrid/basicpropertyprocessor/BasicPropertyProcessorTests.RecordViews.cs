namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessorTests
{
    private class TestCardAttribute : RecordViewTypeAttribute
    {
        public const string Key = "Test.TestCard";

        public TestCardAttribute()
            : base(Key)
        {
        }

        public bool ShowRevenue
        {
            get => GetOption<bool>("showRevenue");
            set => SetOption("showRevenue", value);
        }
    }

    private class RecordViewsForm
    {
        public string? NoViews { get; set; }

        [TestCard(ShowRevenue = true, IdField = "CustomerId")]
        public string? WithCard { get; set; }

        [TestCard, RecordViewType("Test.OtherCard")]
        public string? TwoViews { get; set; }

        [TestCard(IdField = "First"), TestCard(IdField = "Second")]
        public string? SameViewTwice { get; set; }
    }

    private class RecordViewsJoinForm
    {
        [TestCard]
        public string? Name { get; set; }
    }

    private class CardTypeAttribute : RecordViewTypeAttribute
    {
        public CardTypeAttribute()
            : base("Test.CardType")
        {
            Placement = RecordViewPlacement.Card;
        }
    }

    [Fact]
    public void RecordViewInfo_FromAttribute_Has_The_Placement_And_The_Options_Of_A_Card_View()
    {
        var info = RecordViewInfo.FromAttribute(new CardTypeAttribute { Default = true, Below = "lg", HideSwitch = true });

        Assert.Equal("Test.CardType", info.Type);
        Assert.Equal("card", info.Placement);
        Assert.Equal(true, info.Params["default"]);
        Assert.Equal("lg", info.Params["below"]);
        Assert.Equal(true, info.Params["hideSwitch"]);
        Assert.False(info.Params.ContainsKey("idField"));
    }

    [Fact]
    public void RecordViewInfo_FromAttribute_Prefers_The_IdField_Of_The_Attribute()
    {
        Assert.Equal("Own", RecordViewInfo.FromAttribute(new TestCardAttribute { IdField = "Own" }, "Joined").Params["idField"]);
        Assert.Equal("Joined", RecordViewInfo.FromAttribute(new TestCardAttribute(), "Joined").Params["idField"]);
    }

    [Fact]
    public void RecordViewInfo_FromAttribute_Throws_For_Null()
    {
        Assert.Throws<ArgumentNullException>(() => RecordViewInfo.FromAttribute(null!));
    }

    [Fact]
    public void RecordViews_Is_Null_Without_Views()
    {
        var item = Process<RecordViewsForm>(nameof(RecordViewsForm.NoViews));
        Assert.Null(item.RecordViews);
    }

    [Fact]
    public void RecordViews_Has_Type_Options_And_IdField()
    {
        var item = Process<RecordViewsForm>(nameof(RecordViewsForm.WithCard));

        var view = Assert.Single(item.RecordViews!);
        Assert.Equal("Test.TestCard", view.Type);
        Assert.Null(view.Placement);
        Assert.Equal(true, view.Params["showRevenue"]);
        Assert.Equal("CustomerId", view.Params["idField"]);
    }

    [Fact]
    public void RecordViews_Has_Each_View_Of_The_Property()
    {
        var item = Process<RecordViewsForm>(nameof(RecordViewsForm.TwoViews));

        Assert.Collection(item.RecordViews!,
            x => Assert.Equal("Test.TestCard", x.Type),
            x => Assert.Equal("Test.OtherCard", x.Type));
        Assert.All(item.RecordViews!, x => Assert.False(x.Params.ContainsKey("idField")));
    }

    [Fact]
    public void RecordViews_Keeps_Only_One_Of_The_Same_View()
    {
        var item = Process<RecordViewsForm>(nameof(RecordViewsForm.SameViewTwice));
        var view = Assert.Single(item.RecordViews!);
        Assert.Equal("Test.TestCard", view.Type);
    }

    [Fact]
    public void RecordViews_IdField_Is_Determined_From_The_Join()
    {
        var row = NewEditLinkRow();
        var fields = row.GetFields();
        fields.Name.PropertyName = "Name";
        var join = new LeftJoin("Cities", "City", new Criteria("1=1"));
        fields.Name.join = join;
        fields.CityId.PropertyName = "CityId";
        fields.CityId.ForeignJoinAlias = join;

        var item = Process<RecordViewsJoinForm>(nameof(RecordViewsJoinForm.Name), row);

        var view = Assert.Single(item.RecordViews!);
        Assert.Equal("CityId", view.Params["idField"]);
    }
}
