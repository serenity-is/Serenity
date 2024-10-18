namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Suppliers")]
[DisplayName("Suppliers"), InstanceName("Supplier")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class SupplierRow : Row<SupplierRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Supplier Id"), Identity, IdProperty]
    public int? SupplierID { get => fields.SupplierID[this]; set => fields.SupplierID[this] = value; }

    [DisplayName("Company Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string CompanyName { get => fields.CompanyName[this]; set => fields.CompanyName[this] = value; }

    [DisplayName("Contact Name"), Size(30)]
    public string ContactName { get => fields.ContactName[this]; set => fields.ContactName[this] = value; }

    [DisplayName("Contact Title"), Size(30)]
    public string ContactTitle { get => fields.ContactTitle[this]; set => fields.ContactTitle[this] = value; }

    [DisplayName("Address"), Size(60)]
    public string Address { get => fields.Address[this]; set => fields.Address[this] = value; }

    [DisplayName("City"), Size(15)]
    public string City { get => fields.City[this]; set => fields.City[this] = value; }

    [DisplayName("Region"), Size(15)]
    public string Region { get => fields.Region[this]; set => fields.Region[this] = value; }

    [DisplayName("Postal Code"), Size(10)]
    public string PostalCode { get => fields.PostalCode[this]; set => fields.PostalCode[this] = value; }

    [DisplayName("Country"), Size(15), LookupFiltering("Northwind.SupplierCountry")]
    public string Country { get => fields.Country[this]; set => fields.Country[this] = value; }

    [DisplayName("Phone"), Size(24)]
    public string Phone { get => fields.Phone[this]; set => fields.Phone[this] = value; }

    [DisplayName("Fax"), Size(24)]
    public string Fax { get => fields.Fax[this]; set => fields.Fax[this] = value; }

    [DisplayName("Home Page")]
    public string HomePage { get => fields.HomePage[this]; set => fields.HomePage[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field SupplierID;
        public StringField CompanyName;
        public StringField ContactName;
        public StringField ContactTitle;
        public StringField Address;
        public StringField City;
        public StringField Region;
        public StringField PostalCode;
        public StringField Country;
        public StringField Phone;
        public StringField Fax;
        public StringField HomePage;
    }
}