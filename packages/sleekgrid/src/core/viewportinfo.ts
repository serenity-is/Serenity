/**
 * Computed layout metrics for the grid viewport. Calculated during `computeViewportInfo()`
 * and used to size canvases, set scroll extents and decide virtualization bounds.
 */
export interface ViewportInfo {
    /** Height of the scrollable viewport in pixels. */
    height: number;
    /** Width of the scrollable viewport in pixels. */
    width: number;
    /** Whether a vertical scrollbar is currently present. */
    hasVScroll: boolean;
    /** Whether a horizontal scrollbar is currently present. */
    hasHScroll: boolean;
    /** Height of the column header row in pixels. */
    headerHeight: number;
    /** Height of the grouping panel in pixels. */
    groupingPanelHeight: number;
    /** Total virtual height of all rows (`rowHeight * rowCount`), before capping. */
    virtualHeight: number;
    /** Actual scrollable height applied to the canvas (capped for very large data sets). */
    realScrollHeight: number;
    /** Height of the top panel in pixels. */
    topPanelHeight: number;
    /** Height of the header-row (filter row) in pixels. */
    headerRowHeight: number;
    /** Height of the footer row in pixels. */
    footerRowHeight: number;
    /** Number of rows estimated to fit in the current viewport (`ceil(height/rowHeight)+1`). */
    numVisibleRows: number;
}
