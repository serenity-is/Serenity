namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Customers")]
[DisplayName("Customers"), InstanceName("Customer")]
[ReadPermission(PermissionKeys.Customer.View)]
[ModifyPermission(PermissionKeys.Customer.Modify)]
[DeletePermission(PermissionKeys.Customer.Delete)]
[LeftJoin("cd", "CustomerDetails", "cd.[CustomerID] = T0.[CustomerID]", RowType = typeof(CustomerDetailsRow), TitlePrefix = "")]
[UpdatableExtension("cd", typeof(CustomerDetailsRow), CascadeDelete = true)]
[LookupScript(typeof(Lookups.CustomerLookup))]
public sealed class CustomerRow : Row<CustomerRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Customer Id"), Size(5), PrimaryKey, IdProperty, NotNull, QuickSearch, Updatable(false), LookupInclude]
    public string? CustomerID { get => fields.CustomerID[this]; set => fields.CustomerID[this] = value; }

    [DisplayName("Company Name"), Size(40), NotNull, QuickSearch, LookupInclude, NameProperty]
    public string? CompanyName { get => fields.CompanyName[this]; set => fields.CompanyName[this] = value; }

    [DisplayName("Contact Name"), Size(30), QuickSearch]
    public string? ContactName { get => fields.ContactName[this]; set => fields.ContactName[this] = value; }

    [DisplayName("Contact Title"), Size(30)]
    public string? ContactTitle { get => fields.ContactTitle[this]; set => fields.ContactTitle[this] = value; }

    [DisplayName("Address"), Size(60)]
    public string? Address { get => fields.Address[this]; set => fields.Address[this] = value; }

    [DisplayName("City"), Size(15), AsyncLookupEditor(typeof(Lookups.CustomerCityLookup), CascadeFrom = "Country", AutoComplete = true)]
    public string? City { get => fields.City[this]; set => fields.City[this] = value; }

    [DisplayName("Region"), Size(15)]
    public string? Region { get => fields.Region[this]; set => fields.Region[this] = value; }

    [DisplayName("Postal Code"), Size(10)]
    public string? PostalCode { get => fields.PostalCode[this]; set => fields.PostalCode[this] = value; }

    [DisplayName("Country"), Size(15), AsyncLookupEditor(typeof(Lookups.CustomerCountryLookup), AutoComplete = true)]
    public string? Country { get => fields.Country[this]; set => fields.Country[this] = value; }

    [DisplayName("Phone"), Size(24)]
    public string? Phone { get => fields.Phone[this]; set => fields.Phone[this] = value; }

    [DisplayName("Fax"), Size(24)]
    public string? Fax { get => fields.Fax[this]; set => fields.Fax[this] = value; }

    [Origin("cd")]
    public DateTime? LastContactDate { get => fields.LastContactDate[this]; set => fields.LastContactDate[this] = value; }

    [Origin("cd"), AsyncLookupEditor(typeof(EmployeeRow))]
    public int? LastContactedBy { get => fields.LastContactedBy[this]; set => fields.LastContactedBy[this] = value; }

    [Origin("cd"), EmailAddressEditor]
    public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }

    [Origin("cd"), DefaultValue(false)]
    public bool? SendBulletin { get => fields.SendBulletin[this]; set => fields.SendBulletin[this] = value; }

    [NotesEditor, NotMapped]
    public List<NoteRow>? NoteList { get => fields.NoteList[this]; set => fields.NoteList[this] = value; }
    
    [DisplayName("Representatives"), AsyncLookupEditor(typeof(EmployeeRow), Multiple = true), NotMapped]
    [LinkingSetRelation(typeof(CustomerRepresentativesRow), "CustomerId", "EmployeeId")]
    [MinSelectLevel(SelectLevel.Details), QuickFilter(CssClass = "hidden-xs")]
    public List<int>? Representatives { get => fields.Representatives[this]; set => fields.Representatives[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public StringField CustomerID = null!;
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
        public RowListField<NoteRow> NoteList = null!;
        public ListField<int> Representatives = null!;
        public DateTimeField LastContactDate = null!;
        public Int32Field LastContactedBy = null!;
        public StringField Email = null!;
        public BooleanField SendBulletin = null!;
    }
}