namespace Serenity.Localization;

public partial class PropertyItemsLocalTextRegistrationTests
{
    public interface IAddressTexts
    {
        [DisplayName("Street Address"), Hint("The street and the number"), Placeholder("e.g. Main St. 5")]
        string? Street { get; set; }

        [DisplayName("City")]
        string? City { get; set; }
    }

    private class CombinedTextsAttribute : Attribute, Serenity.Reflection.IIntrinsicPropertyAttributeProvider
    {
        [DisplayName("Postal Code"), Placeholder("e.g. 34000")]
        public object PropertyAttributes { get; set; } = null!;
    }

    [FormScript("Test.InterfaceTexts")]
    private class FormWithInterfaceTexts : IAddressTexts
    {
        public string? Street { get; set; }

        [DisplayName("Town")]
        public string? City { get; set; }

        [CombinedTexts]
        public string? PostalCode { get; set; }
    }

    private static Dictionary<string, string?> RegisterTexts(Type type)
    {
        var registry = new LocalTextRegistry();
        PropertyItemsLocalTextRegistration.AddPropertyItemsTexts(registry, new MockTypeSource(type));
        return registry.GetAllTexts(false).ToDictionary(x => x.Key.Key, x => x.Value);
    }

    [Fact]
    public void Registers_TheTexts_OfTheInterfaceMembers_ByTheKeysOfTheForm()
    {
        var texts = RegisterTexts(typeof(FormWithInterfaceTexts));

        Assert.Equal("Street Address", texts["Forms.Test.InterfaceTexts.Street"]);
        Assert.Equal("The street and the number", texts["Forms.Test.InterfaceTexts.Street_Hint"]);
        Assert.Equal("e.g. Main St. 5", texts["Forms.Test.InterfaceTexts.Street_Placeholder"]);
    }

    [Fact]
    public void Registers_TheOwnText_InsteadOfTheOneOfTheInterface()
    {
        Assert.Equal("Town", RegisterTexts(typeof(FormWithInterfaceTexts))["Forms.Test.InterfaceTexts.City"]);
    }

    [Fact]
    public void Registers_TheTexts_OfCombinationAttributes()
    {
        var texts = RegisterTexts(typeof(FormWithInterfaceTexts));

        Assert.Equal("Postal Code", texts["Forms.Test.InterfaceTexts.PostalCode"]);
        Assert.Equal("e.g. 34000", texts["Forms.Test.InterfaceTexts.PostalCode_Placeholder"]);
    }
}
