using System.IO;

namespace Serenity.Demo.Northwind;

[ConnectionKey("Northwind"), Module("Northwind"), TableName("Categories")]
[DisplayName("Categories"), InstanceName("Category")]
[ReadPermission(PermissionKeys.General)]
[ModifyPermission(PermissionKeys.General)]
[LookupScript]
[LocalizationRow(typeof(CategoryLangRow))]
public sealed class CategoryRow : Row<CategoryRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Category Id"), Identity, IdProperty]
    public int? CategoryID { get => fields.CategoryID[this]; set => fields.CategoryID[this] = value; }

    [DisplayName("Category Name"), Size(15), NotNull, QuickSearch, NameProperty, Localizable(true)]
    public string CategoryName { get => fields.CategoryName[this]; set => fields.CategoryName[this] = value; }

    [DisplayName("Description"), QuickSearch, Localizable(true)]
    public string Description { get => fields.Description[this]; set => fields.Description[this] = value; }

    [DisplayName("Picture")]
    public Stream Picture { get => fields.Picture[this]; set => fields.Picture[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public Int32Field CategoryID;
        public StringField CategoryName;
        public StringField Description;
        public StreamField Picture;
    }
}