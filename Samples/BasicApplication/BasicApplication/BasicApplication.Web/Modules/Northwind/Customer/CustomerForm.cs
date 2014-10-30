
namespace BasicApplication.Northwind.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    [FormScript("Northwind.Customer")]
    [BasedOnRow(typeof(Entities.CustomerRow))]
    public class CustomerForm
    {
        [Category("General")]
        public String CustomerID { get; set; }
        public String CompanyName { get; set; }
        [Category("Contact")]
        public String ContactName { get; set; }
        public String ContactTitle { get; set; }
        [Category("Address")]
        public String Address { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public String Phone { get; set; }
        public String Fax { get; set; }
    }
}