
namespace BasicApplication.Northwind.Entities
{
    using Newtonsoft.Json;
    using Serenity;
    using Serenity.Data;
    using Serenity.Data.Mapping;
    using System;
    using System.IO;
    using System.ComponentModel;

    [ConnectionKey("Default"), DisplayName("Employees"), InstanceName("Employees")]
    [ReadPermission("Northwind")]
    [ModifyPermission("Northwind")]
    [JsonConverter(typeof(JsonRowConverter))]
    public sealed class EmployeeRow : Row, IIdRow, INameRow
    {
        [DisplayName("Employee Id"), Identity]
        public Int32? EmployeeID
        {
            get { return Fields.EmployeeID[this]; }
            set { Fields.EmployeeID[this] = value; }
        }

        [DisplayName("Last Name"), Size(20), NotNull, QuickSearch]
        public String LastName
        {
            get { return Fields.LastName[this]; }
            set { Fields.LastName[this] = value; }
        }

        [DisplayName("First Name"), Size(10), NotNull]
        public String FirstName
        {
            get { return Fields.FirstName[this]; }
            set { Fields.FirstName[this] = value; }
        }

        [DisplayName("Title"), Size(30)]
        public String Title
        {
            get { return Fields.Title[this]; }
            set { Fields.Title[this] = value; }
        }

        [DisplayName("Title Of Courtesy"), Size(25)]
        public String TitleOfCourtesy
        {
            get { return Fields.TitleOfCourtesy[this]; }
            set { Fields.TitleOfCourtesy[this] = value; }
        }

        [DisplayName("Birth Date")]
        public DateTime? BirthDate
        {
            get { return Fields.BirthDate[this]; }
            set { Fields.BirthDate[this] = value; }
        }

        [DisplayName("Hire Date")]
        public DateTime? HireDate
        {
            get { return Fields.HireDate[this]; }
            set { Fields.HireDate[this] = value; }
        }

        [DisplayName("Address"), Size(60)]
        public String Address
        {
            get { return Fields.Address[this]; }
            set { Fields.Address[this] = value; }
        }

        [DisplayName("City"), Size(15)]
        public String City
        {
            get { return Fields.City[this]; }
            set { Fields.City[this] = value; }
        }

        [DisplayName("Region"), Size(15)]
        public String Region
        {
            get { return Fields.Region[this]; }
            set { Fields.Region[this] = value; }
        }

        [DisplayName("Postal Code"), Size(10)]
        public String PostalCode
        {
            get { return Fields.PostalCode[this]; }
            set { Fields.PostalCode[this] = value; }
        }

        [DisplayName("Country"), Size(15)]
        public String Country
        {
            get { return Fields.Country[this]; }
            set { Fields.Country[this] = value; }
        }

        [DisplayName("Home Phone"), Size(24)]
        public String HomePhone
        {
            get { return Fields.HomePhone[this]; }
            set { Fields.HomePhone[this] = value; }
        }

        [DisplayName("Extension"), Size(4)]
        public String Extension
        {
            get { return Fields.Extension[this]; }
            set { Fields.Extension[this] = value; }
        }

        [DisplayName("Photo")]
        public Stream Photo
        {
            get { return Fields.Photo[this]; }
            set { Fields.Photo[this] = value; }
        }

        [DisplayName("Notes")]
        public String Notes
        {
            get { return Fields.Notes[this]; }
            set { Fields.Notes[this] = value; }
        }

        [DisplayName("Reports To"), ForeignKey("Employees", "EmployeeID"), LeftJoin("jReportsTo")]
        public Int32? ReportsTo
        {
            get { return Fields.ReportsTo[this]; }
            set { Fields.ReportsTo[this] = value; }
        }

        [DisplayName("Photo Path"), Size(255)]
        public String PhotoPath
        {
            get { return Fields.PhotoPath[this]; }
            set { Fields.PhotoPath[this] = value; }
        }

        [DisplayName("Reports To Last Name"), Expression("jReportsTo.LastName")]
        public String ReportsToLastName
        {
            get { return Fields.ReportsToLastName[this]; }
            set { Fields.ReportsToLastName[this] = value; }
        }

        [DisplayName("Reports To First Name"), Expression("jReportsTo.FirstName")]
        public String ReportsToFirstName
        {
            get { return Fields.ReportsToFirstName[this]; }
            set { Fields.ReportsToFirstName[this] = value; }
        }

        [DisplayName("Reports To Title"), Expression("jReportsTo.Title")]
        public String ReportsToTitle
        {
            get { return Fields.ReportsToTitle[this]; }
            set { Fields.ReportsToTitle[this] = value; }
        }

        [DisplayName("Reports To Title Of Courtesy"), Expression("jReportsTo.TitleOfCourtesy")]
        public String ReportsToTitleOfCourtesy
        {
            get { return Fields.ReportsToTitleOfCourtesy[this]; }
            set { Fields.ReportsToTitleOfCourtesy[this] = value; }
        }

