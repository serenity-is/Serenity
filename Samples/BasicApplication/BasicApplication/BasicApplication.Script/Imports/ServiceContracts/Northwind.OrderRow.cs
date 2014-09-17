
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
    public partial class OrderRow
    {
        public Int32? OrderID { get; set; }
        public String CustomerID { get; set; }
        public Int32? EmployeeID { get; set; }
        public String OrderDate { get; set; }
        public String RequiredDate { get; set; }
        public String ShippedDate { get; set; }
        public Int32? ShipVia { get; set; }
        public Decimal? Freight { get; set; }
        public String ShipName { get; set; }
        public String ShipAddress { get; set; }
        public String ShipCity { get; set; }
        public String ShipRegion { get; set; }
        public String ShipPostalCode { get; set; }
        public String ShipCountry { get; set; }
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
        public String ShipViaCompanyName { get; set; }
        public String ShipViaPhone { get; set; }
    }
    
}

