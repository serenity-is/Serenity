declare namespace Serenity {

    class FilterPanel extends FilterWidgetBase<any> {
        static panelTemplate: string;
        static rowTemplate: string;
        constructor(div: JQuery);
        updateRowsFromStore(): void;
        search(): void;
        get_showInitialLine(): boolean;
        set_showInitialLine(value: boolean): void;
        get_showSearchButton(): boolean;
        set_showSearchButton(value: boolean): void;
        get_updateStoreOnReset(): boolean;
        set_updateStoreOnReset(value: boolean): void;
        get_hasErrors(): boolean;
    }

}