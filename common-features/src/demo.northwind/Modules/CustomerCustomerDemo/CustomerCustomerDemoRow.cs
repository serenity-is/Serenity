namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("CustomerCustomerDemo")]
[DisplayText("CustomerCustomerDemo"), InstanceName("CustomerCustomerDemo")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class CustomerCustomerDemoRow : Row<CustomerCustomerDemoRow.RowFields>, IIdRow, INameRow
{
    const string jCustomer = nameof(jCustomer);
    const string jCustomerType = nameof(jCustomerType);

    [DisplayText("Id"), Identity, IdProperty]
    public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

    [DisplayText("Customer Id"), Size(5), PrimaryKey, ForeignKey(typeof(CustomerRow), nameof(CustomerRow.CustomerID)), LeftJoin(jCustomer), QuickSearch, NameProperty]
    public string? CustomerID { get => fields.CustomerID[this]; set => fields.CustomerID[this] = value; }

    [DisplayText("Customer Type Id"), Size(10), PrimaryKey, ForeignKey(typeof(CustomerDemographicRow), nameof(CustomerDemographicRow.CustomerTypeID)), LeftJoin(jCustomerType)]
    public string? CustomerTypeID { get => fields.CustomerTypeID[this]; set => fields.CustomerTypeID[this] = value; }

    [DisplayText("Customer Company Name"), Origin(jCustomer, nameof(CustomerRow.CompanyName))]
    public string? CustomerCompanyName { get => fields.CustomerCompanyName[this]; set => fields.CustomerCompanyName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ID = null!;
        public StringField CustomerID = null!;
        public StringField CustomerTypeID = null!;
        public StringField CustomerCompanyName = null!;
    }
}