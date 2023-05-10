namespace Serenity.Data;

/// <summary>
/// Maps a row to its localization row
/// </summary>
public class LocalizationRowAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of the attribute
    /// </summary>
    /// <param name="localizationRow">Localization row type</param>
    /// <exception cref="ArgumentNullException">localizationRow is null</exception>
    public LocalizationRowAttribute(Type localizationRow)
    {
        LocalizationRow = localizationRow ?? throw new ArgumentNullException(nameof(localizationRow));
    }

    /// <summary>
    /// Localization row type
    /// </summary>
    public Type LocalizationRow { get; private set; }

    /// <summary>
    /// ID field corresponding to this tables ID field.
    /// Can be automatically determined if not specified.
    /// </summary>
    public string MappedIdField { get; set; }
}