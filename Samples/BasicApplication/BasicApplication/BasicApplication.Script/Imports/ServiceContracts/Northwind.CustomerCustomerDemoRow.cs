
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
    }
    
}

