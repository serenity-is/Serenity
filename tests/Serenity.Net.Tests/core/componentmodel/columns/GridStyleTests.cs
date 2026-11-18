using Serenity.Localization;

namespace Serenity.ComponentModel;

public class GridStyleTests
{
    [Fact]
    public void Presets_Combine_The_Members_That_Hide_Things()
    {
        Assert.Equal(GridStyle.NoStripes | GridStyle.NoHeaderChrome, GridStyle.Lines);
        Assert.Equal(GridStyle.NoStripes | GridStyle.NoCellBorders | GridStyle.NoHeaderChrome, GridStyle.Plain);
        Assert.True(typeof(GridStyle).IsDefined(typeof(FlagsAttribute), false));
    }

    [Fact]
    public void Members_Have_Texts_With_The_Key_Of_The_Full_Name_Of_The_Enum()
    {
        var registry = new MockLocalTextRegistry();
        registry.AddEnumTexts(new MockTypeSource(typeof(GridStyle)));

        var texts = registry.AddedList.ToDictionary(x => x.Item2, x => x.Item3);
        Assert.Equal("No Stripes", texts["Enums.Serenity.ComponentModel.GridStyle.NoStripes"]);
        Assert.Equal("No Cell Borders", texts["Enums.Serenity.ComponentModel.GridStyle.NoCellBorders"]);
        Assert.Equal("No Header Chrome", texts["Enums.Serenity.ComponentModel.GridStyle.NoHeaderChrome"]);
        Assert.Equal("Lines", texts["Enums.Serenity.ComponentModel.GridStyle.Lines"]);
        Assert.Equal("Plain", texts["Enums.Serenity.ComponentModel.GridStyle.Plain"]);
    }
}
