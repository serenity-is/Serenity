
namespace BasicApplication.Northwind.Forms
{
    using Serenity.ComponentModel;
    using System;
    using System.ComponentModel;

    [ColumnsScript("Northwind.Product")]
    [BasedOnRow(typeof(Entities.ProductRow))]
    public class ProductColumns
    {
        [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
        public String ProductID { get; set; }
        [EditLink, Width(250)]
        public String ProductName { get; set; }
        public Boolean Discontinued { get; set; }
        [EditLink(ItemType = "Northwind.Supplier", IdField = "SupplierID")]
        public String SupplierCompanyName { get; set; }
        [EditLink(ItemType = "Northwind.Category", IdField = "CategoryID"), Width(150)]
        public String CategoryName { get; set; }
        [Width(130)]
        public String QuantityPerUnit { get; set; }
        [Width(80), AlignRight]
        public Decimal UnitPrice { get; set; }
        [Width(80), AlignRight]
        public Int16 UnitsInStock { get; set; }
        [Width(80), AlignRight]
        public Int16 UnitsOnOrder { get; set; }
        [Width(80), AlignRight]
        public Int16 ReorderLevel { get; set; }
    }
}