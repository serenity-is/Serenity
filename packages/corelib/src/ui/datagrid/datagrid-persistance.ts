import { Column, Grid } from "@serenity-is/sleekgrid";
import { cssEscape, FilterPanelTexts, Fluent, localText } from "../../base";
import { IRemoteView } from "../../slick";
import { EditorUtils } from "../editors/editorutils";
import { FilterDisplayBar } from "../filtering/filterdisplaybar";
import { FilterLine } from "../filtering/filterline";
import { Toolbar } from "../widgets/toolbar";
import { Widget } from "../widgets/widget";
import { tryGetWidget } from "../widgets/widgetutils";
import { QuickSearchField, QuickSearchInput } from "./quicksearchinput";

export interface SettingStorage {
    getItem(key: string): string | Promise<string>;
    setItem(key: string, value: string): void | Promise<void>;
}

export interface PersistedGridColumn {
    id: string;
    width?: number;
    sort?: number;
    visible?: boolean;
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

export interface GridPersistanceFlags {
    columnWidths?: boolean;
    columnVisibility?: boolean;
    sortColumns?: boolean;
    filterItems?: boolean;
    quickFilters?: boolean;
    quickFilterText?: boolean;
    quickSearch?: boolean;
    includeDeleted?: boolean;
}

export const omitAllGridPersistenceFlags: GridPersistanceFlags = {
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
    filterBar: FilterDisplayBar,
    flags: GridPersistanceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: Fluent,
    slickGrid: Grid,
    toolbar: Toolbar,
    uniqueName: string
}): PersistedGridSettings {

    const flags = opt.flags || {};
    const settings: PersistedGridSettings = {};
    if (flags.columnVisibility !== false || flags.columnWidths !== false || flags.sortColumns !== false) {
        settings.columns = [];
        const sortColumns = opt.slickGrid.getSortColumns() as any[];
        const columns = opt.slickGrid.getColumns();
        for (const column of columns) {
            const p: PersistedGridColumn = {
                id: column.id
            };

            if (flags.columnVisibility !== false) {
                p.visible = true;
            }
            if (flags.columnWidths !== false) {
                p.width = column.width;
            }

            if (flags.sortColumns !== false) {
                const sort = sortColumns.findIndex(x => x.columnId == column.id);
                if (sort >= 0) {
                    p.sort = sortColumns[sort].sortAsc !== false ? (sort + 1) : (-sort - 1);
                }
            }
            settings.columns.push(p);
        }
    }

    if (flags.includeDeleted !== false && opt.includeDeletedToggle) {
        settings.includeDeleted = opt.includeDeletedToggle.matches(".pressed");
    }

    if (flags.filterItems !== false && (opt.filterBar != null) && (opt.filterBar.get_store() != null)) {
        settings.filterItems = opt.filterBar.get_store().get_items().slice();
    }

    if (flags.quickSearch === true) {
        const qsInput = opt.toolbar?.domNode?.querySelector('.s-QuickSearchInput');
        if (qsInput) {
            const qsWidget = tryGetWidget(qsInput, QuickSearchInput);
            if (qsWidget) {
                settings.quickSearchField = qsWidget.get_field();
                settings.quickSearchText = qsWidget.domNode.value;
            }
        }
    }

    if (flags.quickFilters !== false && (opt.quickFiltersDiv != null) && opt.quickFiltersDiv.length > 0) {
        settings.quickFilters = {};
        opt.quickFiltersDiv.findAll('.quick-filter-item').forEach(e => {
            const field = e.dataset.qffield;
            if (!field?.length) {
                return;
            }

            const widget = tryGetWidget('#' + opt.uniqueName + '_QuickFilter_' + field, Widget);
            if (!widget)
                return;

            const qfElement = e as any;
            const saveState = qfElement.qfsavestate;
            const state = typeof saveState === "function" ? saveState(widget) : EditorUtils.getValue(widget);
            settings.quickFilters[field] = state;
            if (flags.quickFilterText === true && e.classList.contains('quick-filter-active')) {

                const getDisplayText = qfElement.qfdisplaytext;
                const filterLabel = e.querySelector('.quick-filter-label')?.textContent ?? '';

                let displayText;
                if (typeof getDisplayText === "function") {
                    displayText = getDisplayText(widget, filterLabel);
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
    allColumns: (value?: Column[]) => Column[],
    canShowColumn: (column: Column) => boolean,
    filterBar: FilterDisplayBar,
    flags: GridPersistanceFlags,
    includeDeletedToggle: HTMLElement,
    quickFiltersDiv: Fluent,
    slickGrid: Grid,
    settings: PersistedGridSettings,
    toolbar: Toolbar,
    uniqueName: string,
    view: IRemoteView<any>
}) {
    let columns = opt.slickGrid.getColumns();
    let colById: { [key: string]: Column } = null;
    const initColById = function (cl: Column[]) {
        colById = {};
        for (let c of cl) {
            colById[c.id] = c;
        }
    };

    const flags = opt.flags || {};
    const settings = opt.settings || {};

    if (settings.columns != null) {
        if (flags.columnVisibility !== false) {
            let allColumns = opt.allColumns();
            initColById(allColumns);
            const newColumns = [];
            for (let x of settings.columns) {
                if (x.id != null && x.visible === true) {
                    const column = colById[x.id];
                    if (opt.canShowColumn(column)) {
                        column.visible = true;
                        newColumns.push(column);
                        delete colById[x.id];
                    }
                }
            }

            for (let c2 of allColumns) {
                if (colById[c2.id] != null) {
                    c2.visible = false;
                    newColumns.push(c2);
                }
            }

            allColumns = opt.allColumns(newColumns);
            columns = allColumns.filter(function (x1) {
                return x1.visible === true;
            });
        }
        if (flags.columnWidths !== false) {
            initColById(columns);
            for (let x2 of settings.columns) {
                if (x2.id != null && x2.width != null && x2.width !== 0) {
                    const column1 = colById[x2.id];
                    if (column1 != null) {
                        column1.width = x2.width;
                    }
                }
            }
        }

        if (flags.sortColumns !== false) {
            initColById(columns);
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
            opt.slickGrid.setSortColumns(list);
        }
        opt.slickGrid.setColumns(columns);
        opt.slickGrid.invalidate();
    }

    if (settings.filterItems != null &&
        flags.filterItems !== false &&
        opt.filterBar != null &&
        opt.filterBar.get_store() != null) {
        const items = opt.filterBar.get_store().get_items();
        items.length = 0;
        items.push.apply(items, settings.filterItems);
        opt.filterBar.get_store().raiseChanged();
    }

    if (settings.includeDeleted != null &&
        flags.includeDeleted !== false) {
        if (opt.includeDeletedToggle && !!settings.includeDeleted !== opt.includeDeletedToggle.classList.contains('pressed')) {
            Fluent.trigger(opt.includeDeletedToggle.querySelector('a'), "click");
        }
    }

    if (settings.quickFilters != null &&
        flags.quickFilters !== false &&
        opt.quickFiltersDiv != null &&
        opt.quickFiltersDiv.length > 0) {
        opt.quickFiltersDiv.findAll('.quick-filter-item').forEach(e => {
            const field = e.dataset.qffield;

            if (!field?.length) {
                return;
            }

            const widget = tryGetWidget('#' + cssEscape(opt.uniqueName + '_QuickFilter_' + field), Widget);
            if (widget == null) {
                return;
            }

            const state = settings.quickFilters[field];
            const loadState = (e as any).qfloadstate;
            if (typeof loadState === "function") {
                loadState(widget, state);
            }
            else {
                EditorUtils.setValue(widget, state);
            }
        });
    }

    if (flags.quickSearch === true && (settings.quickSearchField !== undefined || settings.quickSearchText !== undefined)) {
        const qsInput = opt.toolbar?.domNode?.querySelector('.s-QuickSearchInput');
        if (qsInput) {
            const qsWidget = tryGetWidget(qsInput, QuickSearchInput);
            qsWidget && qsWidget.restoreState(settings.quickSearchText, settings.quickSearchField);
        }
    }
}
