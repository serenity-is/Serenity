import { type Column, type ISleekGrid } from "@serenity-is/sleekgrid";
import { cssEscape, FilterPanelTexts, Fluent } from "../../base";
import { type IRemoteView } from "../../slick";
import { EditorUtils } from "../editors/editorutils";
import { type FilterLine } from "../filtering/filterline";
import type { FilterStore } from "../filtering/filterstore";
import { type Widget } from "../widgets/widget";
import { tryGetWidget } from "../widgets/widgetutils";
import { QuickFilterBar } from "./quickfilterbar";
import { QuickSearchField, QuickSearchInput } from "./quicksearchinput";
import type { DataGridEvent } from "./datagrid";

/**
 * Minimal storage abstraction used for grid persistence.
 * Implementations may be synchronous (localStorage) or asynchronous.
 */
export interface SettingStorage {
    /**
     * Retrieves a stored value by key.
     * @param key - Storage key.
     * @returns Stored value or a promise that resolves to it.
     */
    getItem(key: string): string | Promise<string>;
    /**
     * Persists a value under the given key.
     * @param key - Storage key.
     * @param value - Value to store.
     * @returns Void or a promise that resolves when the write completes.
     */
    setItem(key: string, value: string): void | Promise<void>;
}

/**
 * Persisted state for a single grid column.
 */
export interface PersistedGridColumn {
    /** Column identifier (matches {@link Column.id}). */
    id: string;
    /** Persisted width in pixels. */
    width?: number;
    /** Sort order and direction; positive for ascending, negative for descending. */
    sort?: number;
    /** Whether the column is visible. */
    visible?: boolean;
    /** Frozen / pinned state of the column. */
    pin?: "start" | "end" | false;
}

/**
 * Snapshot of grid state that can be persisted and later restored.
 */
export interface PersistedGridSettings {
    /** Flags that indicate which parts of the settings were persisted. */
    flags?: GridPersistenceFlags;
    /** Column state (order, width, visibility, pinning, sort). */
    columns?: PersistedGridColumn[];
    /** Advanced filter panel items. */
    filterItems?: FilterLine[];
    /** Quick filter widget states keyed by field name. */
    quickFilters?: { [key: string]: any };
    /** Concatenated display text for active quick filters. */
    quickFilterText?: string;
    /** Field selected in the quick search input. */
    quickSearchField?: QuickSearchField;
    /** Text entered in the quick search input. */
    quickSearchText?: string;
    /** Whether the include-deleted toggle was pressed. */
    includeDeleted?: boolean;
}

/**
 * Flags controlling which parts of grid state are persisted.
 * Unspecified flags fall back to {@link defaultGridPersistenceFlags}.
 */
export interface GridPersistenceFlags {
    /** Column pinning state. Defaults to persist unless explicitly set to false. */
    columnPinning?: boolean;
    /** Column widths. Defaults to persist unless explicitly set to false. */
    columnWidths?: boolean;
    /** Column visibility. Defaults to persist unless explicitly set to false. */
    columnVisibility?: boolean;
    /** Sort columns. Defaults to persist unless explicitly set to false. */
    sortColumns?: boolean;
    /** Filter items. Defaults to persist unless explicitly set to false. */
    filterItems?: boolean;
    /** Quick filter values. Defaults to persist unless explicitly set to false. */
    quickFilters?: boolean;
    /** Quick filter display text. Only persists when explicitly set to true. */
    quickFilterText?: boolean;
    /** Quick search input text. Only persists when explicitly set to true. */
    quickSearch?: boolean;
    /** Include deleted toggle state. Defaults to persist unless explicitly set to false. */
    includeDeleted?: boolean;
}

/**
 * @deprecated Use {@link GridPersistenceFlags}; retained for backwards compatibility (typo in original name).
 */
export type GridPersistanceFlags = GridPersistenceFlags;

/** Default persistence flags; most grid state is persisted except transient search text. */
export const defaultGridPersistenceFlags: GridPersistenceFlags = {
    columnPinning: true,
    columnWidths: true,
    columnVisibility: true,
    sortColumns: true,
    filterItems: true,
    quickFilters: true,
    quickFilterText: false,
    quickSearch: false,
    includeDeleted: true
};

/** Flags that disable persistence for every grid aspect. */
export const omitAllGridPersistenceFlags: GridPersistenceFlags = {
    columnPinning: false,
    columnWidths: false,
    columnVisibility: false,
    sortColumns: false,
    filterItems: false,
    quickFilters: false,
    quickFilterText: false,
    quickSearch: false,
    includeDeleted: false
};
/**
 * Builds a {@link PersistedGridSettings} snapshot from the current grid state.
 * @param opt - Context containing the grid, view, filter store and persistence flags.
 * @returns Snapshot of the current grid settings.
 */
