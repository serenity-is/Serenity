namespace Serene.Administration;

[ConnectionKey("Default"), Module("Administration"), TableName("Languages")]
[DisplayName("Languages"), InstanceName("Language")]
[ReadPermission(PermissionKeys.Translation)]
[ModifyPermission(PermissionKeys.Translation)]
[LookupScript(Permission = "*")]
public sealed class LanguageRow : Row<LanguageRow.RowFields>, IIdRow, INameRow
{
    [DisplayName("Language Code"), Size(10), NotNull, IdProperty, Unique]
    public string LanguageId { get => fields.LanguageId[this]; set => fields.LanguageId[this] = value; }

    [DisplayName("Language Name"), Size(50), NotNull, QuickSearch, NameProperty]
    public string LanguageName { get => fields.LanguageName[this]; set => fields.LanguageName[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public StringField LanguageId;
        public StringField LanguageName;
    }
}