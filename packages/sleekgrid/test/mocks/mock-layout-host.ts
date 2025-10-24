import { computed, signal } from "@serenity-is/signals";
import type { Column } from "../../src/core/column";
import type { RowCell } from "../../src/core/editing";
import type { GridSignals } from "../../src/core/grid-signals";
import { gridDefaults, GridOptions } from "../../src/core/gridoptions";
import type { ViewportInfo } from "../../src/core/viewportinfo";
import type { LayoutHost } from "../../src/layouts/layout-host";

export function mockLayoutHost(): LayoutHost & {
    signals: GridSignals,
    opt: GridOptions<any>,
    container: HTMLDivElement
} {
    const host = {
        container: document.createElement("div"),
        opt: {
            get showColumnHeader() { return host.signals.showColumnHeader.peek(); },
            set showColumnHeader(value: boolean) { host.signals.showColumnHeader.value = value; },
            get showHeaderRow() { return host.signals.showHeaderRow.peek(); },
            set showHeaderRow(value: boolean) { host.signals.showHeaderRow.value = value; },
            get showFooterRow() { return host.signals.showFooterRow.peek(); },
            set showFooterRow(value: boolean) { host.signals.showFooterRow.value = value; },
            get showTopPanel() { return !host.signals.hideTopPanel.peek(); },
            set showTopPanel(value: boolean) { host.signals.showTopPanel.value = value; }
        } as GridOptions<any>,
        signals: {
            showColumnHeader: signal(gridDefaults.showColumnHeader),
            hideColumnHeader: computed(() => !host.signals.showColumnHeader.value),
            showTopPanel: signal(gridDefaults.showTopPanel),
            hideTopPanel: computed(() => !host.signals.showTopPanel.value),
            showHeaderRow: signal(gridDefaults.showHeaderRow),
            hideHeaderRow: computed(() => !host.signals.showHeaderRow.value),
            showFooterRow: signal(gridDefaults.showFooterRow),
            hideFooterRow: computed(() => !host.signals.showFooterRow.value),
        } satisfies GridSignals,
        bindAncestorScroll: vi.fn(),
        cleanUpAndRenderCells: vi.fn(),
        getAvailableWidth: vi.fn(() => 1000),
        getCellFromPoint: vi.fn(() => ({ row: 0, cell: 0 } as RowCell)),
        getColumns: vi.fn(() => [] as Column[]),
        getInitialColumns: vi.fn(() => [] as Column[]),
        getContainerNode: vi.fn(() => host.container),
        getDataLength: vi.fn(() => 0),
        getOptions: vi.fn(() => host.opt),
        getSignals: vi.fn(() => host.signals),
        getRowFromNode: vi.fn(() => null),
        getScrollDims: vi.fn(() => ({ width: 0, height: 0 })),
        getViewportInfo: vi.fn(() => ({} as ViewportInfo)),
        removeNode: vi.fn(),
        renderRows: vi.fn()
    };
    return host;
}
