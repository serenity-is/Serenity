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

export interface SettingStorage {
    getItem(key: string): string | Promise<string>;
    setItem(key: string, value: string): void | Promise<void>;
}

export interface PersistedGridColumn {
    id: string;
    width?: number;
    sort?: number;
    visible?: boolean;
    pin?: "start" | "end" | false;
}

export interface PersistedGridSettings {
    columns?: PersistedGridColumn[];
    filterItems?: FilterLine[];
    quickFilters?: { [key: string]: any };
    quickFilterText?: string;
    quickSearchField?: QuickSearchField;
    quickSearchText?: string;
    includeDeleted?: boolean;
}

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

/** @deprecated Use GridPersistenceFlags, this one has a typo in the name */
export type GridPersistanceFlags = GridPersistenceFlags;

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
export function getCurrentSettings(this: void, opt: {
    filterStore: FilterStore,
    flags: GridPersistenceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: Fluent,
    sleekGrid: ISleekGrid,
    toolbarNode: HTMLElement,
    uniqueName: string
}): PersistedGridSettings {

    const flags = Object.assign({}, defaultGridPersistenceFlags, opt.flags || {});
    const settings: PersistedGridSettings = {};
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

            if (flags.columnPinning && column.frozen) {
                p.pin = column.frozen !== "end" ? "start" : "end";
            }

            if (flags.columnVisibility && column.visible !== false) {
                p.visible = true;
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

        if (flags.columnPinning && settings.columns.length > 0 && !settings.columns.some(x => "pin" in x)) {
            // ensure at least one column has pinned info so that while restoring we know pinning flag was used
            settings.columns[0].pin = false;
        }

        if (flags.columnVisibility && settings.columns.length > 0 && !settings.columns.some(x => "visible" in x)) {
            // ensure at least one column has visibility info so that while restoring we know visibility flag was used
            settings.columns[0].visible = false;
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

    if (flags.quickFilters && (opt.quickFiltersDiv != null) && opt.quickFiltersDiv.length > 0) {
        settings.quickFilters = {};
        opt.quickFiltersDiv.findAll('.quick-filter-item').forEach(filterItem => {
            const field = filterItem.dataset.qffield;
            if (!field?.length) {
                return;
            }

            const widget = tryGetWidget<Widget>('#' + opt.uniqueName + '_QuickFilter_' + field);
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

export function restoreSettingsFrom(this: void, opt: {
    canShowColumn: (column: Column) => boolean,
    filterStore: FilterStore,
    flags: GridPersistenceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: Fluent,
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
            settings.columns.some(x => "pin" in x)) {
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

        if (flags.columnWidths) {
            for (let x2 of settings.columns) {
                if (x2.id != null && x2.width != null && x2.width !== 0) {
                    const column1 = colById[x2.id];
                    if (column1 != null) {
                        column1.width = x2.width;
                    }
                }
            }
        }

        if (flags.sortColumns) {
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

        if (flags.columnVisibility &&
            settings.columns.some(x => "visible" in x)) {
            const visibleColumns = settings.columns.filter(x => x.id != null &&
                x.visible === true &&
                colById[x.id] &&
                opt.canShowColumn(colById[x.id])
            ).map(x => x.id);

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
        opt.quickFiltersDiv != null &&
        opt.quickFiltersDiv.length > 0) {
        opt.quickFiltersDiv.findAll('.quick-filter-item').forEach(e => {
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

    if (flags.quickSearch && (settings.quickSearchField !== undefined || settings.quickSearchText !== undefined)) {
        const qsInput = opt.toolbarNode.querySelector('.s-QuickSearchInput');
        if (qsInput) {
            const qsWidget = tryGetWidget(qsInput, QuickSearchInput);
            qsWidget && qsWidget.restoreState(settings.quickSearchText, settings.quickSearchField);
        }
    }
}

export interface GridPersistenceEvent extends DataGridEvent {
    after: boolean;
    flagsArgument: GridPersistenceFlags;
    flagsDefault: GridPersistenceFlags;
    flagsToUse: GridPersistenceFlags;
    settings: PersistedGridSettings;
    readonly restoring: boolean;
    readonly persisting: boolean;
}