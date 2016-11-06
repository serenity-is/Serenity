declare namespace Serenity {

    class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {
        constructor(input: JQuery, opt?: TOptions);
        protected cascadeItems(items: TItem[]): TItem[];
        protected filterItems(items: TItem[]): TItem[];
        protected getCasecadeFromValue(parent: Widget<any>): any;
        protected getItems(lookup: Q.Lookup<TItem>): TItem[];
        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>): string;
        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>): boolean;
        protected getLookup(): Q.Lookup<TItem>;
        protected getLookupKey(): string;
        protected initNewEntity(entity: TItem): void;
        protected updateItems(): void;
        protected getDialogTypeKey(): string;
        protected createEditDialog(callback: (dlg: Serenity.IEditDialog) => void): void;
        onInitNewEntity: (entity: TItem) => void;
        value: string;
        cascadeField: string;
        cascadeFrom: string;
        cascadeValue: any;
        filterField: string;
        filterValue: any;
    }

    interface LookupEditorOptions {
        lookupKey?: string;
        minimumResultsForSearch?: any;
        inplaceAdd?: boolean;
        dialogType?: string;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
        multiple?: boolean;
        delimited?: boolean;
    }

    class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(input: JQuery, opt?: LookupEditorOptions);
    }
}