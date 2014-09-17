
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
    public partial class EmployeeTerritoryRow
    {
        public Int32? EmployeeID { get; set; }
        public String TerritoryID { get; set; }
        public String EmployeeLastName { get; set; }
        public String EmployeeFirstName { get; set; }
        public String EmployeeTitle { get; set; }
        public String EmployeeTitleOfCourtesy { get; set; }
        public String EmployeeBirthDate { get; set; }
        public String EmployeeHireDate { get; set; }
        public String EmployeeAddress { get; set; }
        public String EmployeeCity { get; set; }
        public String EmployeeRegion { get; set; }
        public String EmployeePostalCode { get; set; }
        public String EmployeeCountry { get; set; }
        public String EmployeeHomePhone { get; set; }
        public String EmployeeExtension { get; set; }
        public byte[] EmployeePhoto { get; set; }
        public String EmployeeNotes { get; set; }
        public Int32? EmployeeReportsTo { get; set; }
        public String EmployeePhotoPath { get; set; }
        public String TerritoryTerritoryDescription { get; set; }
        public Int32? TerritoryRegionID { get; set; }
    }
    
}

