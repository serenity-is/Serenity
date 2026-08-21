namespace Serenity.Extensions.Entities;

/// <summary>
/// Row for user preferences.
/// </summary>
[ConnectionKey("Default"), Module("Common"), TableName("UserPreferences")]
[DisplayName("User Preferences"), InstanceName("UserPreference")]
[ReadPermission("")]
[ModifyPermission("")]
public sealed class UserPreferenceRow : Row<UserPreferenceRow.RowFields>, IIdRow, INameRow
{
    /// <summary>
    /// The user preference ID.
    /// </summary>
    [DisplayName("ID"), Identity, IdProperty]
    public int? UserPreferenceId { get => fields.UserPreferenceId[this]; set => fields.UserPreferenceId[this] = value; }

    /// <summary>
    /// The ID of the user the preference belongs to.
    /// </summary>
    [DisplayName("User ID")]
    public int? UserId { get => fields.UserId[this]; set => fields.UserId[this] = value; }

    /// <summary>
    /// The preference type.
    /// </summary>
    [DisplayName("PreferenceType"), Size(100), NotNull]
    public string PreferenceType { get => fields.PreferenceType[this]; set => fields.PreferenceType[this] = value; }

    /// <summary>
    /// The preference name.
    /// </summary>
    [DisplayName("Name"), Size(100), NotNull, QuickSearch, NameProperty]
    public string Name { get => fields.Name[this]; set => fields.Name[this] = value; }

    /// <summary>
    /// The preference value.
    /// </summary>
    [DisplayName("Value")]
    public string Value { get => fields.Value[this]; set => fields.Value[this] = value; }

    public class RowFields : RowFieldsBase
    {
        public readonly Int32Field UserPreferenceId;
        public readonly Int32Field UserId;
        public readonly StringField PreferenceType;
        public readonly StringField Name;
        public readonly StringField Value;
    }
}