namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("Suppliers")]
[DisplayText("Suppliers"), InstanceName("Supplier")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class SupplierRow : Row<SupplierRow.RowFields>, IIdRow, INameRow
{
    [DisplayText("Supplier Id"), Identity, IdProperty]
    public int? SupplierID { get => fields.SupplierID[this]; set => fields.SupplierID[this] = value; }

    [DisplayText("Company Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string? CompanyName { get => fields.CompanyName[this]; set => fields.CompanyName[this] = value; }

    [DisplayText("Contact Name"), Size(30)]
    public string? ContactName { get => fields.ContactName[this]; set => fields.ContactName[this] = value; }

    [DisplayText("Contact Title"), Size(30)]
    public string? ContactTitle { get => fields.ContactTitle[this]; set => fields.ContactTitle[this] = value; }

    [DisplayText("Address"), Size(60)]
    public string? Address { get => fields.Address[this]; set => fields.Address[this] = value; }

    [DisplayText("City"), Size(15)]
    public string? City { get => fields.City[this]; set => fields.City[this] = value; }

    [DisplayText("Region"), Size(15)]
    public string? Region { get => fields.Region[this]; set => fields.Region[this] = value; }

    [DisplayText("Postal Code"), Size(10)]
    public string? PostalCode { get => fields.PostalCode[this]; set => fields.PostalCode[this] = value; }

    [DisplayText("Country"), Size(15), LookupFiltering("Northwind.SupplierCountry")]
    public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }

    [DisplayText("Phone"), Size(24)]
    public string? Phone { get => fields.Phone[this]; set => fields.Phone[this] = value; }

    [DisplayText("Fax"), Size(24)]
    public string? Fax { get => fields.Fax[this]; set => fields.Fax[this] = value; }

    [DisplayText("Home Page")]
    public string? HomePage { get => fields.HomePage[this]; set => fields.HomePage[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field SupplierID = null!;
        public StringField CompanyName = null!;
        public StringField ContactName = null!;
        public StringField ContactTitle = null!;
        public StringField Address = null!;
        public StringField City = null!;
        public StringField Region = null!;
        public StringField PostalCode = null!;
        public StringField Country = null!;
        public StringField Phone = null!;
        public StringField Fax = null!;
        public StringField HomePage = null!;
    }
}