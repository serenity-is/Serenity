import type { DataGrid } from "./datagrid";
import { defaultGridPersistenceFlags, type SettingStorage } from "./datagrid-persistence";

export const dataGridDefaults = {
    /** Default column width delta. This value if specified, is added to the width of columns defined server side. Default is null */
    columnWidthDelta: null as number,
    /** Default column width scale. This value if specified, is multiplied with the width of columns defined server side. Default is null */
    columnWidthScale: null as number,
    /** Controls whether to enable advanced filtering, e.g. via filter dialog/bar. Default is null. */
    enableAdvancedFiltering: null as (boolean | ((grid: DataGrid<any>) => boolean)),
    /**
     * Controls whether the first cell of the data rows inside a group is indented according to the
     * grouping level, so the data rows line up under their group rows. It is passed to the view in
     * getViewOptions, and to the group item metadata provider as its `indentDataRows` option. Default
     * is null, which leaves it to `GroupItemMetadataProvider.defaults.indentDataRows`, which is true.
     */
    indentDataRows: null as boolean,
    /** Controls whether to open dialogs as panels. Default is null. */
    openDialogsAsPanel: null as boolean,
    /** Controls whether the views of the grids ask the server for the aggregates of the summary options, e.g. the totals in the footer, from all the rows and not only the page. Grids can override it in getViewOptions. Default is null, which is false. */
    serverAggregates: null as boolean,
    /** Default row height. Default is null. */
    rowHeight: null as number,
    /** Default persistence flags. Defaults are true except quickSearch and quickFilterText */
    persistenceFlags: defaultGridPersistenceFlags,
    /** Default persistence storage. Default is null */
    persistenceStorage: null as SettingStorage,
};