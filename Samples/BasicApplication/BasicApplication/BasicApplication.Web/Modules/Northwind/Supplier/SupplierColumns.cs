
namespace BasicApplication.Northwind.Forms
{
    using Serenity.ComponentModel;
    using System;
    using System.ComponentModel;

    [ColumnsScript("Northwind.Supplier")]
    [BasedOnRow(typeof(Entities.SupplierRow))]
    public class SupplierColumns
    {
        [EditLink, DisplayName("Db.Shared.RecordId"), AlignRight]
        public Int32 SupplierID { get; set; }
        [EditLink, Width(250)]
        public String CompanyName { get; set; }
        [Width(150)]
        public String ContactName { get; set; }
        [Width(150)]
        public String ContactTitle { get; set; }
        [Width(120)]
        public String Phone { get; set; }
        [Width(130)]
        public String City { get; set; }
        [Width(80)]
        public String Region { get; set; }
        [Width(130)]
        public String Country { get; set; }
    }
}