using System.IO;

namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Categories")]
[DisplayName("Categories"), InstanceName("Category")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
[LocalizationRow(typeof(CategoryLangRow), LocalizeListByDefault = true)]
public sealed class CategoryRow : Row<CategoryRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Category Id"), Identity, IdProperty]
    public int? CategoryID { get => fields.CategoryID[this]; set => fields.CategoryID[this] = value; }

    [DisplayName("Category Name"), Size(15), NotNull, QuickSearch, NameProperty]
    public string CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayName("Description"), QuickSearch]
    public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

    [DisplayName("Picture")]
    public string PicturePath { get => fields.PicturePath[this]; set => fields.PicturePath[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field CategoryID;
        public StringField CategoryName;
        public StringField Description;
        public StringField PicturePath;
    }
}