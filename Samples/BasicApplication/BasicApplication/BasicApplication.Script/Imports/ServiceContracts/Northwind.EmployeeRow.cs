
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
        public String FullName { get; set; }
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
        public String ReportsToFullName { get; set; }
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
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string EmployeeID = "EmployeeID";
            [InlineConstant] public const string LastName = "LastName";
            [InlineConstant] public const string FirstName = "FirstName";
            [InlineConstant] public const string FullName = "FullName";
            [InlineConstant] public const string Title = "Title";
            [InlineConstant] public const string TitleOfCourtesy = "TitleOfCourtesy";
            [InlineConstant] public const string BirthDate = "BirthDate";
            [InlineConstant] public const string HireDate = "HireDate";
            [InlineConstant] public const string Address = "Address";
            [InlineConstant] public const string City = "City";
            [InlineConstant] public const string Region = "Region";
            [InlineConstant] public const string PostalCode = "PostalCode";
            [InlineConstant] public const string Country = "Country";
            [InlineConstant] public const string HomePhone = "HomePhone";
            [InlineConstant] public const string Extension = "Extension";
            [InlineConstant] public const string Photo = "Photo";
            [InlineConstant] public const string Notes = "Notes";
            [InlineConstant] public const string ReportsTo = "ReportsTo";
            [InlineConstant] public const string PhotoPath = "PhotoPath";
            [InlineConstant] public const string ReportsToFullName = "ReportsToFullName";
            [InlineConstant] public const string ReportsToLastName = "ReportsToLastName";
            [InlineConstant] public const string ReportsToFirstName = "ReportsToFirstName";
            [InlineConstant] public const string ReportsToTitle = "ReportsToTitle";
            [InlineConstant] public const string ReportsToTitleOfCourtesy = "ReportsToTitleOfCourtesy";
            [InlineConstant] public const string ReportsToBirthDate = "ReportsToBirthDate";
            [InlineConstant] public const string ReportsToHireDate = "ReportsToHireDate";
            [InlineConstant] public const string ReportsToAddress = "ReportsToAddress";
            [InlineConstant] public const string ReportsToCity = "ReportsToCity";
            [InlineConstant] public const string ReportsToRegion = "ReportsToRegion";
            [InlineConstant] public const string ReportsToPostalCode = "ReportsToPostalCode";
            [InlineConstant] public const string ReportsToCountry = "ReportsToCountry";
            [InlineConstant] public const string ReportsToHomePhone = "ReportsToHomePhone";
            [InlineConstant] public const string ReportsToExtension = "ReportsToExtension";
            [InlineConstant] public const string ReportsToPhoto = "ReportsToPhoto";
            [InlineConstant] public const string ReportsToNotes = "ReportsToNotes";
            [InlineConstant] public const string ReportsToReportsTo = "ReportsToReportsTo";
            [InlineConstant] public const string ReportsToPhotoPath = "ReportsToPhotoPath";
        }
    }
    
}