export function getCurrentSettings(this: void, opt: {
    filterStore: FilterStore,
    flags: GridPersistenceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: HTMLElement,
    sleekGrid: ISleekGrid,
    toolbarNode: HTMLElement,
    uniqueName: string
}): PersistedGridSettings {

    const flags = Object.assign({}, defaultGridPersistenceFlags, opt.flags || {});
    const settings: PersistedGridSettings = {
        flags: flags
    };
    if (flags.columnVisibility ||
        flags.columnWidths ||
        flags.columnPinning ||
        flags.sortColumns) {
        settings.columns = [];
        const sortColumns = opt.sleekGrid.getSortColumns();
        const columns = opt.sleekGrid.getAllColumns();
        for (const column of columns) {
            const p: PersistedGridColumn = {
                id: column.id
            };

            if (flags.columnPinning) {
                p.pin = column.frozen ? (column.frozen !== "end" ? "start" : "end") : false;
            }

            if (flags.columnVisibility) {
                p.visible = column.visible !== false;
            }

            if (flags.columnWidths) {
                p.width = column.width;
            }

            if (flags.sortColumns) {
                const sort = sortColumns.findIndex(x => x.columnId == column.id);
                if (sort >= 0) {
                    p.sort = sortColumns[sort].sortAsc !== false ? (sort + 1) : (-sort - 1);
                }
            }
            settings.columns.push(p);
        }
    }

    if (flags.includeDeleted && opt.includeDeletedToggle) {
        settings.includeDeleted = opt.includeDeletedToggle.matches(".pressed");
    }

    if (flags.filterItems && opt.filterStore) {
        settings.filterItems = opt.filterStore.get_items().slice();
    }

    if (flags.quickSearch) {
        const qsInput = opt.toolbarNode?.querySelector('.s-QuickSearchInput');
        if (qsInput) {
            const qsWidget = tryGetWidget(qsInput, QuickSearchInput);
            if (qsWidget) {
                settings.quickSearchField = qsWidget.get_field();
                settings.quickSearchText = qsWidget.domNode.value;
            }
        }
    }

    if (flags.quickFilters && (opt.quickFiltersDiv != null)) {
        settings.quickFilters = {};
        opt.quickFiltersDiv.querySelectorAll<HTMLElement>('.quick-filter-item').forEach(filterItem => {
            const field = filterItem.dataset.qffield;
            if (!field?.length) {
                return;
            }

            const widget = tryGetWidget<Widget>('#' + cssEscape(opt.uniqueName + '_QuickFilter_' + field));
            if (!widget)
                return;

            const qfData = QuickFilterBar.getItemData(filterItem);
            const state = typeof qfData?.saveState === "function" ? qfData.saveState(widget) : EditorUtils.getValue(widget);
            settings.quickFilters[field] = state;
            if (flags.quickFilterText && filterItem.classList.contains('quick-filter-active')) {

                const filterLabel = filterItem.querySelector('.quick-filter-label')?.textContent ?? '';

                let displayText;
                if (typeof qfData?.displayText === "function") {
                    displayText = qfData.displayText(widget, filterLabel);
                }
                else {
                    displayText = filterLabel + ' = ' + EditorUtils.getDisplayText(widget);
                }

                if (displayText?.length) {
                    if (settings.quickFilterText?.length) {
                        settings.quickFilterText += ' ' + FilterPanelTexts.And + ' ';
                        settings.quickFilterText += displayText;
                    }
                    else {
                        settings.quickFilterText = displayText;
                    }
                }
            }
        });
    }
    return settings;
}

/**
 * Restores grid state from a previously persisted snapshot.
 * @param opt - Context containing the grid, view, stores and the settings to restore.
 */
