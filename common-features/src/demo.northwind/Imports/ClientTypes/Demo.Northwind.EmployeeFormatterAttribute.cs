namespace Serenity.Demo.Northwind;

public partial class EmployeeFormatterAttribute : CustomFormatterAttribute
{
    public const string Key = "Serenity.Demo.Northwind.EmployeeFormatter";

    public EmployeeFormatterAttribute()
        : base(Key)
    {
    }

    public string GenderProperty
    {
        get { return GetOption<string>("genderProperty"); }
        set { SetOption("genderProperty", value); }
    }
}