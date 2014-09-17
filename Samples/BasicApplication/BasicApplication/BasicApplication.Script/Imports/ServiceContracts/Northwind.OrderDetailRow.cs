
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
    }
    
}

