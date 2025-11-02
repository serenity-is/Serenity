import type { ReadonlySignal, Signal } from "@preact/signals-core";

export interface GridSignals {
    readonly showColumnHeader: Signal<boolean>;
    readonly hideColumnHeader: ReadonlySignal<boolean>;
    readonly showTopPanel: Signal<boolean>;
    readonly hideTopPanel: ReadonlySignal<boolean>;
    readonly showHeaderRow: Signal<boolean>;
    readonly hideHeaderRow: ReadonlySignal<boolean>;
    readonly showFooterRow: Signal<boolean>;
    readonly hideFooterRow: ReadonlySignal<boolean>;
    readonly pinnedStartLast: Signal<number>;
    readonly pinnedEndFirst: Signal<number>;
    readonly frozenTopLast: Signal<number>;
    readonly frozenBottomFirst: Signal<number>;
}
