namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Territories")]
[DisplayName("Territories"), InstanceName("Territory")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class TerritoryRow : Row<TerritoryRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("ID"), Identity, IdProperty]
    public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

    [DisplayName("Territory ID"), Size(20), PrimaryKey, NotNull, QuickSearch, Updatable(false), NameProperty]
    public string TerritoryID { get => fields.TerritoryID[this]; set => fields.TerritoryID[this] = value; }

    [DisplayName("Territory Description"), Size(50), NotNull, QuickSearch, LookupInclude]
    public string TerritoryDescription { get => fields.TerritoryDescription[this]; set => fields.TerritoryDescription[this] = value; }

    [DisplayName("Region"), NotNull, ForeignKey(typeof(RegionRow)), LeftJoin("jRegion")]
    public int? RegionID { get => fields.RegionID[this]; set => fields.RegionID[this] = value; }

    [Origin("jRegion"), DisplayName("Region"), QuickSearch, LookupInclude]
    public string RegionDescription { get => fields.RegionDescription[this]; set => fields.RegionDescription[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field ID;
        public StringField TerritoryID;
        public StringField TerritoryDescription;
        public Int32Field RegionID;

        public StringField RegionDescription;
    }
}