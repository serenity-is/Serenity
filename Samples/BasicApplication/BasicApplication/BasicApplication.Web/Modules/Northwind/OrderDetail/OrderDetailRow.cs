
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.IO;
    using System.ComponentModel;

    [ConnectionKey("Default"), DisplayName("Order Details"), InstanceName("Order Details")]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class OrderDetailRow : Row, IIdRow
    {
        [DisplayName("Order Id"), PrimaryKey, ForeignKey("Orders", "OrderID"), LeftJoin("jOrder")]
        public Int32? OrderID
        {
            get { return Fields.OrderID[this]; }
            set { Fields.OrderID[this] = value; }
        }

        [DisplayName("Product Id"), PrimaryKey, ForeignKey("Products", "ProductID"), LeftJoin("jProduct")]
        public Int32? ProductID
        {
            get { return Fields.ProductID[this]; }
            set { Fields.ProductID[this] = value; }
        }

        [DisplayName("Unit Price"), Scale(4), NotNull]
        public Decimal? UnitPrice
        {
            get { return Fields.UnitPrice[this]; }
            set { Fields.UnitPrice[this] = value; }
        }

        [DisplayName("Quantity"), NotNull]
        public Int16? Quantity
        {
            get { return Fields.Quantity[this]; }
            set { Fields.Quantity[this] = value; }
        }

        [DisplayName("Discount"), NotNull]
        public Double? Discount
        {
            get { return Fields.Discount[this]; }
            set { Fields.Discount[this] = value; }
        }

        [DisplayName("Order Customer Id"), Expression("jOrder.CustomerID")]
        public String OrderCustomerID
        {
            get { return Fields.OrderCustomerID[this]; }
            set { Fields.OrderCustomerID[this] = value; }
        }

        [DisplayName("Order Employee Id"), Expression("jOrder.EmployeeID")]
        public Int32? OrderEmployeeID
        {
            get { return Fields.OrderEmployeeID[this]; }
            set { Fields.OrderEmployeeID[this] = value; }
        }

        [DisplayName("Order Order Date"), Expression("jOrder.OrderDate")]
        public DateTime? OrderOrderDate
        {
            get { return Fields.OrderOrderDate[this]; }
            set { Fields.OrderOrderDate[this] = value; }
        }

        [DisplayName("Order Required Date"), Expression("jOrder.RequiredDate")]
        public DateTime? OrderRequiredDate
        {
            get { return Fields.OrderRequiredDate[this]; }
            set { Fields.OrderRequiredDate[this] = value; }
        }

        [DisplayName("Order Shipped Date"), Expression("jOrder.ShippedDate")]
        public DateTime? OrderShippedDate
        {
            get { return Fields.OrderShippedDate[this]; }
            set { Fields.OrderShippedDate[this] = value; }
        }

        [DisplayName("Order Ship Via"), Expression("jOrder.ShipVia")]
        public Int32? OrderShipVia
        {
            get { return Fields.OrderShipVia[this]; }
            set { Fields.OrderShipVia[this] = value; }
        }

        [DisplayName("Order Freight"), Expression("jOrder.Freight")]
        public Decimal? OrderFreight
        {
            get { return Fields.OrderFreight[this]; }
            set { Fields.OrderFreight[this] = value; }
        }

        [DisplayName("Order Ship Name"), Expression("jOrder.ShipName")]
        public String OrderShipName
        {
            get { return Fields.OrderShipName[this]; }
            set { Fields.OrderShipName[this] = value; }
        }

        [DisplayName("Order Ship Address"), Expression("jOrder.ShipAddress")]
        public String OrderShipAddress
        {
            get { return Fields.OrderShipAddress[this]; }
            set { Fields.OrderShipAddress[this] = value; }
        }

        [DisplayName("Order Ship City"), Expression("jOrder.ShipCity")]
        public String OrderShipCity
        {
            get { return Fields.OrderShipCity[this]; }
            set { Fields.OrderShipCity[this] = value; }
        }

        [DisplayName("Order Ship Region"), Expression("jOrder.ShipRegion")]
        public String OrderShipRegion
        {
            get { return Fields.OrderShipRegion[this]; }
            set { Fields.OrderShipRegion[this] = value; }
        }

        [DisplayName("Order Ship Postal Code"), Expression("jOrder.ShipPostalCode")]
        public String OrderShipPostalCode
        {
            get { return Fields.OrderShipPostalCode[this]; }
            set { Fields.OrderShipPostalCode[this] = value; }
        }

        [DisplayName("Order Ship Country"), Expression("jOrder.ShipCountry")]
        public String OrderShipCountry
        {
            get { return Fields.OrderShipCountry[this]; }
            set { Fields.OrderShipCountry[this] = value; }
        }

        [DisplayName("Product Product Name"), Expression("jProduct.ProductName")]
        public String ProductProductName
        {
            get { return Fields.ProductProductName[this]; }
            set { Fields.ProductProductName[this] = value; }
        }

        [DisplayName("Product Discontinued"), Expression("jProduct.Discontinued")]
        public Boolean? ProductDiscontinued
        {
            get { return Fields.ProductDiscontinued[this]; }
            set { Fields.ProductDiscontinued[this] = value; }
        }

        [DisplayName("Product Supplier Id"), Expression("jProduct.SupplierID")]
        public Int32? ProductSupplierID
        {
            get { return Fields.ProductSupplierID[this]; }
            set { Fields.ProductSupplierID[this] = value; }
        }

        [DisplayName("Product Category Id"), Expression("jProduct.CategoryID")]
        public Int32? ProductCategoryID
        {
            get { return Fields.ProductCategoryID[this]; }
            set { Fields.ProductCategoryID[this] = value; }
        }

        [DisplayName("Product Quantity Per Unit"), Expression("jProduct.QuantityPerUnit")]
        public String ProductQuantityPerUnit
        {
            get { return Fields.ProductQuantityPerUnit[this]; }
            set { Fields.ProductQuantityPerUnit[this] = value; }
        }

        [DisplayName("Product Unit Price"), Expression("jProduct.UnitPrice")]
        public Decimal? ProductUnitPrice
        {
            get { return Fields.ProductUnitPrice[this]; }
            set { Fields.ProductUnitPrice[this] = value; }
        }

        [DisplayName("Product Units In Stock"), Expression("jProduct.UnitsInStock")]
        public Int16? ProductUnitsInStock
        {
            get { return Fields.ProductUnitsInStock[this]; }
            set { Fields.ProductUnitsInStock[this] = value; }
        }

        [DisplayName("Product Units On Order"), Expression("jProduct.UnitsOnOrder")]
        public Int16? ProductUnitsOnOrder
        {
            get { return Fields.ProductUnitsOnOrder[this]; }
            set { Fields.ProductUnitsOnOrder[this] = value; }
        }

        [DisplayName("Product Reorder Level"), Expression("jProduct.ReorderLevel")]
        public Int16? ProductReorderLevel
        {
            get { return Fields.ProductReorderLevel[this]; }
            set { Fields.ProductReorderLevel[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.OrderID; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public OrderDetailRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field OrderID;
            public readonly Int32Field ProductID;
            public readonly DecimalField UnitPrice;
            public readonly Int16Field Quantity;
            public readonly DoubleField Discount;

            public readonly StringField OrderCustomerID;
            public readonly Int32Field OrderEmployeeID;
            public readonly DateTimeField OrderOrderDate;
            public readonly DateTimeField OrderRequiredDate;
            public readonly DateTimeField OrderShippedDate;
            public readonly Int32Field OrderShipVia;
            public readonly DecimalField OrderFreight;
            public readonly StringField OrderShipName;
            public readonly StringField OrderShipAddress;
            public readonly StringField OrderShipCity;
            public readonly StringField OrderShipRegion;
            public readonly StringField OrderShipPostalCode;
            public readonly StringField OrderShipCountry;


            public readonly StringField ProductProductName;
            public readonly BooleanField ProductDiscontinued;
            public readonly Int32Field ProductSupplierID;
            public readonly Int32Field ProductCategoryID;
            public readonly StringField ProductQuantityPerUnit;
            public readonly DecimalField ProductUnitPrice;
            public readonly Int16Field ProductUnitsInStock;
            public readonly Int16Field ProductUnitsOnOrder;
            public readonly Int16Field ProductReorderLevel;

            public RowFields()
                : base("[Order Details]")
            {
                LocalTextPrefix = "Northwind.OrderDetail";
            }
        }
    }
}