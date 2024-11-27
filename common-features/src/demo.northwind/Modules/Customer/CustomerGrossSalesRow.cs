namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("GrossSales")]
[DisplayName("Customer Gross Sales")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class CustomerGrossSalesRow : Row<CustomerGrossSalesRow.RowFields>, INameRow
{
    [DisplayName("Customer Id"), Column("CustomerID"), NotNull]
    public string CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

    [DisplayName("Contact Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string ContactName { get => fields.ContactName[this]; set => fields.ContactName[this] = value; }

    [DisplayName("Product Id"), Column("ProductID"), NotNull]
    public int? ProductId { get => fields.ProductId[this]; set => fields.ProductId[this] = value; }

    [DisplayName("Product Name"), Size(40), NotNull, QuickSearch]
    public string ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [DisplayName("Gross Amount"), Size(19), Scale(2)]
    public decimal? GrossAmount { get => fields.GrossAmount[this]; set => fields.GrossAmount[this] = value; }


    public CustomerGrossSalesRow()
    {
    }

    public CustomerGrossSalesRow(RowFields fields)
        : base(fields)
    {
    }

    public class RowFields : RowFieldsBase
    {
        public StringField CustomerId;
        public StringField ContactName;
        public Int32Field ProductId;
        public StringField ProductName;
        public DecimalField GrossAmount;
    }
}