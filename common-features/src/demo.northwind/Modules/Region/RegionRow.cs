namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Region")]
[DisplayName("Regions"), InstanceName("Region")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
public sealed class RegionRow : Row<RegionRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Region Id"), PrimaryKey, NotNull, Updatable(false), QuickSearch, IdProperty]
    public int? RegionID { get => fields.RegionID[this]; set => fields.RegionID[this] = value; }

    [DisplayName("Region Description"), Size(50), NotNull, QuickSearch, NameProperty]
    public string RegionDescription { get => fields.RegionDescription[this]; set => fields.RegionDescription[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field RegionID;
        public StringField RegionDescription;
    }
}