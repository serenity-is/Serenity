
namespace BasicApplication.Northwind.Forms
{
    using Serenity.ComponentModel;
    using System;
    using System.ComponentModel;

    [ColumnsScript("Northwind.Category")]
    [BasedOnRow(typeof(Entities.CategoryRow))]
    public class CategoryColumns
    {
        [EditLink, CssClass("align-right"), DisplayName("Db.Shared.RecordId")]
        public Int32 CategoryID { get; set; }
        [EditLink, Width(250)]
        public String CategoryName { get; set; }
        [Width(450)]
        public String Description { get; set; }
    }
}