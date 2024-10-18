namespace Serenity.Demo.Northwind;

public partial class FreightFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.Northwind.FreightFormatter";

    public FreightFormatterAttribute()
        : base(Key)
    {
    }
}