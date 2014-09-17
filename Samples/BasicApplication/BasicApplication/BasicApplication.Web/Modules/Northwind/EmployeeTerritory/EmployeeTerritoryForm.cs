
namespace BasicApplication.Northwind.Forms
{
    using Serenity;
    using Serenity.ComponentModel;
    using Serenity.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [FormScript("Northwind.EmployeeTerritory")]
    [BasedOnRow(typeof(Entities.EmployeeTerritoryRow))]
    public class EmployeeTerritoryForm
    {
        public String TerritoryID { get; set; }
    }
}