namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("ProductLog")]
public sealed class ProductLogRow : Row<ProductLogRow.RowFields>, ICaptureLogRow
{
    [Identity, IdProperty]
    public long? ProductLogID { get => fields.ProductLogID[this]; set => fields.ProductLogID[this] = value; }

    public CaptureOperationType? OperationType { get => fields.OperationType[this]; set => fields.OperationType[this] = value; }

    public int? ChangingUserId { get => fields.ChangingUserId[this]; set => fields.ChangingUserId[this] = value; }

    public DateTime? ValidFrom { get => fields.ValidFrom[this]; set => fields.ValidFrom[this] = value; }

    public DateTime? ValidUntil { get => fields.ValidUntil[this]; set => fields.ValidUntil[this] = value; }

    [NotNull]
    public int? ProductID { get => fields.ProductID[this]; set => fields.ProductID[this] = value; }

    [Size(40)]
    public string? ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [Size(100)]
    public string? ProductImage { get => fields.ProductImage[this]; set => fields.ProductImage[this] = value; }

    public bool? Discontinued { get => fields.Discontinued[this]; set => fields.Discontinued[this] = value; }

    public int? SupplierID { get => fields.SupplierID[this]; set => fields.SupplierID[this] = value; }

    public int? CategoryID { get => fields.CategoryID[this]; set => fields.CategoryID[this] = value; }

    public string? QuantityPerUnit { get => fields.QuantityPerUnit[this]; set => fields.QuantityPerUnit[this] = value; }

    [Scale(4)]
    public decimal? UnitPrice { get => fields.UnitPrice[this]; set => fields.UnitPrice[this] = value; }

    public short? UnitsInStock { get => fields.UnitsInStock[this]; set => fields.UnitsInStock[this] = value; }

    public short? UnitsOnOrder { get => fields.UnitsOnOrder[this]; set => fields.UnitsOnOrder[this] = value; }

    public short? ReorderLevel { get => fields.ReorderLevel[this]; set => fields.ReorderLevel[this] = value; }

    EnumField<CaptureOperationType> ICaptureLogRow.OperationTypeField => fields.OperationType;
    Field ICaptureLogRow.ChangingUserIdField => fields.ChangingUserId;
    DateTimeField ICaptureLogRow.ValidFromField => fields.ValidFrom;
    DateTimeField ICaptureLogRow.ValidUntilField => fields.ValidUntil;

    public class RowFields : RowFieldsBase
    {
        public Int64Field ProductLogID = null!;
        public EnumField<CaptureOperationType> OperationType = null!;
        public Int32Field ChangingUserId = null!;
        public DateTimeField ValidFrom = null!;
        public DateTimeField ValidUntil = null!;

        public Int32Field ProductID = null!;
        public StringField ProductName = null!;
        public StringField ProductImage = null!;
        public BooleanField Discontinued = null!;
        public Int32Field SupplierID = null!;
        public Int32Field CategoryID = null!;
        public StringField QuantityPerUnit = null!;
        public DecimalField UnitPrice = null!;
        public Int16Field UnitsInStock = null!;
        public Int16Field UnitsOnOrder = null!;
        public Int16Field ReorderLevel = null!;
    }
}