
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public partial class ShipperForm : PrefixedContext
    {
        public ShipperForm(string idPrefix) : base(idPrefix) {}
    
        public StringEditor CompanyName { get { return ById<StringEditor>("CompanyName"); } }
    }
}

