namespace Serenity.ComponentModel;

/// <summary>
/// Where the buttons that switch between the views of a grid go, when the view is on a columns class, e.g. a card
/// view, see <see cref="RecordViewTypeAttribute.SwitchPlacement"/>. A grid that lacks the part a placement names puts
/// the buttons in the closest one it has, the title bar being the last resort.
/// </summary>
public enum CardViewSwitchPlacement
{
    /// <summary>
    /// As close to the table as the grid allows, which is the default: at the end of the bar of its quick filters, which
    /// is the row above the table when it has them, else at the end of its toolbar, else in its title bar.
    /// </summary>
    Closest = 0,

    /// <summary>
    /// In the title bar of the grid, to the right of its title.
    /// </summary>
    Title = 1,

    /// <summary>
    /// At the end of the bar of the quick filters of the grid, which is the row above its table, when it has quick
    /// filters.
    /// </summary>
    Filters = 2,

    /// <summary>
    /// At the end of the toolbar of the grid, when it has a toolbar.
    /// </summary>
    Toolbar = 3
}
