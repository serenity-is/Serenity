namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("CustomerDemographics")]
[DisplayName("CustomerDemographics"), InstanceName("CustomerDemographics")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class CustomerDemographicRow : Row<CustomerDemographicRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Id"), Identity, IdProperty]
    public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

    [DisplayName("Customer Type Id"), Size(10), PrimaryKey, QuickSearch, NameProperty]
    public string CustomerTypeID { get => fields.CustomerTypeID[this]; set => fields.CustomerTypeID[this] = value; }

    [DisplayName("Customer Desc")]
    public string CustomerDesc { get => fields.CustomerDesc[this]; set => fields.CustomerDesc[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ID;
        public StringField CustomerTypeID;
        public StringField CustomerDesc;
    }
}