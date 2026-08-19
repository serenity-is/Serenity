/**
 * Base class for special rows that do not represent regular data items.
 * Group headers and group totals derive from this to allow the grid and
 * data view to distinguish them from plain data rows via the marker property.
 */
export class NonDataRow {
    /**
     * Marker flag used at runtime to identify non-data rows.
     * Checked by the grid and `DataView` to skip data-specific handling.
     */
    __nonDataRow: boolean = true;
}

/**
 * CSS class applied to a cell that received a mousedown immediately before
 * an editor is activated. Editors can check for this class to decide whether
 * to select text or preserve the click target.
 */
export const preClickClassName = "slick-edit-preclick";
