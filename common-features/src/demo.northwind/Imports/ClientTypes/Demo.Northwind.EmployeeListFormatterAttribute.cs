namespace Serenity.Demo.Northwind;

public partial class EmployeeListFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.Northwind.EmployeeListFormatter";

    public EmployeeListFormatterAttribute()
        : base(Key)
    {
    }
}