
namespace BasicApplication.Northwind.Forms
{
    using System;
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Northwind.Category")]
    [BasedOnRow(typeof(Entities.CategoryRow))]
    public class CategoryForm
    {
        public String CategoryName { get; set; }
        public String Description { get; set; }
        public Stream Picture { get; set; }
    }
}