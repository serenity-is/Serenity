
namespace BasicApplication.Northwind.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Northwind.CustomerDemographic")]
    [BasedOnRow(typeof(Entities.CustomerDemographicRow))]
    public class CustomerDemographicForm
    {
        public String CustomerTypeID { get; set; }
        public String CustomerDesc { get; set; }
    }
}