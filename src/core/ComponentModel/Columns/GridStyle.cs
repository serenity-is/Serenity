using System.ComponentModel;

namespace Serenity.ComponentModel;

/// <summary>
/// What a grid hides of the look its theme gives to grids, like the stripes of its rows, so it looks more like a plain table.
/// The members are flags, so they can be combined. A grid that has none of them, or has no style, looks as the theme says.
/// </summary>
[Flags]
public enum GridStyle
{
    /// <summary>
    /// The rows are not striped, which is only a change for a theme that stripes them.
    /// </summary>
    [Description("No Stripes")]
    NoStripes = 1,

    /// <summary>
    /// There are no lines between the rows.
    /// </summary>
    [Description("No Cell Borders")]
    NoCellBorders = 2,

    /// <summary>
    /// The header is plain, without a background, a shadow, a top border, separators between the columns or menu buttons
    /// that are always visible.
    /// </summary>
    [Description("No Header Chrome")]
    NoHeaderChrome = 4,

    /// <summary>
    /// A plain header, and lines between the rows without stripes.
    /// </summary>
    [Description("Lines")]
    Lines = NoStripes | NoHeaderChrome,

    /// <summary>
    /// None of the stripes, the lines between the rows and the chrome of the header, so the grid looks like a plain table.
    /// </summary>
    [Description("Plain")]
    Plain = NoStripes | NoCellBorders | NoHeaderChrome
}
