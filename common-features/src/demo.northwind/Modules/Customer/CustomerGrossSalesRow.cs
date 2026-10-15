namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("GrossSales")]
[DisplayText("Customer Gross Sales")]
[ReadPermission("Northwind:General")]
[ModifyPermission("Northwind:General")]
public sealed class CustomerGrossSalesRow : Row<CustomerGrossSalesRow.RowFields>, INameRow
{
    [DisplayText("Customer Id"), Column("CustomerID"), NotNull]
    public string? CustomerId { get => fields.CustomerId[this]; set => fields.CustomerId[this] = value; }

    [DisplayText("Contact Name"), Size(40), NotNull, QuickSearch, NameProperty]
    public string? ContactName { get => fields.ContactName[this]; set => fields.ContactName[this] = value; }

    [DisplayText("Product Id"), Column("ProductID"), NotNull]
    public int? ProductId { get => fields.ProductId[this]; set => fields.ProductId[this] = value; }

    [DisplayText("Product Name"), Size(40), NotNull, QuickSearch]
    public string? ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [DisplayText("Gross Amount"), Size(19), Scale(2)]
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
        public StringField CustomerId = null!;
        public StringField ContactName = null!;
        public Int32Field ProductId = null!;
        public StringField ProductName = null!;
        public DecimalField GrossAmount = null!;
    }
}