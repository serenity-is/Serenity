namespace Serenity.Demo.Northwind;

public partial class OrderDetailsEditorAttribute : CustomEditorAttribute
{
    public const string Key = "Serenity.Demo.Northwind.OrderDetailsEditor";

    public OrderDetailsEditorAttribute()
        : base(Key)
    {
    }
}