namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Shippers")]
[DisplayName("Shippers"), InstanceName("Shipper")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class ShipperRow : Row<ShipperRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Shipper Id"), Identity, IdProperty]
    public int? ShipperID { get => fields.ShipperID[this]; set => fields.ShipperID[this] = value; }

    [DisplayName("Company Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string CompanyName { get => fields.CompanyName[this]; set => fields.CompanyName[this] = value; }

    [DisplayName("Phone"), Size(24)]
    public string Phone { get => fields.Phone[this]; set => fields.Phone[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ShipperID;
        public StringField CompanyName;
        public StringField Phone;
    }
}