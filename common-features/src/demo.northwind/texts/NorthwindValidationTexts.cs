namespace Serenity.Demo.Northwind;

[NestedLocalTexts(Prefix = "Validation.")]
public static class NorthwindValidationTexts
{
    public static readonly LocalText NorthwindPhone = "Phone numbers should be entered in format '(503) 555-9831'.";
    public static readonly LocalText NorthwindPhoneMultiple = "Phone numbers should be entered in format '(503) 555-9831. " +
        "Multiple numbers can be separated with comma.";
}