import type { ReadonlySignal, Signal } from "@serenity-is/signals";

export interface GridSignals {
    readonly showColumnHeader: Signal<boolean>;
    readonly hideColumnHeader: ReadonlySignal<boolean>;
    readonly showTopPanel: Signal<boolean>;
    readonly hideTopPanel: ReadonlySignal<boolean>;
    readonly showHeaderRow: Signal<boolean>;
    readonly hideHeaderRow: ReadonlySignal<boolean>;
    readonly showFooterRow: Signal<boolean>;
    readonly hideFooterRow: ReadonlySignal<boolean>;
}