        [DisplayName("Reports To Birth Date"), Expression("jReportsTo.BirthDate")]
        public DateTime? ReportsToBirthDate
        {
            get { return Fields.ReportsToBirthDate[this]; }
            set { Fields.ReportsToBirthDate[this] = value; }
        }

        [DisplayName("Reports To Hire Date"), Expression("jReportsTo.HireDate")]
        public DateTime? ReportsToHireDate
        {
            get { return Fields.ReportsToHireDate[this]; }
            set { Fields.ReportsToHireDate[this] = value; }
        }

        [DisplayName("Reports To Address"), Expression("jReportsTo.Address")]
        public String ReportsToAddress
        {
            get { return Fields.ReportsToAddress[this]; }
            set { Fields.ReportsToAddress[this] = value; }
        }

        [DisplayName("Reports To City"), Expression("jReportsTo.City")]
        public String ReportsToCity
        {
            get { return Fields.ReportsToCity[this]; }
            set { Fields.ReportsToCity[this] = value; }
        }

        [DisplayName("Reports To Region"), Expression("jReportsTo.Region")]
        public String ReportsToRegion
        {
            get { return Fields.ReportsToRegion[this]; }
            set { Fields.ReportsToRegion[this] = value; }
        }

        [DisplayName("Reports To Postal Code"), Expression("jReportsTo.PostalCode")]
        public String ReportsToPostalCode
        {
            get { return Fields.ReportsToPostalCode[this]; }
            set { Fields.ReportsToPostalCode[this] = value; }
        }

        [DisplayName("Reports To Country"), Expression("jReportsTo.Country")]
        public String ReportsToCountry
        {
            get { return Fields.ReportsToCountry[this]; }
            set { Fields.ReportsToCountry[this] = value; }
        }

        [DisplayName("Reports To Home Phone"), Expression("jReportsTo.HomePhone")]
        public String ReportsToHomePhone
        {
            get { return Fields.ReportsToHomePhone[this]; }
            set { Fields.ReportsToHomePhone[this] = value; }
        }

        [DisplayName("Reports To Extension"), Expression("jReportsTo.Extension")]
        public String ReportsToExtension
        {
            get { return Fields.ReportsToExtension[this]; }
            set { Fields.ReportsToExtension[this] = value; }
        }

        [DisplayName("Reports To Photo"), Expression("jReportsTo.Photo")]
        public Stream ReportsToPhoto
        {
            get { return Fields.ReportsToPhoto[this]; }
            set { Fields.ReportsToPhoto[this] = value; }
        }

        [DisplayName("Reports To Notes"), Expression("jReportsTo.Notes")]
        public String ReportsToNotes
        {
            get { return Fields.ReportsToNotes[this]; }
            set { Fields.ReportsToNotes[this] = value; }
        }

        [DisplayName("Reports To"), Expression("jReportsTo.ReportsTo")]
        public Int32? ReportsToReportsTo
        {
            get { return Fields.ReportsToReportsTo[this]; }
            set { Fields.ReportsToReportsTo[this] = value; }
        }

        [DisplayName("Reports To Photo Path"), Expression("jReportsTo.PhotoPath")]
        public String ReportsToPhotoPath
        {
            get { return Fields.ReportsToPhotoPath[this]; }
            set { Fields.ReportsToPhotoPath[this] = value; }
        }

        IIdField IIdRow.IdField
        {
            get { return Fields.EmployeeID; }
        }

        StringField INameRow.NameField
        {
            get { return Fields.LastName; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public EmployeeRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public readonly Int32Field EmployeeID;
            public readonly StringField LastName;
            public readonly StringField FirstName;
            public readonly StringField Title;
            public readonly StringField TitleOfCourtesy;
            public readonly DateTimeField BirthDate;
            public readonly DateTimeField HireDate;
            public readonly StringField Address;
            public readonly StringField City;
            public readonly StringField Region;
            public readonly StringField PostalCode;
            public readonly StringField Country;
            public readonly StringField HomePhone;
            public readonly StringField Extension;
            public readonly StreamField Photo;
            public readonly StringField Notes;
            public readonly Int32Field ReportsTo;
            public readonly StringField PhotoPath;

            public readonly StringField ReportsToLastName;
            public readonly StringField ReportsToFirstName;
            public readonly StringField ReportsToTitle;
            public readonly StringField ReportsToTitleOfCourtesy;
            public readonly DateTimeField ReportsToBirthDate;
            public readonly DateTimeField ReportsToHireDate;
            public readonly StringField ReportsToAddress;
            public readonly StringField ReportsToCity;
            public readonly StringField ReportsToRegion;
            public readonly StringField ReportsToPostalCode;
            public readonly StringField ReportsToCountry;
            public readonly StringField ReportsToHomePhone;
            public readonly StringField ReportsToExtension;
            public readonly StreamField ReportsToPhoto;
            public readonly StringField ReportsToNotes;
            public readonly Int32Field ReportsToReportsTo;
            public readonly StringField ReportsToPhotoPath;

            public RowFields()
                : base("Employees")
            {
                LocalTextPrefix = "Northwind.Employee";
            }
        }
    }
}