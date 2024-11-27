namespace Serenity.Demo.Northwind;

[NestedPermissionKeys]
[DisplayName("Northwind")]
public class PermissionKeys
{
    [DisplayName("Customers")]
    public class Customer
    {
        [ImplicitPermission(General), ImplicitPermission(View)]
        public const string Delete = "Northwind:Customer:Delete";
        [Description("Create/Update"), ImplicitPermission(General), ImplicitPermission(View)]
        public const string Modify = "Northwind:Customer:Modify";
        public const string View = "Northwind:Customer:View";
    }

    [Description("[General]")]
    public const string General = "Northwind:General";
}
