namespace Serenity.Demo.Northwind;

public partial class CustomerEditorAttribute : LookupEditorBaseAttribute
{
    public const string Key = "Serenity.Demo.Northwind.CustomerEditor";

    public CustomerEditorAttribute()
        : base(Key)
    {
    }
}