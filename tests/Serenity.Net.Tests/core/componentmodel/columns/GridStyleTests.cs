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

        var texts = registry.AddedList.ToDictionary(x => x.key, x => x.text);
        Assert.Equal("No Stripes", texts["Enums.Serenity.GridStyle.NoStripes"]);
        Assert.Equal("No Cell Borders", texts["Enums.Serenity.GridStyle.NoCellBorders"]);
        Assert.Equal("No Header Chrome", texts["Enums.Serenity.GridStyle.NoHeaderChrome"]);
        Assert.Equal("Lines", texts["Enums.Serenity.GridStyle.Lines"]);
        Assert.Equal("Plain", texts["Enums.Serenity.GridStyle.Plain"]);
    }
}
