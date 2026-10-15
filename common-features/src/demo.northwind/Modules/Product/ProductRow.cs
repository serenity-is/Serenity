namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("Products")]
[DisplayText("Products"), InstanceName("Product")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
[CaptureLog(typeof(ProductLogRow))]
[LocalizationRow(typeof(ProductLangRow), LocalizeListByDefault = true)]
public sealed class ProductRow : Row<ProductRow.RowFields>, IIdRow, INameRow, IConcurrencyVersionRow
{
    [DisplayText("Product Id"), Identity, LookupInclude, IdProperty]
    public int? ProductID { get => fields.ProductID[this]; set => fields.ProductID[this] = value; }

    [DisplayText("Product Name"), Size(40), NotNull, QuickSearch, LookupInclude, NameProperty]
    public string? ProductName { get => fields.ProductName[this]; set => fields.ProductName[this] = value; }

    [DisplayText("Product Image"), Size(100)]
    [ImageUploadEditor(FilenameFormat = "ProductImage/~", CopyToHistory = true)]
    public string? ProductImage { get => fields.ProductImage[this]; set => fields.ProductImage[this] = value; }

    [DisplayText("Discontinued"), NotNull, DefaultValue(false)]
    public bool? Discontinued { get => fields.Discontinued[this]; set => fields.Discontinued[this] = value; }

    [DisplayText("Supplier"), NotNull, ForeignKey(typeof(SupplierRow)), LeftJoin("sup")]
    [LookupEditor(typeof(SupplierRow), InplaceAdd = true, DialogType = "Demo.Northwind.SupplierDialog", Async = true)]
    public int? SupplierID { get => fields.SupplierID[this]; set => fields.SupplierID[this] = value; }

    [DisplayText("Category"), NotNull, ForeignKey(typeof(CategoryRow)), LeftJoin("cat"), LookupInclude]
    [ServiceLookupEditor(typeof(CategoryRow), InplaceAdd = true, DialogType = "Demo.Northwind.CategoryDialog")]
    public int? CategoryID { get => fields.CategoryID[this]; set => fields.CategoryID[this] = value; }

    [DisplayText("Quantity Per Unit"), Size(20)]
    public string? QuantityPerUnit { get => fields.QuantityPerUnit[this]; set => fields.QuantityPerUnit[this] = value; }

    [DisplayText("Unit Price"), Scale(4), LookupInclude, SummaryType(SummaryType.Avg)]
    public decimal? UnitPrice { get => fields.UnitPrice[this]; set => fields.UnitPrice[this] = value; }

    [DisplayText("Units In Stock"), NotNull, DefaultValue(0), LookupInclude]
    public short? UnitsInStock { get => fields.UnitsInStock[this]; set => fields.UnitsInStock[this] = value; }

    [DisplayText("Units On Order"), NotNull, DefaultValue(0)]
    public short? UnitsOnOrder { get => fields.UnitsOnOrder[this]; set => fields.UnitsOnOrder[this] = value; }

    [DisplayText("Reorder Level"), NotNull, DefaultValue(0)]
    public short? ReorderLevel { get => fields.ReorderLevel[this]; set => fields.ReorderLevel[this] = value; }

    [Origin("sup"), DisplayText("Supplier"), LookupInclude]
    public string? SupplierCompanyName { get => fields.SupplierCompanyName[this]; set => fields.SupplierCompanyName[this] = value; }

    [Origin("sup")]
    public string? SupplierCountry { get => fields.SupplierCountry[this]; set => fields.SupplierCountry[this] = value; }

    [Origin("cat"), DisplayText("Category")]
    public string? CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayText("Row Version"), Insertable(false), Updatable(false)]
    public int? RowVersion { get => fields.RowVersion[this]; set => fields.RowVersion[this] = value; }

    Field IConcurrencyVersionRow.ConcurrencyVersionField => fields.RowVersion;

    public class RowFields : RowFieldsBase
    {
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
        public Int32Field RowVersion = null!;

        public StringField SupplierCompanyName = null!;
        public StringField SupplierCountry = null!;

        public StringField CategoryName = null!;
    }
}