
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [Imported, Serializable, PreserveMemberCase]
    public partial class ProductRow
    {
        public Int32? ProductID { get; set; }
        public String ProductName { get; set; }
        public Boolean? Discontinued { get; set; }
        public Int32? SupplierID { get; set; }
        public Int32? CategoryID { get; set; }
        public String QuantityPerUnit { get; set; }
        public Decimal? UnitPrice { get; set; }
        public Int16? UnitsInStock { get; set; }
        public Int16? UnitsOnOrder { get; set; }
        public Int16? ReorderLevel { get; set; }
        public String SupplierCompanyName { get; set; }
        public String SupplierContactName { get; set; }
        public String SupplierContactTitle { get; set; }
        public String SupplierAddress { get; set; }
        public String SupplierCity { get; set; }
        public String SupplierRegion { get; set; }
        public String SupplierPostalCode { get; set; }
        public String SupplierCountry { get; set; }
        public String SupplierPhone { get; set; }
        public String SupplierFax { get; set; }
        public String SupplierHomePage { get; set; }
        public String CategoryName { get; set; }
        public String CategoryDescription { get; set; }
        public byte[] CategoryPicture { get; set; }
    }
    
}

