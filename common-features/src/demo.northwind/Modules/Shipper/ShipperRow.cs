namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("Shippers")]
[DisplayText("Shippers"), InstanceName("Shipper")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class ShipperRow : Row<ShipperRow.RowFields>, IIdRow, INameRow
{
    [DisplayText("Shipper Id"), Identity, IdProperty]
    public int? ShipperID { get => fields.ShipperID[this]; set => fields.ShipperID[this] = value; }

    [DisplayText("Company Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string? CompanyName { get => fields.CompanyName[this]; set => fields.CompanyName[this] = value; }

    [DisplayText("Phone"), Size(24)]
    public string? Phone { get => fields.Phone[this]; set => fields.Phone[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ShipperID = null!;
        public StringField CompanyName = null!;
        public StringField Phone = null!;
    }
}