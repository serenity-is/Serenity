using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

public class ValidationRulesTests
{
    private class ImpliesTwo : Attribute, IImpliesValidation
    {
        public IEnumerable<ValidationAttribute> GetImpliedValidation() => [new ValidateEmailAttribute(), new ValidateLengthAttribute(10)];
    }

    [Fact]
    public void GetRules_Has_The_Validation_Attributes_And_The_Implied_Ones()
    {
        var rules = ValidationRules.GetRules([new DisplayTextAttribute("x"), new ValidateRangeAttribute(1, 2), new ImpliesTwo()]).ToList();
        Assert.Collection(rules,
            x => Assert.IsType<ValidateRangeAttribute>(x),
            x => Assert.IsType<ValidateEmailAttribute>(x),
            x => Assert.IsType<ValidateLengthAttribute>(x));
    }

    [Fact]
    public void GetClientRule_Has_The_Rule_Its_Params_And_Its_Messages()
    {
        var rule = ValidationRules.GetClientRule(new ValidateRangeAttribute(1, 10) { TextKey = "My.Key", ErrorMessage = "Own" })!;
        Assert.Equal("Range", rule.Rule);
        Assert.Equal([1, 10], (object[])rule.Params!);
        Assert.Equal("My.Key", rule.TextKey);
        Assert.Equal("Own", rule.Message);
    }

    [Fact]
    public void GetClientRule_Of_The_Validate_Attributes()
    {
        Assert.Equal("Length", Get(new ValidateLengthAttribute(5)).Rule);
        Assert.Equal([0, 5], (int[])Get(new ValidateLengthAttribute(5)).Params!);
        Assert.Equal([2, 5], (int[])Get(new ValidateLengthAttribute(5) { MinimumLength = 2 }).Params!);
        Assert.Equal(("Pattern", (object?)"[a-z]+"), Get(new ValidatePatternAttribute("[a-z]+")));
        Assert.Equal(("Email", (object?)null), Get(new ValidateEmailAttribute()));
        Assert.Equal(("Url", (object?)null), Get(new ValidateUrlAttribute()));
        Assert.Equal(("Compare", (object?)"Password"), Get(new ValidateCompareAttribute("Password")));
    }

    private static (string Rule, object? Params) Get(ValidationAttribute attribute)
    {
        var rule = ValidationRules.GetClientRule(attribute)!;
        return (rule.Rule, rule.Params);
    }

    [Fact]
    public void GetClientRule_Is_Null_For_A_Rule_Only_Checked_On_The_Server_Or_One_The_Forms_Dont_Know()
    {
        Assert.Null(ValidationRules.GetClientRule(new ValidateRangeAttribute(1, 2) { Side = ValidationSide.Server }));
        Assert.Null(ValidationRules.GetClientRule(new CreditCardAttribute()));
    }

    [Fact]
    public void The_DataAnnotations_Attributes_Are_Checked_By_The_Forms_Too()
    {
        Assert.Equal("Range", Get(new RangeAttribute(1, 2)).Rule);
        Assert.Equal("Email", Get(new EmailAddressAttribute()).Rule);
        Assert.Equal("Url", Get(new UrlAttribute()).Rule);
        Assert.Equal([3, null], (int?[])Get(new MinLengthAttribute(3)).Params!);
        Assert.Equal([0, 4], (int[])Get(new System.ComponentModel.DataAnnotations.MaxLengthAttribute(4)).Params!);
        Assert.Equal("Length", Get(new StringLengthAttribute(4)).Rule);
    }

    private class ValidateEvenAttribute : ValidationRuleAttribute
    {
        protected override bool IsValidValue(object? value, ValidationContext context) => true;
    }

    private class ValidateKeyedAttribute() : ValidationRuleAttribute(Key)
    {
        public const string Key = "MyApp.SomeValidationRule";
        protected override bool IsValidValue(object? value, ValidationContext context) => true;
    }

    [Fact]
    public void A_Custom_Rule_Has_Its_Key_Or_The_One_Of_The_Convention()
    {
        Assert.Equal("MyApp.SomeValidationRule", new ValidateKeyedAttribute().Rule);
        Assert.Equal("Serenity.ComponentModel.EvenValidationRule", new ValidateEvenAttribute().Rule);
        Assert.Equal("Serenity.ComponentModel.EvenValidationRule", ValidationRuleAttribute.GetConventionalRule(typeof(ValidateEvenAttribute)));
        Assert.Equal("Range", ValidateRangeAttribute.Key);
    }

    [Fact]
    public void The_Message_Of_A_DataAnnotations_Attribute_Is_Not_A_Key_And_Is_Left_Out_When_It_Has_The_Name_Of_The_Field()
    {
        Assert.Equal("Too big", ValidationRules.GetClientRule(new RangeAttribute(1, 2) { ErrorMessage = "Too big" })!.Message);
        Assert.Null(ValidationRules.GetClientRule(new RangeAttribute(1, 2) { ErrorMessage = "{0} is too big" })!.Message);
        Assert.Null(ValidationRules.GetClientRule(new RangeAttribute(1, 2) { ErrorMessage = "Too big" })!.TextKey);
    }

    [Fact]
    public void A_DataAnnotations_Attribute_Without_A_Message_Has_The_Localized_Default_On_The_Server()
    {
        var localizer = new MockTextLocalizer(key => key == "Validation.Range" ? "Between {0} and {1}" : null!);
        var services = new ServiceCollection().AddSingleton<ITextLocalizer>(localizer).BuildServiceProvider();
        var context = new ValidationContext(new object(), services, null) { MemberName = "Value", DisplayName = "Value" };

        Assert.Equal("Between 1 and 2", ValidationRules.Validate(new RangeAttribute(1, 2), 3, context));
        Assert.Equal("Value is out", ValidationRules.Validate(new RangeAttribute(1, 2) { ErrorMessage = "{0} is out" }, 3, context));
        Assert.NotNull(ValidationRules.Validate(new CreditCardAttribute(), "x", context));
    }
}
