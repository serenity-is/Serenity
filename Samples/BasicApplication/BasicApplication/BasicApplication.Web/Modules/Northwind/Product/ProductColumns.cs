
namespace BasicApplication.Northwind.Forms
{
    using Serenity.ComponentModel;
    using System;
    using System.ComponentModel;

    [ColumnsScript("Northwind.Product")]
    [BasedOnRow(typeof(Entities.ProductRow))]
    public class ProductColumns
    {
        [EditLink, CssClass("align-right"), DisplayName("Db.Shared.RecordId"), Width(55)]
        public String ProductID { get; set; }
        [EditLink, Width(250)]
        public String ProductName { get; set; }
        [Width(40)]
        public Boolean Discontinued { get; set; }
        [Width(200)]
        public String SupplierCompanyName { get; set; }
        [Width(200)]
        public String CategoryName { get; set; }
        [Width(130)]
        public String QuantityPerUnit { get; set; }
        [CssClass("align-right"), Width(80)]
        public Decimal UnitPrice { get; set; }
        [CssClass("align-right"), Width(80)]
        public Int16 UnitsInStock { get; set; }
        [CssClass("align-right"), Width(80)]
        public Int16 UnitsOnOrder { get; set; }
        [CssClass("align-right"), Width(80)]
        public Int16 ReorderLevel { get; set; }
    }
}