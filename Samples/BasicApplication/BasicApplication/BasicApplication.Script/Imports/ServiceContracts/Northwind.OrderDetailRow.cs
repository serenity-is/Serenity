
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
    public partial class OrderDetailRow
    {
        public Int32? OrderID { get; set; }
        public Int32? ProductID { get; set; }
        public Decimal? UnitPrice { get; set; }
        public Int16? Quantity { get; set; }
        public Double? Discount { get; set; }
        public String OrderCustomerID { get; set; }
        public Int32? OrderEmployeeID { get; set; }
        public String OrderOrderDate { get; set; }
        public String OrderRequiredDate { get; set; }
        public String OrderShippedDate { get; set; }
        public Int32? OrderShipVia { get; set; }
        public Decimal? OrderFreight { get; set; }
        public String OrderShipName { get; set; }
        public String OrderShipAddress { get; set; }
        public String OrderShipCity { get; set; }
        public String OrderShipRegion { get; set; }
        public String OrderShipPostalCode { get; set; }
        public String OrderShipCountry { get; set; }
        public String ProductProductName { get; set; }
        public Boolean? ProductDiscontinued { get; set; }
        public Int32? ProductSupplierID { get; set; }
        public Int32? ProductCategoryID { get; set; }
        public String ProductQuantityPerUnit { get; set; }
        public Decimal? ProductUnitPrice { get; set; }
        public Int16? ProductUnitsInStock { get; set; }
        public Int16? ProductUnitsOnOrder { get; set; }
        public Int16? ProductReorderLevel { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string OrderID = "OrderID";
            [InlineConstant] public const string ProductID = "ProductID";
            [InlineConstant] public const string UnitPrice = "UnitPrice";
            [InlineConstant] public const string Quantity = "Quantity";
            [InlineConstant] public const string Discount = "Discount";
            [InlineConstant] public const string OrderCustomerID = "OrderCustomerID";
            [InlineConstant] public const string OrderEmployeeID = "OrderEmployeeID";
            [InlineConstant] public const string OrderOrderDate = "OrderOrderDate";
            [InlineConstant] public const string OrderRequiredDate = "OrderRequiredDate";
            [InlineConstant] public const string OrderShippedDate = "OrderShippedDate";
            [InlineConstant] public const string OrderShipVia = "OrderShipVia";
            [InlineConstant] public const string OrderFreight = "OrderFreight";
            [InlineConstant] public const string OrderShipName = "OrderShipName";
            [InlineConstant] public const string OrderShipAddress = "OrderShipAddress";
            [InlineConstant] public const string OrderShipCity = "OrderShipCity";
            [InlineConstant] public const string OrderShipRegion = "OrderShipRegion";
            [InlineConstant] public const string OrderShipPostalCode = "OrderShipPostalCode";
            [InlineConstant] public const string OrderShipCountry = "OrderShipCountry";
            [InlineConstant] public const string ProductProductName = "ProductProductName";
            [InlineConstant] public const string ProductDiscontinued = "ProductDiscontinued";
            [InlineConstant] public const string ProductSupplierID = "ProductSupplierID";
            [InlineConstant] public const string ProductCategoryID = "ProductCategoryID";
            [InlineConstant] public const string ProductQuantityPerUnit = "ProductQuantityPerUnit";
            [InlineConstant] public const string ProductUnitPrice = "ProductUnitPrice";
            [InlineConstant] public const string ProductUnitsInStock = "ProductUnitsInStock";
            [InlineConstant] public const string ProductUnitsOnOrder = "ProductUnitsOnOrder";
            [InlineConstant] public const string ProductReorderLevel = "ProductReorderLevel";
        }
    }
    
}

