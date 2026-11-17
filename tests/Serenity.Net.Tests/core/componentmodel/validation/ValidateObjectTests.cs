using Serenity.Localization;

namespace Serenity.ComponentModel;

public class ValidateObjectTests
{
    private class Model
    {
        [Required]
        public string? Name { get; set; }

        public string? Action { get; set; }

        [VisibleWhen(nameof(Action), "link"), Required, ValidateUrl]
        public string? Url { get; set; }

        [VisibleWhen(nameof(Url))]
        [Required]
        public string? UrlTitle { get; set; }

        [RequiredWhen(nameof(Action), "dialog")]
        public string? Dialog { get; set; }

        [ReadOnly(true), Required]
        public string? Locked { get; set; }

        public bool IsLocked { get; set; }

        [ReadOnlyWhen(nameof(IsLocked)), ValidateRange(1, 10)]
        public int? Quantity { get; set; }

        [Required]
        public bool Flag { get; set; }

        [HideOnInsert, Required]
        public string? UpdateOnly { get; set; }

        [ValidateRange(1, 10, Side = ValidationSide.Client)]
        public int? Hint { get; set; }
    }

    private static Model Valid() => new() { Name = "a", Locked = null, UpdateOnly = "u" };

    private static ObjectValidationFailure? Validate(Model model, ObjectValidationOptions? options = null) =>
        ValidationRules.ValidateObject(model, options);

    [Fact]
    public void A_Valid_Object_Has_No_Failure()
    {
        Assert.Null(Validate(Valid()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void A_Missing_Required_Value_Is_A_Required_Failure(string? name)
    {
        var model = Valid();
        model.Name = name;
        var failure = Validate(model)!;
        Assert.Equal("Name", failure.PropertyName);
        Assert.True(failure.IsRequired);
    }

    [Fact]
    public void A_Field_Hidden_By_Its_Condition_Is_Not_Checked_And_Hides_The_Ones_On_It()
    {
        var model = Valid();
        model.Url = "not a url";
        Assert.Null(Validate(model));

        model.Action = "link";
        var failure = Validate(model)!;
        Assert.Equal("Url", failure.PropertyName);
        Assert.False(failure.IsRequired);

        // UrlTitle is shown by Url, which is hidden without the link action
        model.Action = null;
        model.Url = "https://example.com";
        Assert.Null(Validate(model));
        model.Action = "link";
        Assert.Equal("UrlTitle", Validate(model)!.PropertyName);
    }

    [Fact]
    public void A_Field_Required_By_Its_Condition_Is_Required_When_It_Is_Met()
    {
        var model = Valid();
        model.Action = "dialog";
        Assert.Equal("Dialog", Validate(model)!.PropertyName);
        model.Dialog = "Northwind.OrderDialog";
        Assert.Null(Validate(model));
    }

    [Fact]
    public void A_Read_Only_Field_Is_Not_Checked()
    {
        var model = Valid();
        model.IsLocked = true;
        model.Quantity = 99;
        Assert.Null(Validate(model));
        model.IsLocked = false;
        Assert.Equal("Quantity", Validate(model)!.PropertyName);
    }

    [Fact]
    public void A_Checkbox_Is_Not_Required_And_A_Rule_Only_Checked_By_The_Forms_Is_Skipped()
    {
        var model = Valid();
        model.Hint = 99;
        Assert.Null(Validate(model));
    }

    [Fact]
    public void A_Field_Hidden_On_Insert_Is_Not_Checked_On_Insert()
    {
        var model = Valid();
        model.UpdateOnly = null;
        Assert.Null(Validate(model, new ObjectValidationOptions { IsInsert = true }));
        Assert.Equal("UpdateOnly", Validate(model, new ObjectValidationOptions { IsInsert = false })!.PropertyName);
    }

    [Fact]
    public void Only_The_Included_Properties_Are_Checked()
    {
        var model = Valid();
        model.Name = null;
        Assert.Null(Validate(model, new ObjectValidationOptions { IncludeProperty = name => name != "Name" }));
    }

    private class Entity
    {
        public string? Name { get; set; }
        public int? Quantity { get; set; }
    }

    [FormScript("MyForm")]
    private class EntityForm
    {
        [Required]
        public string? Name { get; set; }

        [ValidateRange(1, 10, ErrorMessage = "Pick 1 to 10")]
        public int? Quantity { get; set; }

        [Required]
        public string? NotOnTheEntity { get; set; }
    }

    [Fact]
    public void Validates_By_The_Rules_Of_Another_Type_Skipping_What_The_Object_Doesnt_Have()
    {
        var options = new ObjectValidationOptions { RulesType = typeof(EntityForm) };
        Assert.Equal("Name", ValidationRules.ValidateObject(new Entity(), options)!.PropertyName);
        Assert.Null(ValidationRules.ValidateObject(new Entity { Name = "a" }, options));
    }

    [Fact]
    public void Messages_Have_The_Implied_Keys_Of_The_Form()
    {
        var localizer = new MockTextLocalizer(key => key == "Forms.MyForm.Quantity_ValidateRange" ? "Translated" : null!);
        var options = new ObjectValidationOptions
        {
            RulesType = typeof(EntityForm),
            Services = new ServiceCollection().AddSingleton<ITextLocalizer>(localizer).BuildServiceProvider()
        };

        Assert.Equal("Translated", ValidationRules.ValidateObject(new Entity { Name = "a", Quantity = 11 }, options)!.Message);
    }

    [Fact]
    public async Task ValidateObjectAsync_Gives_The_Same_Failures()
    {
        var model = Valid();
        model.Action = "dialog";
        Assert.Equal("Dialog", (await ValidationRules.ValidateObjectAsync(model, cancellationToken: TestContext.Current.CancellationToken))!.PropertyName);
        Assert.Null(await ValidationRules.ValidateObjectAsync(Valid(), cancellationToken: TestContext.Current.CancellationToken));
    }
}
