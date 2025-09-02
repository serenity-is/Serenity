namespace Serenity.Data;

/// <summary>
/// Maps a row to its localization row
/// </summary>
/// <remarks>
/// Creates a new instance of the attribute
/// </remarks>
/// <param name="localizationRow">Localization row type</param>
/// <exception cref="ArgumentNullException">localizationRow is null</exception>
public class LocalizationRowAttribute(Type localizationRow) : Attribute
{

    /// <summary>
    /// Localization row type
    /// </summary>
    public Type LocalizationRow { get; private set; } = localizationRow ?? throw new ArgumentNullException(nameof(localizationRow));

    /// <summary>
    /// ID field corresponding to this tables ID field.
    /// Can be automatically determined if not specified.
    /// </summary>
    public string MappedIdField { get; set; }


    /// <summary>
    /// If set to true returns translated values by default for the list handler (available in Pro.Extensions package)
    /// when ListRequest.Localize is not set, e.g. it is null.
    /// Note that if this is enabled, Request.Localize parameter must be specified as empty
    /// string to get original values.
    /// </summary>
    public bool LocalizeListByDefault { get; set; }
}