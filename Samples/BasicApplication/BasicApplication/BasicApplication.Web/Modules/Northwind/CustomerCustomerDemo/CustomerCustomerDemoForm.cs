
namespace BasicApplication.Northwind.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Northwind.CustomerCustomerDemo")]
    [BasedOnRow(typeof(Entities.CustomerCustomerDemoRow))]
    public class CustomerCustomerDemoForm
    {
        public String CustomerID { get; set; }
        public String CustomerTypeID { get; set; }
    }
}