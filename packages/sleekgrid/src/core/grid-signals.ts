import type { Computed, Signal } from "@serenity-is/domwise";

export interface GridSignals {
    readonly showColumnHeader: Signal<boolean>;
    readonly hideColumnHeader: Computed<boolean>;
    readonly showTopPanel: Signal<boolean>;
    readonly hideTopPanel: Computed<boolean>;
    readonly showHeaderRow: Signal<boolean>;
    readonly hideHeaderRow: Computed<boolean>;
    readonly showFooterRow: Signal<boolean>;
    readonly hideFooterRow: Computed<boolean>;
    readonly pinnedStartCols: Signal<number>;
    readonly pinnedEndCols: Signal<number>;
    readonly frozenTopRows: Signal<number>;
    readonly frozenBottomRows: Signal<number>;
}
