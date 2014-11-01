
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class OrderForm : PrefixedContext
    {
        public OrderForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor CustomerID { get { return ById<StringEditor>("CustomerID"); } }
        public IntegerEditor EmployeeID { get { return ById<IntegerEditor>("EmployeeID"); } }
        public DateEditor OrderDate { get { return ById<DateEditor>("OrderDate"); } }
        public DateEditor RequiredDate { get { return ById<DateEditor>("RequiredDate"); } }
        public DateEditor ShippedDate { get { return ById<DateEditor>("ShippedDate"); } }
        public IntegerEditor ShipVia { get { return ById<IntegerEditor>("ShipVia"); } }
        public DecimalEditor Freight { get { return ById<DecimalEditor>("Freight"); } }
        public StringEditor ShipName { get { return ById<StringEditor>("ShipName"); } }
        public StringEditor ShipAddress { get { return ById<StringEditor>("ShipAddress"); } }
        public StringEditor ShipCity { get { return ById<StringEditor>("ShipCity"); } }
        public StringEditor ShipRegion { get { return ById<StringEditor>("ShipRegion"); } }
        public StringEditor ShipPostalCode { get { return ById<StringEditor>("ShipPostalCode"); } }
        public StringEditor ShipCountry { get { return ById<StringEditor>("ShipCountry"); } }
    }
}

