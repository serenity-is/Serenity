namespace Serenity.Demo.Northwind;

[NorthwindConnection, NorthwindModule, TableName("EmployeeTerritories")]
[DisplayText("EmployeeTerritories"), InstanceName("EmployeeTerritories")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
public sealed class EmployeeTerritoryRow : Row<EmployeeTerritoryRow.RowFields>, IIdRow, INameRow
{
    [DisplayText("Employee Id"), PrimaryKey, ForeignKey(typeof(EmployeeRow)), LeftJoin("jEmployee"), IdProperty]
    public int? EmployeeID { get => fields.EmployeeID[this]; set => fields.EmployeeID[this] = value; }

    [DisplayText("Territory Id"), Size(20), PrimaryKey, ForeignKey(typeof(TerritoryRow)), LeftJoin("jTerritory"), QuickSearch, NameProperty]
    public string? TerritoryID { get => fields.TerritoryID[this]; set => fields.TerritoryID[this] = value; }

    [DisplayText("Employee Full Name"), Origin("jEmployee", nameof(EmployeeRow.FullName))]
    public string? EmployeeFullName { get => fields.EmployeeFullName[this]; set => fields.EmployeeFullName[this] = value; }

    [DisplayText("Territory Description"), Expression("jTerritory.[TerritoryDescription]")]
    public string? TerritoryDescription { get => fields.TerritoryDescription[this]; set => fields.TerritoryDescription[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field EmployeeID = null!;
        public StringField TerritoryID = null!;

        public StringField EmployeeFullName = null!;

        public StringField TerritoryDescription = null!;
    }
}