namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Orders")]
[DisplayName("Orders"), InstanceName("Order")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[DataAuditLog]
public sealed class OrderRow : Row<OrderRow.RowFields>, IIdRow, INameRow
{
    const string jCustomer = nameof(jCustomer);
    const string jEmployee = nameof(jEmployee);
    const string jShipVia = nameof(jShipVia);

    [DisplayName("Order ID"), NotNull, Identity, QuickSearch, IdProperty]
    public int? OrderID { get => fields.OrderID[this]; set => fields.OrderID[this] = value; }

    [DisplayName("Customer"), Size(5), NotNull, NameProperty]
    [ForeignKey(typeof(CustomerRow), nameof(CustomerRow.CustomerID)), LeftJoin(jCustomer)]
    public string CustomerID { get => fields.CustomerID[this]; set => fields.CustomerID[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.CompanyName)), DisplayName("Customer"), QuickSearch]
    public string CustomerCompanyName { get => fields.CustomerCompanyName[this]; set => fields.CustomerCompanyName[this] = value; }

    [DisplayName("Employee"), ForeignKey(typeof(EmployeeRow)), LeftJoin(jEmployee)]
    [TextualField(nameof(EmployeeFullName))]
    public int? EmployeeID { get => fields.EmployeeID[this]; set => fields.EmployeeID[this] = value; }

    [Origin(jEmployee, nameof(EmployeeRow.FullName)), DisplayName("Employee")]
    public string EmployeeFullName { get => fields.EmployeeFullName[this]; set => fields.EmployeeFullName[this] = value; }

    [Origin(jEmployee, nameof(EmployeeRow.Gender))]
    public Gender? EmployeeGender { get => fields.EmployeeGender[this]; set => fields.EmployeeGender[this] = value; }

    [DisplayName("Order Date"), NotNull]
    public DateTime? OrderDate { get => fields.OrderDate[this]; set => fields.OrderDate[this] = value; }

    [DisplayName("Required Date")]
    public DateTime? RequiredDate { get => fields.RequiredDate[this]; set => fields.RequiredDate[this] = value; }

    [DisplayName("Shipped Date")]
    public DateTime? ShippedDate { get => fields.ShippedDate[this]; set => fields.ShippedDate[this] = value; }

    [DisplayName("Shipping State"), Case($"T0.[{nameof(ShippedDate)}] IS NULL",
        (int)OrderShippingState.NotShipped, (int)OrderShippingState.Shipped)]
    public OrderShippingState? ShippingState { get => fields.ShippingState[this]; set => fields.ShippingState[this] = value; }

    [DisplayName("Ship Via"), ForeignKey(typeof(ShipperRow)), LeftJoin(jShipVia)]
    public int? ShipVia { get => fields.ShipVia[this]; set => fields.ShipVia[this] = value; }

    [DisplayName("Freight"), Scale(4), DisplayFormat("#,##0.####"), AlignRight]
    public decimal? Freight { get => fields.Freight[this]; set => fields.Freight[this] = value; }

    [DisplayName("Ship Name"), Size(40)]
    public string ShipName { get => fields.ShipName[this]; set => fields.ShipName[this] = value; }

    [DisplayName("Ship Address"), Size(60)]
    public string ShipAddress { get => fields.ShipAddress[this]; set => fields.ShipAddress[this] = value; }

    [DisplayName("Ship City"), Size(15)]
    public string ShipCity { get => fields.ShipCity[this]; set => fields.ShipCity[this] = value; }

    [DisplayName("Ship Region"), Size(15)]
    public string ShipRegion { get => fields.ShipRegion[this]; set => fields.ShipRegion[this] = value; }

    [DisplayName("Ship Postal Code"), Size(10)]
    public string ShipPostalCode { get => fields.ShipPostalCode[this]; set => fields.ShipPostalCode[this] = value; }

    [DisplayName("Ship Country"), Size(15)]
    public string ShipCountry { get => fields.ShipCountry[this]; set => fields.ShipCountry[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.ContactName))]
    public string CustomerContactName { get => fields.CustomerContactName[this]; set => fields.CustomerContactName[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.ContactTitle))]
    public string CustomerContactTitle { get => fields.CustomerContactTitle[this]; set => fields.CustomerContactTitle[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.City))]
    public string CustomerCity { get => fields.CustomerCity[this]; set => fields.CustomerCity[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.Region))]
    public string CustomerRegion { get => fields.CustomerRegion[this]; set => fields.CustomerRegion[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.Country))]
    public string CustomerCountry { get => fields.CustomerCountry[this]; set => fields.CustomerCountry[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.Phone))]
    public string CustomerPhone { get => fields.CustomerPhone[this]; set => fields.CustomerPhone[this] = value; }

    [Origin(jCustomer, nameof(CustomerRow.Fax))]
    public string CustomerFax { get => fields.CustomerFax[this]; set => fields.CustomerFax[this] = value; }

    [Origin(jShipVia, nameof(ShipperRow.CompanyName)), DisplayName("Ship Via")]
    public string ShipViaCompanyName { get => fields.ShipViaCompanyName[this]; set => fields.ShipViaCompanyName[this] = value; }

    [DisplayName("Details"), MasterDetailRelation(foreignKey: nameof(OrderDetailRow.OrderID)), NotMapped]
    public List<OrderDetailRow> DetailList { get => fields.DetailList[this]; set => fields.DetailList[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field OrderID;
        public StringField CustomerID;
        public Int32Field EmployeeID;
        public DateTimeField OrderDate;
        public DateTimeField RequiredDate;
        public DateTimeField ShippedDate;
        public Int32Field ShipVia;
        public DecimalField Freight;
        public StringField ShipName;
        public StringField ShipAddress;
        public StringField ShipCity;
        public StringField ShipRegion;
        public StringField ShipPostalCode;
        public StringField ShipCountry;

        public StringField CustomerCompanyName;
        public StringField CustomerContactName;
        public StringField CustomerContactTitle;
        public StringField CustomerCity;
        public StringField CustomerRegion;
        public StringField CustomerCountry;
        public StringField CustomerPhone;
        public StringField CustomerFax;

        public StringField EmployeeFullName;
        public EnumField<Gender> EmployeeGender;

        public StringField ShipViaCompanyName;

        public EnumField<OrderShippingState> ShippingState;
        public RowListField<OrderDetailRow> DetailList;
    }
}