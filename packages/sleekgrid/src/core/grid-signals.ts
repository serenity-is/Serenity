import type { Computed, Signal } from "@serenity-is/domwise";

/**
 * Reactive signals surface for the grid's chrome and pinning state.
 * Backed by `@serenity-is/domwise` signals and used internally by the grid
 * and layout engine to drive visibility and pinning.
 */
export interface GridSignals {
    /** Whether the column header row is visible. */
    readonly showColumnHeader: Signal<boolean>;
    /** Inverse of {@link GridSignals.showColumnHeader}; `true` when the header is hidden. */
    readonly hideColumnHeader: Computed<boolean>;
    /** Whether the top panel row is visible. */
    readonly showTopPanel: Signal<boolean>;
    /** Inverse of {@link GridSignals.showTopPanel}. */
    readonly hideTopPanel: Computed<boolean>;
    /** Whether the header row (filter row) is visible. */
    readonly showHeaderRow: Signal<boolean>;
    /** Inverse of {@link GridSignals.showHeaderRow}. */
    readonly hideHeaderRow: Computed<boolean>;
    /** Whether the footer row is visible. */
    readonly showFooterRow: Signal<boolean>;
    /** Inverse of {@link GridSignals.showFooterRow}. */
    readonly hideFooterRow: Computed<boolean>;
    /** Number of columns pinned to the start (left in LTR, right in RTL) side. */
    readonly pinnedStartCols: Signal<number>;
    /** Number of columns pinned to the end (right in LTR, left in RTL) side. */
    readonly pinnedEndCols: Signal<number>;
    /** Number of rows frozen at the top of the viewport. */
    readonly frozenTopRows: Signal<number>;
    /** Number of rows frozen at the bottom of the viewport. */
    readonly frozenBottomRows: Signal<number>;
}
