using Serenity.Localization;

namespace Serenity.Tests.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    private class TypeWithoutFormsOrColumnsAttr
    {
        [DisplayName("S.No")]
        public string SNo { get; set; }
        [Category("S.No")]
        public string SNoCategory { get; set; }
        [Tab("S.No")]
        public string SNoTab { get; set; }
        [Hint("S.No")]
        public string SNoHint { get; set; }
        [Placeholder("S.No")]
        public string SNoPlaceholder { get; set; }
    }

    [Fact]
    public void Skips_Types_Without_Columns_Or_Forms_Attribute()
    {
        var registry = new LocalTextRegistry();

        var typeSource = new MockTypeSource(typeof(TypeWithoutFormsOrColumnsAttr));

        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, typeSource);

        Assert.Empty(registry.GetAllTexts(false));
    }
}