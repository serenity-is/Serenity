namespace Serenity.ComponentModel;

/// <summary>
/// Where a record view of a property is shown, see <see cref="RecordViewTypeAttribute"/>.
/// </summary>
public enum RecordViewPlacement
{
    /// <summary>
    /// In a popup, e.g. a hover card, that the value of the property opens when it is hovered, focused or long
    /// pressed, in a grid cell or wherever the value is shown.
    /// </summary>
    Trigger = 0
}
