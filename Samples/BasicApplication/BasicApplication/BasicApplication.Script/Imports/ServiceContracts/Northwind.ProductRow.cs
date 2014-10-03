
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
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string ProductID = "ProductID";
            [InlineConstant] public const string ProductName = "ProductName";
            [InlineConstant] public const string Discontinued = "Discontinued";
            [InlineConstant] public const string SupplierID = "SupplierID";
            [InlineConstant] public const string CategoryID = "CategoryID";
            [InlineConstant] public const string QuantityPerUnit = "QuantityPerUnit";
            [InlineConstant] public const string UnitPrice = "UnitPrice";
            [InlineConstant] public const string UnitsInStock = "UnitsInStock";
            [InlineConstant] public const string UnitsOnOrder = "UnitsOnOrder";
            [InlineConstant] public const string ReorderLevel = "ReorderLevel";
            [InlineConstant] public const string SupplierCompanyName = "SupplierCompanyName";
            [InlineConstant] public const string SupplierContactName = "SupplierContactName";
            [InlineConstant] public const string SupplierContactTitle = "SupplierContactTitle";
            [InlineConstant] public const string SupplierAddress = "SupplierAddress";
            [InlineConstant] public const string SupplierCity = "SupplierCity";
            [InlineConstant] public const string SupplierRegion = "SupplierRegion";
            [InlineConstant] public const string SupplierPostalCode = "SupplierPostalCode";
            [InlineConstant] public const string SupplierCountry = "SupplierCountry";
            [InlineConstant] public const string SupplierPhone = "SupplierPhone";
            [InlineConstant] public const string SupplierFax = "SupplierFax";
            [InlineConstant] public const string SupplierHomePage = "SupplierHomePage";
            [InlineConstant] public const string CategoryName = "CategoryName";
            [InlineConstant] public const string CategoryDescription = "CategoryDescription";
            [InlineConstant] public const string CategoryPicture = "CategoryPicture";
        }
    }
    
}

