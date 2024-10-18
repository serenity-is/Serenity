namespace Serenity.Demo.Northwind;

public partial class ShipperFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.Northwind.ShipperFormatter";

    public ShipperFormatterAttribute()
        : base(Key)
    {
    }
}