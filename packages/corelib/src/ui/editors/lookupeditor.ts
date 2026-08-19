import { bindThis } from "@serenity-is/domwise";
import { getInstanceType, getLookupAsync, getTypeFullName, nsSerenity, type Lookup } from "../../base";
import { ScriptData, getLookup, reloadLookup } from "../../compat";
import { ComboboxItem, ComboboxSearchQuery, ComboboxSearchResult } from "./combobox";
import { ComboboxEditor, ComboboxEditorOptions } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

/**
 * Options for the {@link LookupEditor}.
 */
export interface LookupEditorOptions extends ComboboxEditorOptions {
    /** Key of the lookup to load items from. */
    lookupKey?: string;
    /** Whether items are loaded asynchronously. */
    async?: boolean;
}

/**
 * Base editor that renders a combobox over lookup items.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export abstract class LookupEditorBase<P extends LookupEditorOptions, TItem> extends ComboboxEditor<P, TItem> {

    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    declare private lookupChangeOff: any;

    /**
     * Creates a lookup editor base.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        if (!this.hasAsyncSource()) {
            this.updateItems();
            this.lookupChangeOff = ScriptData.bindToChange('Lookup.' + this.getLookupKey(), bindThis(this).updateItems);
        }
    }

    /**
     * Whether the editor has an asynchronous item source.
     * @returns True when async.
     */
    override hasAsyncSource(): boolean {
        return !!this.options.async;
    }

    /**
     * Cleans up the lookup change subscription.
     */
    override destroy(): void {
        if (this.lookupChangeOff) {
            this.lookupChangeOff();
            this.lookupChangeOff = null;
        }

        super.destroy();
    }

    /**
     * Returns the lookup key for this editor.
     * @returns The lookup key.
     */
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

    declare protected lookup: Lookup<TItem>;

    /**
     * Asynchronously loads the lookup.
     * @returns A promise resolving to the lookup.
     */
    protected getLookupAsync(): PromiseLike<Lookup<TItem>> {
        return getLookupAsync<TItem>(this.getLookupKey());
    }

    /**
     * Returns the lookup synchronously.
     * @returns The lookup.
     */
    protected getLookup(): Lookup<TItem> {
        return getLookup<TItem>(this.getLookupKey());
    }

    /**
     * Returns the items for the given lookup, filtered by cascade/filter values.
     * @param lookup - The lookup.
     * @returns The items.
     */
    protected getItems(lookup: Lookup<TItem>) {
        return this.filterItems(this.cascadeItems(lookup.items));
    }

    /**
     * Returns the id field name.
     * @returns The id field.
     */
    protected override getIdField() {
        return this.lookup != null ? this.lookup.idField : super.getIdField();
    }

    /**
     * Returns the display text of an item.
     * @param item - The item.
     * @param lookup - The lookup.
     * @returns The item text.
     */
    protected getItemText(item: TItem, lookup: Lookup<TItem>) {
        if (lookup == null)
            return super.itemText(item);

        var textValue = (item as any)[lookup.textField];
        return textValue == null ? '' : textValue.toString();
    }

    /**
     * Maps an item to a combobox item.
     * @param item - The item.
     * @returns The combobox item.
     */
    protected override mapItem(item: TItem): ComboboxItem<TItem> {
        return {
            id: this.itemId(item),
            text: this.getItemText(item, this.lookup),
            disabled: this.getItemDisabled(item, this.lookup),
            source: item
        };
    }

    /**
     * Whether an item is disabled.
     * @param item - The item.
     * @param lookup - The lookup.
     * @returns True when disabled.
     */
    protected getItemDisabled(item: TItem, lookup: Lookup<TItem>) {
        return super.itemDisabled(item);
    }

    /**
     * Loads the lookup items into the editor.
     */
    public override updateItems() {
        if (this.hasAsyncSource())
            return;

        this.clearItems();
        this.lookup = this.getLookup();
        var items = this.getItems(this.lookup);
        for (var item of items)
            this.addItem(this.mapItem(item));
    }

    /**
     * Performs an asynchronous search over the lookup items.
     * @param query - The search query.
     * @returns A promise resolving to the search result.
     */
    protected override async asyncSearch(query: ComboboxSearchQuery): Promise<ComboboxSearchResult<TItem>> {
        this.lookup = await this.getLookupAsync();
        var items = this.getItems(this.lookup);

        if (query.idList != null) {
            items = items.filter(x => query.idList.indexOf(this.itemId(x)) >= 0);
        }

        const getText = (item: TItem) => this.getItemText(item, this.lookup);

        items = ComboboxEditor.filterByText(items, getText, query.searchTerm);

        return {
            items: items.slice(query.skip, query.take ? (query.skip + query.take) : items.length),
            more: query.take && items.length > 0 && items.length > query.skip + query.take
        };
    }

    /**
     * Returns the dialog type key for in-place add.
     * @returns The dialog type key.
     */
    protected override getDialogTypeKey() {
        var dialogTypeKey = super.getDialogTypeKey();
        if (dialogTypeKey)
            return dialogTypeKey;

        return this.getLookupKey();
    }

    /**
     * Sets the search term on a new entity.
     * @param entity - The new entity.
     * @param term - The search term.
     */
    protected setCreateTermOnNewEntity(entity: TItem, term: string) {
        (entity as any)[this.getLookup().textField] = term;
    }

    /**
     * Reloads the lookup when the edit dialog data changes.
     */
    protected override editDialogDataChange() {
        reloadLookup(this.getLookupKey());
    }
}

/**
 * An editor that renders a combobox over lookup items.
 * @typeParam P - Widget props type.
 */
export class LookupEditor<P extends LookupEditorOptions = LookupEditorOptions> extends LookupEditorBase<P, {}> {

    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates a lookup editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);
    }
}