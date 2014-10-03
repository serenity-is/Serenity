
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
    
        [Imported, PreserveMemberCase]
        public static class Fields
        {
            [InlineConstant] public const string EmployeeID = "EmployeeID";
            [InlineConstant] public const string TerritoryID = "TerritoryID";
            [InlineConstant] public const string EmployeeLastName = "EmployeeLastName";
            [InlineConstant] public const string EmployeeFirstName = "EmployeeFirstName";
            [InlineConstant] public const string EmployeeTitle = "EmployeeTitle";
            [InlineConstant] public const string EmployeeTitleOfCourtesy = "EmployeeTitleOfCourtesy";
            [InlineConstant] public const string EmployeeBirthDate = "EmployeeBirthDate";
            [InlineConstant] public const string EmployeeHireDate = "EmployeeHireDate";
            [InlineConstant] public const string EmployeeAddress = "EmployeeAddress";
            [InlineConstant] public const string EmployeeCity = "EmployeeCity";
            [InlineConstant] public const string EmployeeRegion = "EmployeeRegion";
            [InlineConstant] public const string EmployeePostalCode = "EmployeePostalCode";
            [InlineConstant] public const string EmployeeCountry = "EmployeeCountry";
            [InlineConstant] public const string EmployeeHomePhone = "EmployeeHomePhone";
            [InlineConstant] public const string EmployeeExtension = "EmployeeExtension";
            [InlineConstant] public const string EmployeePhoto = "EmployeePhoto";
            [InlineConstant] public const string EmployeeNotes = "EmployeeNotes";
            [InlineConstant] public const string EmployeeReportsTo = "EmployeeReportsTo";
            [InlineConstant] public const string EmployeePhotoPath = "EmployeePhotoPath";
            [InlineConstant] public const string TerritoryTerritoryDescription = "TerritoryTerritoryDescription";
            [InlineConstant] public const string TerritoryRegionID = "TerritoryRegionID";
        }
    }
    
}

