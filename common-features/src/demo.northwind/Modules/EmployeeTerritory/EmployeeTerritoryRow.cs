namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("EmployeeTerritories")]
[DisplayName("EmployeeTerritories"), InstanceName("EmployeeTerritories")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class EmployeeTerritoryRow : Row<EmployeeTerritoryRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Employee Id"), PrimaryKey, ForeignKey(typeof(EmployeeRow)), LeftJoin("jEmployee"), IdProperty]
    public int? EmployeeID { get => fields.EmployeeID[this]; set => fields.EmployeeID[this] = value; }

    [DisplayName("Territory Id"), Size(20), PrimaryKey, ForeignKey(typeof(TerritoryRow)), LeftJoin("jTerritory"), QuickSearch, NameProperty]
    public string TerritoryID { get => fields.TerritoryID[this]; set => fields.TerritoryID[this] = value; }

    [DisplayName("Employee Full Name"), Origin("jEmployee", nameof(EmployeeRow.FullName))]
    public string EmployeeFullName { get => fields.EmployeeFullName[this]; set => fields.EmployeeFullName[this] = value; }

    [DisplayName("Territory Description"), Expression("jTerritory.[TerritoryDescription]")]
    public string TerritoryDescription { get => fields.TerritoryDescription[this]; set => fields.TerritoryDescription[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field EmployeeID;
        public StringField TerritoryID;

        public StringField EmployeeFullName;

        public StringField TerritoryDescription;
    }
}