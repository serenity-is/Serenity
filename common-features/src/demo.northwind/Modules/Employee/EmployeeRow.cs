using System.IO;

namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Employees")]
[DisplayName("Employees"), InstanceName("Employee")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class EmployeeRow : Row<EmployeeRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Employee Id"), Identity, IdProperty]
    public int? EmployeeID { get => fields.EmployeeID[this]; set => fields.EmployeeID[this] = value; }

    [DisplayName("Last Name"), Size(20), NotNull]
    public string LastName { get => fields.LastName[this]; set => fields.LastName[this] = value; }

    [DisplayName("First Name"), Size(10), NotNull]
    public string FirstName { get => fields.FirstName[this]; set => fields.FirstName[this] = value; }

    [DisplayName("FullName"), NameProperty, QuickSearch]
    [Concat($"T0.[{nameof(FirstName)}]", "' '", $"T0.[{nameof(LastName)}]")]
    public string FullName { get => fields.FullName[this]; set => fields.FullName[this] = value; }

    [DisplayName("Gender"), Case($"T0.[{nameof(TitleOfCourtesy)}] LIKE '%s%'", 2,
        $"T0.[{nameof(TitleOfCourtesy)}] LIKE '%Mr%'", 1)]
    public Gender? Gender { get => (Gender?)Fields.Gender[this]; set => fields.Gender[this] = (int?)value; } 

    [DisplayName("Title"), Size(30)]
    public string Title { get => fields.Title[this]; set => fields.Title[this] = value; }

    [DisplayName("Title Of Courtesy"), Size(25)]
    public string TitleOfCourtesy { get => fields.TitleOfCourtesy[this]; set => fields.TitleOfCourtesy[this] = value; }

    [DisplayName("Birth Date")]
    public DateTime? BirthDate { get => fields.BirthDate[this]; set => fields.BirthDate[this] = value; }

    [DisplayName("Hire Date")]
    public DateTime? HireDate { get => fields.HireDate[this]; set => fields.HireDate[this] = value; }

    [DisplayName("Address"), Size(60)]
    public string Address { get => fields.Address[this]; set => fields.Address[this] = value; }

    [DisplayName("City"), Size(15)]
    public string City { get => fields.City[this]; set => fields.City[this] = value; }

    [DisplayName("Region"), Size(15)]
    public string Region { get => fields.Region[this]; set => fields.Region[this] = value; }

    [DisplayName("Postal Code"), Size(10)]
    public string PostalCode { get => fields.PostalCode[this]; set => fields.PostalCode[this] = value; }

    [DisplayName("Country"), Size(15)]
    public string Country { get => fields.Country[this]; set => fields.Country[this] = value; }

    [DisplayName("Home Phone"), Size(24)]
    public string HomePhone { get => fields.HomePhone[this]; set => fields.HomePhone[this] = value; }

    [DisplayName("Extension"), Size(4)]
    public string Extension { get => fields.Extension[this]; set => fields.Extension[this] = value; }

    [DisplayName("Photo")]
    public Stream Photo { get => fields.Photo[this]; set => fields.Photo[this] = value; }

    [DisplayName("Notes")]
    public string Notes { get => fields.Notes[this]; set => fields.Notes[this] = value; }

    [DisplayName("Reports To"), ForeignKey(typeof(EmployeeRow)), LeftJoin("jReportsTo")]
    public int? ReportsTo { get => fields.ReportsTo[this]; set => fields.ReportsTo[this] = value; }

    [DisplayName("Photo Path"), Size(255)]
    public string PhotoPath { get => fields.PhotoPath[this]; set => fields.PhotoPath[this] = value; }

    [Origin("jReportsTo", nameof(FullName))]
    public string ReportsToFullName { get => fields.ReportsToFullName[this]; set => fields.ReportsToFullName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field EmployeeID;
        public StringField LastName;
        public StringField FirstName;
        public StringField FullName;
        public StringField Title;
        public StringField TitleOfCourtesy;
        public DateTimeField BirthDate;
        public DateTimeField HireDate;
        public StringField Address;
        public StringField City;
        public StringField Region;
        public StringField PostalCode;
        public StringField Country;
        public StringField HomePhone;
        public StringField Extension;
        public StreamField Photo;
        public StringField Notes;
        public Int32Field ReportsTo;
        public StringField PhotoPath;

        public StringField ReportsToFullName;

        public Int32Field Gender;
    }
}