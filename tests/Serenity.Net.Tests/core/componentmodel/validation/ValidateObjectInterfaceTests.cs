namespace Serenity.ComponentModel;

/// <summary>
/// An object is validated with the attributes of the members of the interfaces its properties implement, and of the combination
/// attributes they have, as the browser does with its property items.
/// </summary>
public class ValidateObjectInterfaceTests
{
    public interface ILinkOptions
    {
        string? Action { get; set; }

        [VisibleWhen(nameof(Action), "link"), Required, ValidateUrl]
        string? Url { get; set; }

        [RequiredWhen(nameof(Action), "dialog")]
        string? Dialog { get; set; }

        [ReadOnly(true), Required]
        string? Locked { get; set; }
    }

    private class QuantityRuleAttribute : Attribute, Reflection.IIntrinsicPropertyAttributeProvider
    {
        [ValidateRange(1, 10)]
        public object PropertyAttributes { get; set; } = null!;
    }

    private class Model : ILinkOptions
    {
        public string? Action { get; set; }
        public string? Url { get; set; }
        public string? Dialog { get; set; }
        public string? Locked { get; set; }

        [QuantityRule]
        public int? Quantity { get; set; }
    }

    private static ObjectValidationFailure? Validate(Model model) => ValidationRules.ValidateObject(model, null);

    [Fact]
    public void A_Valid_Object_Has_No_Failure()
    {
        Assert.Null(Validate(new Model()));
        Assert.Null(Validate(new Model { Action = "link", Url = "https://serenity.is", Quantity = 5 }));
    }

    [Fact]
    public void The_Rules_And_Conditions_Of_The_Interface_Members_Are_Checked()
    {
        var failure = Validate(new Model { Action = "link" });
        Assert.Equal("Url", failure?.PropertyName);
        Assert.True(failure!.IsRequired);

        failure = Validate(new Model { Action = "link", Url = "not a url" });
        Assert.Equal("Url", failure?.PropertyName);
        Assert.False(failure!.IsRequired);

        Assert.Equal("Dialog", Validate(new Model { Action = "dialog" })?.PropertyName);
    }

    [Fact]
    public void A_Read_Only_Interface_Member_Is_Not_Checked()
    {
        Assert.Null(Validate(new Model { Locked = null }));
    }

    [Fact]
    public void The_Rules_Of_Combination_Attributes_Are_Checked()
    {
        Assert.Equal("Quantity", Validate(new Model { Quantity = 11 })?.PropertyName);
    }
}
