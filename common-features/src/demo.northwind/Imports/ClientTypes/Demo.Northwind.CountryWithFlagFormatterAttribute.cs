namespace Serenity.Demo.Northwind;

public partial class CountryWithFlagFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.Northwind.CountryWithFlagFormatter";

    public CountryWithFlagFormatterAttribute()
        : base(Key)
    {
    }
}