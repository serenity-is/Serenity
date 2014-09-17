
namespace BasicApplication.Northwind
{
    using Serenity;
    using Serenity.ComponentModel;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using BasicApplication;

    public partial class RegionForm : PrefixedContext
    {
        public RegionForm(string idPrefix) : base(idPrefix) {}
    
        public IntegerEditor RegionID { get { return ById<IntegerEditor>("RegionID"); } }
        public StringEditor RegionDescription { get { return ById<StringEditor>("RegionDescription"); } }
    }
}

