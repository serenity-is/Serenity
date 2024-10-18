namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("[Order Details]")]
[DisplayName("Order Details"), InstanceName("Order Detail")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[DataAuditLog]
public sealed class OrderDetailRow : Row<OrderDetailRow.RowFields>, IIdRow
{
    const string jOrder = nameof(jOrder);
    const string jProduct = nameof(jProduct);

    [DisplayName("ID"), Identity, IdProperty]
    public int? DetailID { get => fields.DetailID[this]; set => fields.DetailID[this] = value; }

    [DisplayName("Order Id"), PrimaryKey, ForeignKey(typeof(OrderRow)), LeftJoin(jOrder), Updatable(false)]
    public int? OrderID { get => fields.OrderID[this]; set => fields.OrderID[this] = value; }

    [DisplayName("Product"), PrimaryKey, NotNull, ForeignKey(typeof(ProductRow)), LeftJoin(jProduct)]
    [AsyncLookupEditor(typeof(ProductRow))]
    public int? ProductID { get => fields.ProductID[this]; set => fields.ProductID[this] = value; }

    [DisplayName("Unit Price"), Scale(4), NotNull, AlignRight, DisplayFormat("#,##0.00")]
    public decimal? UnitPrice { get => fields.UnitPrice[this]; set => fields.UnitPrice[this] = value; }

    [DisplayName("Quantity"), NotNull, DefaultValue(1), AlignRight]
    public short? Quantity { get => fields.Quantity[this]; set => fields.Quantity[this] = value; }

    [DisplayName("Discount"), NotNull, DefaultValue(0), AlignRight, DisplayFormat("#,##0.00")]
    public float? Discount { get => fields.Discount[this]; set => fields.Discount[this] = value; }

    [DisplayName("Line Total"), Expression("(T0.[UnitPrice] * T0.[Quantity] - T0.[Discount])")]
    [AlignRight, DisplayFormat("#,##0.00"), MinSelectLevel(SelectLevel.List)]
    public decimal? LineTotal { get => fields.LineTotal[this]; set => fields.LineTotal[this] = value; }

    [Origin(jOrder, nameof(OrderRow.CustomerID))]
    public string OrderCustomerID { get => fields.OrderCustomerID[this]; set => fields.OrderCustomerID[this] = value; }

    [Origin(jOrder, nameof(OrderRow.EmployeeID))]
    public int? OrderEmployeeID { get => fields.OrderEmployeeID[this]; set => fields.OrderEmployeeID[this] = value; }

    [Origin(jOrder, nameof(OrderRow.OrderDate))]
    public DateTime? OrderDate { get => fields.OrderDate[this]; set => fields.OrderDate[this] = value; }

    [Origin(jProduct, nameof(ProductRow.ProductName)), MinSelectLevel(SelectLevel.List)]
    public string ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field DetailID;
        public Int32Field OrderID;
        public Int32Field ProductID;
        public DecimalField UnitPrice;
        public Int16Field Quantity;
        public SingleField Discount;

        public StringField OrderCustomerID;

        public Int32Field OrderEmployeeID;
        public DateTimeField OrderDate;

        public StringField ProductName;

        public DecimalField LineTotal;
    }
}