export function restoreSettingsFrom(this: void, opt: {
    canShowColumn?: (column: Column) => boolean,
    filterStore: FilterStore,
    flags: GridPersistenceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: HTMLElement,
    sleekGrid: ISleekGrid,
    settings: PersistedGridSettings,
    toolbarNode: HTMLElement,
    uniqueName: string,
    view: IRemoteView<any>
}) {
    let allColumns = opt.sleekGrid.getAllColumns();
    let colById: { [key: string]: Column } = Object.create(null);
    for (let c of allColumns) {
        colById[c.id] = c;
    }

    const flags = Object.assign({}, defaultGridPersistenceFlags, opt.flags || {});
    const settings = opt.settings || {};

    if (settings.columns != null) {

        if (flags.columnPinning &&
            (settings.flags?.columnPinning 
                ?? settings.columns.some(x => "pin" in x))) {
            for (let x1 of settings.columns) {
                if (x1.id != null) {
                    const column = colById[x1.id];
                    if (column != null) {
                        column.frozen = x1.pin === "start" ||
                            x1.pin === "end" ? x1.pin : null;
                    }
                }
            }
        }

        if (flags.columnWidths && (settings.flags?.columnWidths 
                ?? settings.columns.some(c => "width" in c))) {
            for (let x2 of settings.columns) {
                if (x2.id != null && x2.width != null && x2.width !== 0) {
                    const column1 = colById[x2.id];
                    if (column1 != null) {
                        column1.width = x2.width;
                    }
                }
            }
        }

        if (flags.sortColumns && (settings.flags?.sortColumns ??
                settings.columns.some(c => "sort" in c))) {
            const list = [];
            const sortColumns = settings.columns.filter(function (x3) {
                return x3.id != null && (x3.sort ?? 0) !== 0;
            });

            sortColumns.sort(function (a, b) {
                // sort holds two informations:
                // absolute value: order of sorting
                // sign: positive = ascending, negative = descending
                // so we have to compare absolute values here
                return Math.abs(a.sort) - Math.abs(b.sort);
            });

            for (let x4 of sortColumns) {
                const column2 = colById[x4.id];
                if (column2 != null) {
                    list.push({
                        columnId: x4.id,
                        sortAsc: x4.sort > 0
                    });
                }
            }
            opt.view.sortBy = list.map(function (x5) {
                return x5.columnId + ((x5.sortAsc === false) ? ' DESC' : '');
            });
            opt.sleekGrid.setSortColumns(list);
        }

        if (flags.columnVisibility && (settings.flags?.columnVisibility
                ?? settings.columns.some(x => "visible" in x))) {
            let visibleColumns = settings.columns.filter(x => x.id != null &&
                x.visible === true &&
                colById[x.id] &&
                (opt.canShowColumn ?? isVisibleOrTogglable)(colById[x.id])
            ).map(x => x.id);

            const alwaysVisibleColumns = allColumns.filter(c => c.visible !== false && 
                c.togglable === false).map(c => c.id).filter(id => !visibleColumns.includes(id));
            visibleColumns = alwaysVisibleColumns.concat(visibleColumns);

            opt.sleekGrid.setVisibleColumns(visibleColumns, { notify: false });
        }
        else {
            opt.sleekGrid.invalidateColumns();
        }

        opt.sleekGrid.invalidate();
    }

    if (settings.filterItems != null &&
        flags.filterItems &&
        opt.filterStore) {
        const items = opt.filterStore.get_items();
        items.length = 0;
        items.push.apply(items, settings.filterItems);
        opt.filterStore.raiseChanged();
    }

    if (settings.includeDeleted != null &&
        flags.includeDeleted) {
        if (opt.includeDeletedToggle && !!settings.includeDeleted !== opt.includeDeletedToggle.classList.contains('pressed')) {
            Fluent.trigger(opt.includeDeletedToggle.querySelector('a'), "click");
        }
    }

    if (settings.quickFilters != null &&
        flags.quickFilters &&
        opt.quickFiltersDiv != null) {
        opt.quickFiltersDiv.querySelectorAll<HTMLElement>('.quick-filter-item').forEach(e => {
            const field = e.dataset.qffield;

            if (!field?.length) {
                return;
            }

            const widget = tryGetWidget<Widget>('#' + cssEscape(opt.uniqueName + '_QuickFilter_' + field));
            if (widget == null) {
                return;
            }

            const state = settings.quickFilters[field];
            const loadState = QuickFilterBar.getItemData(e)?.loadState;
            if (typeof loadState === "function") {
                loadState(widget, state);
            }
            else {
                EditorUtils.setValue(widget, state);
            }
        });
    }

    if (flags.quickSearch && (settings.flags?.quickSearch ??
         ((settings.quickSearchField !== undefined || settings.quickSearchText !== undefined)))) {
        const qsInput = opt.toolbarNode.querySelector('.s-QuickSearchInput');
        if (qsInput) {
            const qsWidget = tryGetWidget(qsInput, QuickSearchInput);
            qsWidget && qsWidget.restoreState(settings.quickSearchText, settings.quickSearchField);
        }
    }
}

/**
 * Event arguments for grid persistence hooks (before/after persist and restore).
 */
export interface DataGridPersistenceEvent extends DataGridEvent {
    /** Whether this is the after phase of the operation. */
    after: boolean;
    /** Flags passed by the caller. */
    flagsArgument: GridPersistenceFlags;
    /** Default flags for the grid type. */
    flagsDefault: GridPersistenceFlags;
    /** Effective flags after merging argument and defaults. */
    flagsToUse: GridPersistenceFlags;
    /** Settings being persisted or restored. */
    settings: PersistedGridSettings;
    /** True while the grid is restoring settings. */
    readonly restoring: boolean;
    /** True while the grid is persisting settings. */
    readonly persisting: boolean;
}

function isVisibleOrTogglable(column: Column): boolean {
    return !!column && (column.visible !== false || column.togglable !== false);
}