namespace Serenity.Demo.Northwind;

[NestedPermissionKeys]
[DisplayText("Northwind")]
public class PermissionKeys
{
    [DisplayText("Customers")]
    public class Customer
    {
        [ImplicitPermission(General), ImplicitPermission(View)]
        public const string Delete = "Northwind:Customer:Delete";
        [DescriptionText("Create/Update"), ImplicitPermission(General), ImplicitPermission(View)]
        public const string Modify = "Northwind:Customer:Modify";
        public const string View = "Northwind:Customer:View";
    }

    [DescriptionText("[General]")]
    public const string General = "Northwind:General";
}
