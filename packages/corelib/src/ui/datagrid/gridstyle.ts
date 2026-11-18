import { registerEnum } from "../../base";

/**
 * What a grid hides of the look its theme gives to grids, like the stripes of its rows, so it looks more like a plain table.
 * The members are flags, so they can be combined. A grid that has none of them, or has no style, looks as the theme says.
 */
export enum GridStyle {
    /** The rows are not striped, which is only a change for a theme that stripes them. */
    NoStripes = 1,
    /** There are no lines between the rows. */
    NoCellBorders = 2,
    /** The header is plain, without a background, a shadow, a top border, separators between the columns or menu buttons that are always visible. */
    NoHeaderChrome = 4,
    /** A plain header, and lines between the rows without stripes. */
    Lines = NoStripes | NoHeaderChrome,
    /** None of the stripes, the lines between the rows and the chrome of the header, so the grid looks like a plain table. */
    Plain = NoStripes | NoCellBorders | NoHeaderChrome
}

registerEnum(GridStyle, "Serenity.ComponentModel.GridStyle", { enumFlags: true });
