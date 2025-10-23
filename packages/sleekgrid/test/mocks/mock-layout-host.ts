import { signal } from "@serenity-is/signals";
import { gridDefaults, ViewportInfo, type Column, type RowCell } from "../../src/core";
import { GridOptions } from "../../src/core/gridoptions";
import type { GridSignals, LayoutHost } from "../../src/grid/layout";

export function mockLayoutHost(): LayoutHost & {
    signals: GridSignals,
    opt: GridOptions<any>,
    container: HTMLDivElement
} {
    const host = {
        container: document.createElement("div"),
        opt: {
            get showColumnHeader() { return !host.signals.hideColumnHeader.peek(); },
            set showColumnHeader(value: boolean) { host.signals.hideColumnHeader.value = !value; },
            get showHeaderRow() { return !host.signals.hideHeaderRow.peek(); },
            set showHeaderRow(value: boolean) { host.signals.hideHeaderRow.value = !value; },
            get showFooterRow() { return !host.signals.hideFooterRow.peek(); },
            set showFooterRow(value: boolean) { host.signals.hideFooterRow.value = !value; },
            get showTopPanel() { return !host.signals.hideTopPanel.peek(); },
            set showTopPanel(value: boolean) { host.signals.hideTopPanel.value = !value; }
        } as GridOptions<any>,
        signals: {
            hideColumnHeader: signal(!gridDefaults.showColumnHeader),
            hideHeaderRow: signal(!gridDefaults.showHeaderRow),
            hideFooterRow: signal(!gridDefaults.showFooterRow),
            hideTopPanel: signal(!gridDefaults.showTopPanel)
        } satisfies GridSignals,
        bindAncestorScroll: vi.fn(),
        cleanUpAndRenderCells: vi.fn(),
        getAvailableWidth: vi.fn(() => 1000),
        getCellFromPoint: vi.fn(() => ({ row: 0, cell: 0 } as RowCell)),
        getColumnCssRules: vi.fn(() => ({ right: '', left: '' })),
        getColumns: vi.fn(() => [] as Column[]),
        getInitialColumns: vi.fn(() => [] as Column[]),
        getContainerNode: vi.fn(() => host.container),
        getDataLength: vi.fn(() => 0),
        getOptions: vi.fn(() => host.opt),
        getSignals: vi.fn(() => host.signals),
        getRowFromNode: vi.fn(() => null),
        getScrollDims: vi.fn(() => ({ width: 0, height: 0 })),
        getScrollLeft: vi.fn(() => 0),
        getScrollTop: vi.fn(() => 0),
        getViewportInfo: vi.fn(() => ({} as ViewportInfo)),
        removeNode: vi.fn(),
        renderRows: vi.fn()
    };
    return host;
}
