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
    Trigger = 0,

    /// <summary>
    /// As the card of each record in an alternate view of the rows of a grid, e.g. a list of cards on a phone, on a
    /// columns class. The generated attributes of the client side card view classes, which derive from
    /// <c>CardView</c>, have it as their placement.
    /// </summary>
    Card = 1
}
