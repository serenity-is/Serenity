import { getInstanceType, getLookupAsync, getTypeFullName, type Lookup } from "@serenity-is/base";
import { Decorators } from "../../types/decorators";
import { getLookup, reloadLookup, ScriptData } from "../../q";
import { EditorProps } from "../widgets/widget";
import { Select2Editor, Select2EditorOptions, Select2SearchPromise, Select2SearchQuery, Select2SearchResult } from "./select2editor";

export interface LookupEditorOptions extends Select2EditorOptions {
    lookupKey?: string;
    async?: boolean;
}

@Decorators.registerEditor("Serenity.LookupEditorBase")
export abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends Select2Editor<P, TItem> {

    private lookupChangeUnbind: any; 

    constructor(props: EditorProps<P>) {
        super(props);

        if (!this.hasAsyncSource()) {
            this.updateItems();
            this.lookupChangeUnbind = ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.updateItems.bind(this));
        }
    }

    hasAsyncSource(): boolean {
        return !!this.options.async;
    }

    destroy(): void {
        if (this.lookupChangeUnbind) {
            this.lookupChangeUnbind();
            this.lookupChangeUnbind = null;
        }

        super.destroy();
    }

    protected getLookupKey(): string {
        if (this.options.lookupKey != null) {
            return this.options.lookupKey;
        }

        var key = getTypeFullName(getInstanceType(this));

        var idx = key.indexOf('.');
        if (idx >= 0) {
            key = key.substring(idx + 1);
        }

        if (key.endsWith('Editor')) {
            key = key.substring(0, key.length - 6);
        }

        return key;
    }

    protected lookup: Lookup<TItem>;

    protected getLookupAsync(): PromiseLike<Lookup<TItem>> {
        return getLookupAsync<TItem>(this.getLookupKey());
    }

    protected getLookup(): Lookup<TItem> {
        return getLookup<TItem>(this.getLookupKey());
    }

    protected getItems(lookup: Lookup<TItem>) {
        return this.filterItems(this.cascadeItems(lookup.items));
    }

    protected getIdField() {
        return this.lookup != null ? this.lookup.idField : super.getIdField();
    }

    protected getItemText(item: TItem, lookup: Lookup<TItem>) {
        if (lookup == null)
            return super.itemText(item);

        var textValue = (item as any)[lookup.textField];
        return textValue == null ? '' : textValue.toString();
    }

    protected mapItem(item: TItem): Select2Item {
        return {
            id: this.itemId(item),
            text: this.getItemText(item, this.lookup),
            disabled: this.getItemDisabled(item, this.lookup),
            source: item
        };
    }

    protected getItemDisabled(item: TItem, lookup: Lookup<TItem>) {
        return super.itemDisabled(item);
    }

    public updateItems() {
        if (this.hasAsyncSource())
            return;

        this.clearItems();
        this.lookup = this.getLookup();
        var items = this.getItems(this.lookup);
        for (var item of items)
            this.addItem(this.mapItem(item));
    }

    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise {
        return this.getLookupAsync().then(lookup => {
            this.lookup = lookup;

            var items = this.getItems(this.lookup);

            if (query.idList != null) {
                items = items.filter(x => query.idList.indexOf(this.itemId(x)) >= 0);
            }

            function getText(item: TItem) {
                return this.getItemText(item, this.lookup);
            }

            items = Select2Editor.filterByText(items, getText.bind(this), query.searchTerm);

            results({
                items: items.slice(query.skip, query.take),
                more: items.length >= query.take
            });
        }) as any;
    }

    protected getDialogTypeKey() {
        var dialogTypeKey = super.getDialogTypeKey();
        if (dialogTypeKey)
            return dialogTypeKey;

        return this.getLookupKey();
    }

    protected setCreateTermOnNewEntity(entity: TItem, term: string) {
        (entity as any)[this.getLookup().textField] = term;
    }

    protected editDialogDataChange() {
        reloadLookup(this.getLookupKey());
    }
}

@Decorators.registerEditor("Serenity.LookupEditor")
export class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {
    
    constructor(props: EditorProps<P>) {
        super(props);
    }
}