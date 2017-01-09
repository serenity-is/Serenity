declare namespace Serenity {
    class GridRows<TItem> {
    }

    class GridRowSelectionMixin extends ScriptContext {
        constructor(grid: IDataGrid);
        clear(): void;
        resetCheckedAndRefresh(): void;
        selectKeys(keys: string[]): void;
        getSelectedKeys(): string[];
        getSelectedAsInt32(): number[];
        getSelectedAsInt64(): number[];
        setSelectedKeys(keys: string[]): void;
        static createSelectColumn(getMixin: () => GridRowSelectionMixin): Slick.Column;
    }

    namespace GridSelectAllButtonHelper {
        function update(grid: IDataGrid, getSelected: (p1: any) => boolean): void;
        function define(getGrid: () => IDataGrid, getId: (p1: any) => any, getSelected: (p1: any) => boolean, setSelected: (p1: any, p2: boolean) => void, text?: string, onClick?: () => void): ToolButton;
    }

    namespace GridUtils {
        function addToggleButton(toolDiv: JQuery, cssClass: string, callback: (p1: boolean) => void, hint: string, initial?: boolean): void;
        function addIncludeDeletedToggle(toolDiv: JQuery, view: Slick.RemoteView<any>, hint?: string, initial?: boolean): void;
        function addQuickSearchInput(toolDiv: JQuery, view: Slick.RemoteView<any>, fields?: QuickSearchField[]): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string) => void, fields?: QuickSearchField[]): void;
        function addQuickSearchInputCustom(container: JQuery, onSearch: (p1: string, p2: string, p3: (p1: boolean) => void) => void, fields?: QuickSearchField[]): void;
        function makeOrderable(grid: Slick.Grid, handleMove: (p1: any, p2: number) => void): void;
        function makeOrderableWithUpdateRequest(grid: DataGrid<any, any>, getId: (p1: any) => number, getDisplayOrder: (p1: any) => any, service: string, getUpdateRequest: (p1: number, p2: number) => SaveRequest<any>): void;
    }

    interface QuickSearchField {
        name: string;
        title: string;
    }

    namespace PropertyItemSlickConverter {
        function toSlickColumns(items: PropertyItem[]): Slick.Column[];
        function toSlickColumn(item: PropertyItem): Slick.Column;
    }

    namespace SlickTreeHelper {
        function filterCustom<TItem>(item: TItem, getParent: (x: TItem) => any): boolean;
        function filterById<TItem>(item: TItem, view: Slick.RemoteView<TItem>, getParentId: (x: TItem) => any): boolean;
        function setCollapsed<TItem>(items: TItem[], collapsed: boolean): void;
        function setCollapsedFlag<TItem>(item: TItem, collapsed: boolean): void;
        function setIndents<TItem>(items: TItem[], getId: (x: TItem) => any, getParentId: (x: TItem) => any, setCollapsed?: boolean): void;
        function toggleClick<TItem>(e: JQueryEventObject, row: number, cell: number, view: Slick.RemoteView<TItem>, getId: (x: TItem) => any): void;
    }

    namespace SlickFormatting {
        function getEnumText(value: any): string;
        function getEnumText(enumKey: string, name: string): string;
        function treeToggle<TItem>(getView: () => Slick.RemoteView<TItem>, getId: (x: TItem) => any,
            formatter: Slick.Format): Slick.Format;
        function date(format?: string): Slick.Format;
        function dateTime(format?: string): Slick.Format;
        function checkBox(): Slick.Format;
        function number(format: string): Slick.Format;
        function getItemType(link: JQuery): string;
        function getItemId(link: JQuery): string;
        function itemLinkText(itemType: string, id: any, text: any, extraClass: string, encode: boolean): string;
        function itemLink(itemType: string, idField: string, getText: Slick.Format,
            cssClass?: Slick.Format, encode?: boolean): Slick.Format;
    }

    namespace SlickHelper {
        function setDefaults(columns: any, localTextPrefix?: string): any;
        function convertToFormatter(format: Slick.Format): Slick.ColumnFormatter;
    }
}