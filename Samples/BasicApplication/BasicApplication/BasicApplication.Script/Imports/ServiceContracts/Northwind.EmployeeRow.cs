
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
    public partial class EmployeeRow
    {
        public Int32? EmployeeID { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String Title { get; set; }
        public String TitleOfCourtesy { get; set; }
        public String BirthDate { get; set; }
        public String HireDate { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String PostalCode { get; set; }
        public String Country { get; set; }
        public String HomePhone { get; set; }
        public String Extension { get; set; }
        public byte[] Photo { get; set; }
        public String Notes { get; set; }
        public Int32? ReportsTo { get; set; }
        public String PhotoPath { get; set; }
        public String ReportsToLastName { get; set; }
        public String ReportsToFirstName { get; set; }
        public String ReportsToTitle { get; set; }
        public String ReportsToTitleOfCourtesy { get; set; }
        public String ReportsToBirthDate { get; set; }
        public String ReportsToHireDate { get; set; }
        public String ReportsToAddress { get; set; }
        public String ReportsToCity { get; set; }
        public String ReportsToRegion { get; set; }
        public String ReportsToPostalCode { get; set; }
        public String ReportsToCountry { get; set; }
        public String ReportsToHomePhone { get; set; }
        public String ReportsToExtension { get; set; }
        public byte[] ReportsToPhoto { get; set; }
        public String ReportsToNotes { get; set; }
        public Int32? ReportsToReportsTo { get; set; }
        public String ReportsToPhotoPath { get; set; }
    }
    
}

