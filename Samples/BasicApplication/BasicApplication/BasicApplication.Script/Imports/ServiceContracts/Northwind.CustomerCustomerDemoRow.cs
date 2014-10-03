
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
    public partial class CustomerCustomerDemoRow
    {
        public Int32? ID { get; set; }
        public String CustomerID { get; set; }
        public String CustomerTypeID { get; set; }
        public String CustomerCompanyName { get; set; }
        public String CustomerContactName { get; set; }
        public String CustomerContactTitle { get; set; }
        public String CustomerAddress { get; set; }
        public String CustomerCity { get; set; }
        public String CustomerRegion { get; set; }
        public String CustomerPostalCode { get; set; }
        public String CustomerCountry { get; set; }
        public String CustomerPhone { get; set; }
        public String CustomerFax { get; set; }
        public String CustomerTypeCustomerDesc { get; set; }
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string ID = "ID";
            [InlineConstant] public const string CustomerID = "CustomerID";
            [InlineConstant] public const string CustomerTypeID = "CustomerTypeID";
            [InlineConstant] public const string CustomerCompanyName = "CustomerCompanyName";
            [InlineConstant] public const string CustomerContactName = "CustomerContactName";
            [InlineConstant] public const string CustomerContactTitle = "CustomerContactTitle";
            [InlineConstant] public const string CustomerAddress = "CustomerAddress";
            [InlineConstant] public const string CustomerCity = "CustomerCity";
            [InlineConstant] public const string CustomerRegion = "CustomerRegion";
            [InlineConstant] public const string CustomerPostalCode = "CustomerPostalCode";
            [InlineConstant] public const string CustomerCountry = "CustomerCountry";
            [InlineConstant] public const string CustomerPhone = "CustomerPhone";
            [InlineConstant] public const string CustomerFax = "CustomerFax";
            [InlineConstant] public const string CustomerTypeCustomerDesc = "CustomerTypeCustomerDesc";
        }
    }
    
}

