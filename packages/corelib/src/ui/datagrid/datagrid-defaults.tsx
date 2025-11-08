import type { DataGrid } from "./datagrid";
import { defaultGridPersistenceFlags, type SettingStorage } from "./datagrid-persistence";

export const dataGridDefaults = {
    /** Default column width delta. This value if specified, is added to the width of columns defined server side. Default is null */
    columnWidthDelta: null as number,
    /** Default column width scale. This value if specified, is multiplied with the width of columns defined server side. Default is null */
    columnWidthScale: null as number,
    /** Controls whether to enable advanced filtering, e.g. via filter dialog/bar. Default is null. */
    enableAdvancedFiltering: null as (boolean | ((grid: DataGrid<any>) => boolean)),
    /** Controls whether to open dialogs as panels. Default is null. */
    openDialogsAsPanel: null as boolean,
    /** Default row height. Default is null. */
    rowHeight: null as number,
    /** Default persistence flags. Defaults are true except quickSearch and quickFilterText */
    persistenceFlags: defaultGridPersistenceFlags,
    /** Default persistence storage. Default is null */
    persistenceStorage: null as SettingStorage,
};