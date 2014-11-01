
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class ProductForm : PrefixedContext
    {
        public ProductForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor ProductName { get { return ById<StringEditor>("ProductName"); } }
        public BooleanEditor Discontinued { get { return ById<BooleanEditor>("Discontinued"); } }
        public LookupEditor SupplierID { get { return ById<LookupEditor>("SupplierID"); } }
        public LookupEditor CategoryID { get { return ById<LookupEditor>("CategoryID"); } }
        public StringEditor QuantityPerUnit { get { return ById<StringEditor>("QuantityPerUnit"); } }
        public DecimalEditor UnitPrice { get { return ById<DecimalEditor>("UnitPrice"); } }
        public StringEditor UnitsInStock { get { return ById<StringEditor>("UnitsInStock"); } }
        public StringEditor UnitsOnOrder { get { return ById<StringEditor>("UnitsOnOrder"); } }
        public StringEditor ReorderLevel { get { return ById<StringEditor>("ReorderLevel"); } }
    }
}

