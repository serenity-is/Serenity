namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("CustomerDemographics")]
[DisplayText("CustomerDemographics"), InstanceName("CustomerDemographics")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class CustomerDemographicRow : Row<CustomerDemographicRow.RowFields>, IIdRow, INameRow
{
    [DisplayText("Id"), Identity, IdProperty]
    public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

    [DisplayText("Customer Type Id"), Size(10), PrimaryKey, QuickSearch, NameProperty]
    public string? CustomerTypeID { get => fields.CustomerTypeID[this]; set => fields.CustomerTypeID[this] = value; }

    [DisplayText("Customer Desc")]
    public string? CustomerDesc { get => fields.CustomerDesc[this]; set => fields.CustomerDesc[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ID = null!;
        public StringField CustomerTypeID = null!;
        public StringField CustomerDesc = null!;
    }
}