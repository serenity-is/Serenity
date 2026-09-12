namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("CustomerDetails")]
[DisplayName("CustomerDetails"), InstanceName("CustomerDetails")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class CustomerDetailsRow : Row<CustomerDetailsRow.RowFields>, IIdRow, INameRow
{
    const string jLastContactedBy = nameof(jLastContactedBy);

    [DisplayName("Customer"), Size(5), PrimaryKey, IdProperty]
    public string? CustomerID { get => fields.CustomerID[this]; set => fields.CustomerID[this] = value; }

    [DisplayName("Last Contact Date")]
    public DateTime? LastContactDate { get => fields.LastContactDate[this]; set => fields.LastContactDate[this] = value; }

    [DisplayName("Last Contacted By"), ForeignKey(typeof(EmployeeRow)), LeftJoin(jLastContactedBy), TextualField(nameof(LastContactedByFullName))]
    public int? LastContactedBy { get => fields.LastContactedBy[this]; set => fields.LastContactedBy[this] = value; }

    [DisplayName("Last Contacted By"), Origin(jLastContactedBy, nameof(EmployeeRow.FullName))]
    public string? LastContactedByFullName { get => fields.LastContactedByFullName[this]; set => fields.LastContactedByFullName[this] = value; }

    [DisplayName("Email"), Size(100), QuickSearch, NameProperty]
    public string? Email { get => fields.Email[this]; set => fields.Email[this] = value; }

    [DisplayName("Send Bulletin"), NotNull]
    public bool? SendBulletin { get => fields.SendBulletin[this]; set => fields.SendBulletin[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public StringField CustomerID = null!;
        public DateTimeField LastContactDate = null!;
        public Int32Field LastContactedBy = null!;
        public StringField Email = null!;
        public BooleanField SendBulletin = null!;

        public StringField LastContactedByFullName = null!;
    }
}