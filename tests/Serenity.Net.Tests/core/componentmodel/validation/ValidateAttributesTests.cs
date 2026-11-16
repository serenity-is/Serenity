using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Serenity.ComponentModel;

public class ValidateAttributesTests
{
    private class Model
    {
        public string? Password { get; set; }
        public string? Confirm { get; set; }
    }

    private static ValidationContext Context(object? instance = null, ITextLocalizer? localizer = null)
    {
        var services = new ServiceCollection();
        if (localizer is not null)
            services.AddSingleton(localizer);
        return new ValidationContext(instance ?? new Model(), services.BuildServiceProvider(), null) { MemberName = "Value" };
    }

    private static string? Check(ValidationAttribute attribute, object? value, ValidationContext? context = null) =>
        ValidationRules.Validate(attribute, value, context ?? Context());

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData(5, true)]
    [InlineData("10", true)]
    [InlineData(0, false)]
    [InlineData(11, false)]
    public void Range_Includes_Its_Limits_And_Skips_Empty(object? value, bool valid)
    {
        Assert.Equal(valid, Check(new ValidateRangeAttribute(1, 10), value) is null);
    }

    [Fact]
    public void Range_Has_The_Default_Message_With_Its_Limits()
    {
        Assert.Equal("Please enter a value between 1 and 10.", Check(new ValidateRangeAttribute(1, 10), 11));
    }

    [Fact]
    public void Range_Parses_Its_Limits_In_The_Invariant_Culture()
    {
        var old = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
        try
        {
            var range = new ValidateRangeAttribute(typeof(decimal), "1.5", "2.5");
            Assert.Null(Check(range, 2m));
            Assert.NotNull(Check(range, 3m));
        }
        finally
        {
            CultureInfo.CurrentCulture = old;
        }
    }

    [Fact]
    public void Message_Is_The_Text_Of_The_Key_Then_The_Own_Message_Then_The_Localized_Default()
    {
        var localizer = new MockTextLocalizer(key => key switch
        {
            "My.Key" => "From key {0}",
            "Validation.Range" => "Localized {0}-{1}",
            _ => null!
        });
        var context = Context(localizer: localizer);

        Assert.Equal("From key 1", Check(new ValidateRangeAttribute(1, 10) { TextKey = "My.Key", ErrorMessage = "Own" }, 0, context));
        Assert.Equal("Own", Check(new ValidateRangeAttribute(1, 10) { TextKey = "Missing.Key", ErrorMessage = "Own" }, 0, context));
        Assert.Equal("Localized 1-10", Check(new ValidateRangeAttribute(1, 10), 0, context));
    }

    [Theory]
    [InlineData("abc", true)]
    [InlineData("", true)]
    [InlineData("abcdef", false)]
    public void Length_Checks_The_Maximum(string value, bool valid)
    {
        Assert.Equal(valid, Check(new ValidateLengthAttribute(5), value) is null);
    }

    [Fact]
    public void Length_Checks_The_Minimum_With_Its_Own_Message()
    {
        var length = new ValidateLengthAttribute(5) { MinimumLength = 2 };
        Assert.Equal("Please enter a value between 2 and 5 characters long.", Check(length, "a"));
        Assert.Equal("Please enter no more than 5 characters.", Check(new ValidateLengthAttribute(5), "abcdef"));
        Assert.Null(Check(length, ""));
    }

    [Theory]
    [InlineData("AB12", true)]
    [InlineData("", true)]
    [InlineData("AB123", false)]
    [InlineData("xAB12", false)]
    public void Pattern_Matches_The_Whole_Text(string value, bool valid)
    {
        Assert.Equal(valid, Check(new ValidatePatternAttribute("[A-Z]{2}[0-9]{2}"), value) is null);
    }

    [Fact]
    public void Pattern_Uses_ECMAScript_Semantics()
    {
        // \d is only ASCII digits in ECMAScript, while .NET also takes the digits of other scripts
        Assert.NotNull(Check(new ValidatePatternAttribute(@"\d+"), "١٢٣"));
        Assert.Null(Check(new ValidatePatternAttribute(@"\d+"), "123"));
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData(null, true)]
    [InlineData("a@b", true)]
    [InlineData("invalid", false)]
    [InlineData("a@@b", false)]
    [InlineData("user@-example.com", false)]
    public void Email_Uses_The_Pattern(string? value, bool valid)
    {
        Assert.Equal(valid, Check(new ValidateEmailAttribute(), value) is null);
    }

    [Theory]
    [InlineData("https://example.com/a?b=1", true)]
    [InlineData("ftp://example.com", true)]
    [InlineData(null, true)]
    [InlineData("http://", false)]
    [InlineData("example.com", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("http://exa mple.com", false)]
    public void Url_Requires_An_Absolute_Address_With_A_Host(string? value, bool valid)
    {
        Assert.Equal(valid, Check(new ValidateUrlAttribute(), value) is null);
    }

    [Fact]
    public void Email_And_Url_Have_Their_Data_Types()
    {
        Assert.Equal(DataType.EmailAddress, new ValidateEmailAttribute().DataType);
        Assert.Equal(DataType.Url, new ValidateUrlAttribute().DataType);
    }

    [Fact]
    public void Compare_Checks_The_Other_Property()
    {
        var compare = new ValidateCompareAttribute(nameof(Model.Password));
        Assert.Null(Check(compare, "x", Context(new Model { Password = "x" })));
        Assert.Null(Check(compare, "", Context(new Model { Password = null })));
        Assert.Equal("Please enter the same value again.", Check(compare, "y", Context(new Model { Password = "x" })));
    }

    [Fact]
    public void A_Rule_Only_Checked_By_The_Forms_Is_Not_Checked_On_The_Server()
    {
        Assert.Null(Check(new ValidateRangeAttribute(1, 10) { Side = ValidationSide.Client }, 11));
    }

    private class ValidateEvenAttribute : ValidationRuleAttribute
    {
        public override string Rule => "even";
        protected override string DefaultTextKey => "Validation.Even";
        protected override bool IsValidValue(object? value, ValidationContext context) =>
            Convert.ToInt64(value, CultureInfo.InvariantCulture) % 2 == 0;
    }

    [Fact]
    public void A_Custom_Rule_Skips_Empty_Values_And_Uses_Its_Default_Key()
    {
        var localizer = new MockTextLocalizer(key => key == "Validation.Even" ? "Must be even" : null!);
        Assert.Null(Check(new ValidateEvenAttribute(), null));
        Assert.Null(Check(new ValidateEvenAttribute(), 4));
        Assert.Equal("Must be even", Check(new ValidateEvenAttribute(), 3, Context(localizer: localizer)));
        Assert.Equal("Validation.Even", Check(new ValidateEvenAttribute(), 3));
    }
}
