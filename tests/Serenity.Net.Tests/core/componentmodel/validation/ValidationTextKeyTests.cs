using System.ComponentModel.DataAnnotations;
using Serenity.Localization;

namespace Serenity.ComponentModel;

public class ValidationTextKeyTests
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    private class ValidateClientAttribute(string rule) : ValidationRuleAttribute
    {
        public override string Rule => rule;
        protected override bool IsValidValue(object? value, ValidationContext context) => false;
    }

    [Fact]
    public void Suffix_Is_The_Name_Of_The_Attribute_And_The_Rule_Only_For_One_That_Can_Be_Repeated()
    {
        Assert.Equal("_ValidateRange", ValidationRules.GetTextKeySuffix(new ValidateRangeAttribute(1, 2)));
        // the rule of a length changes by its minimum, which the key doesn't
        Assert.Equal("_ValidateLength", ValidationRules.GetTextKeySuffix(new ValidateLengthAttribute(5) { MinimumLength = 1 }));
        Assert.Equal("_ValidateClient_even", ValidationRules.GetTextKeySuffix(new ValidateClientAttribute("even")));
        Assert.Null(ValidationRules.GetTextKeySuffix(new RangeAttribute(1, 2)));
    }

    [Fact]
    public void Server_Message_Is_The_Text_Of_The_Implied_Key_Before_The_Own_Message()
    {
        var localizer = new MockTextLocalizer(key => key == "Db.MyRow.Quantity_ValidateRange" ? "Translated {0}" : null!);
        var services = new ServiceCollection().AddSingleton<ITextLocalizer>(localizer).BuildServiceProvider();
        var context = new ValidationContext(new object(), services, new Dictionary<object, object?>
        {
            [ValidationRuleMessage.PropertyTextKeyItem] = "Db.MyRow.Quantity"
        });

        Assert.Equal("Translated 1", ValidationRules.Validate(new ValidateRangeAttribute(1, 2) { ErrorMessage = "Own" }, 3, context));
        // an explicit key comes first
        var keyed = new MockTextLocalizer(key => key switch
        {
            "Site.Explicit" => "Explicit",
            "Db.MyRow.Quantity_ValidateRange" => "Implied",
            _ => null!
        });
        var keyedContext = new ValidationContext(new object(), new ServiceCollection().AddSingleton<ITextLocalizer>(keyed).BuildServiceProvider(),
            new Dictionary<object, object?> { [ValidationRuleMessage.PropertyTextKeyItem] = "Db.MyRow.Quantity" });
        Assert.Equal("Explicit", ValidationRules.Validate(new ValidateRangeAttribute(1, 2) { TextKey = "Site.Explicit" }, 3, keyedContext));
    }

    [FormScript("MyForm")]
    private class MessagesForm
    {
        [ValidateRange(1, 5, ErrorMessage = "Pick 1 to 5")]
        public int? Quantity { get; set; }

        [ValidateEmail]
        public string? Email { get; set; }

        [ValidateLength(5, TextKey = "Site.MyLength", ErrorMessage = "Too long")]
        public string? Code { get; set; }

        [Range(1, 2, ErrorMessage = "Plain")]
        public int? Plain { get; set; }
    }

    [Fact]
    public void Form_Texts_Register_The_Own_Messages_With_Their_Implied_Keys()
    {
        var registry = new LocalTextRegistry();
        registry.AddPropertyItemsTexts(new MockTypeSource(typeof(MessagesForm)));

        Assert.Equal("Pick 1 to 5", registry.TryGet(LocalText.InvariantLanguageID, "Forms.MyForm.Quantity_ValidateRange", false));
        Assert.Equal("Too long", registry.TryGet(LocalText.InvariantLanguageID, "Site.MyLength", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "Forms.MyForm.Email_ValidateEmail", false));
        Assert.Null(registry.TryGet(LocalText.InvariantLanguageID, "Forms.MyForm.Plain_Range", false));
    }

    [LocalTextPrefix("MyRow")]
    private class MessagesRow : Row<MessagesRow.RowFields>
    {
        [ValidateRange(1, 10, ErrorMessage = "Pick 1 to 10")]
        public int? Quantity { get => fields.Quantity[this]; set => fields.Quantity[this] = value; }

        public class RowFields : RowFieldsBase
        {
            public Int32Field Quantity = null!;
        }
    }

    [Fact]
    public void Row_Texts_Register_The_Own_Messages_With_Their_Implied_Keys()
    {
        var registry = new LocalTextRegistry();
        registry.AddRowTexts([new MessagesRow()]);

        Assert.Equal("Pick 1 to 10", registry.TryGet(LocalText.InvariantLanguageID, "Db.MyRow.Quantity_ValidateRange", false));
    }
}
