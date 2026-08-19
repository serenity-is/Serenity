import { bindThis } from "@serenity-is/domwise";
import { Authorization, Fluent, PropertyItem, SelectEditorTexts, isPromiseLike, nsSerenity, setElementReadOnly } from "../../base";
import { ValidationHelper, isTrimmedEmpty } from "../../compat";
import { IEditDialog, IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../interfaces";
import { DialogType } from "../../types/dialogtype";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";
import { Combobox, ComboboxItem, ComboboxOptions, ComboboxSearchQuery, ComboboxSearchResult, stripDiacritics } from "./combobox";
import { EditorUtils } from "./editorutils";
import { EditorProps, EditorWidget } from "./editorwidget";

/**
 * Common options shared by combobox-based editors.
 */
export interface ComboboxCommonOptions {
    /** Whether the selection can be cleared. */
    allowClear?: boolean;
    /** Whether multiple items can be selected. */
    delimited?: boolean;
    /** Minimum results required to show the search box. */
    minimumResultsForSearch?: any;
    /** Whether multiple items can be selected. */
    multiple?: boolean;
}

/**
 * Options for cascading and filtering combobox editors.
 */
export interface ComboboxFilterOptions {
    /** Id of the parent editor to cascade from. */
    cascadeFrom?: string;
    /** Field used for cascading. */
    cascadeField?: string;
    /** Value used for cascading. */
    cascadeValue?: any;
    /** Field used for filtering. */
    filterField?: string;
    /** Value used for filtering. */
    filterValue?: any;
}

/**
 * Options for in-place add functionality in combobox editors.
 */
export interface ComboboxInplaceAddOptions {
    /** Whether in-place add is enabled. */
    inplaceAdd?: boolean;
    /** Permission required for in-place add. */
    inplaceAddPermission?: string;
    /** Dialog type used for in-place add. */
    dialogType?: string | DialogType | PromiseLike<DialogType>;
    /** Whether arbitrary values are allowed. */
    autoComplete?: boolean;
}

/**
 * Options for the {@link ComboboxEditor}.
 */
export interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
}

/**
 * Base editor that renders a searchable combobox over a set of items.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export class ComboboxEditor<P, TItem> extends EditorWidget<P> implements
    ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [ISetEditValue, IGetEditValue, IStringValue, IReadOnly]);

    static override createDefaultElement() { return <input type="hidden" /> as HTMLInputElement; }
    declare readonly domNode: HTMLInputElement;

    declare private combobox: Combobox;
    declare private _items: ComboboxItem<TItem>[];
    declare private _itemById: { [key: string]: ComboboxItem<TItem> };
    declare protected lastCreateTerm: string;

    /**
     * Creates a combobox editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        let hidden = this.domNode;

        this._items = [];
        this._itemById = Object.create(null);
        var emptyItemText = this.emptyItemText();
        if (emptyItemText != null) {
            hidden.setAttribute('placeholder', emptyItemText);
        }
        var comboboxOptions = this.getComboboxOptions();
        comboboxOptions.element = hidden;
        this.combobox = new Combobox(comboboxOptions);
        hidden.setAttribute('type', 'text');

        // for jquery validate to work
        Fluent.on(hidden, "change." + this.uniqueName, (e) => {
            if (!(e.target as HTMLElement)?.dataset?.comboboxsettingvalue)
                ValidationHelper.validateElement(hidden);
        });

        this.setCascadeFrom((this.options as ComboboxEditorOptions).cascadeFrom);

        if (this.useInplaceAdd())
            this.addInplaceCreate(SelectEditorTexts.InplaceAdd, null);
    }

    /**
     * Disposes the combobox and delegates to the base destroy.
     */
    override destroy() {
        this.combobox?.dispose();
        this.combobox = null;
        super.destroy();
    }

    /**
     * Whether the editor has an asynchronous item source.
     * @returns True when async.
     */
    protected hasAsyncSource(): boolean {
        return false;
    }

    /**
     * Performs an asynchronous search.
     * @param query - The search query.
     * @returns A promise resolving to the search result.
     */
    protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TItem>> {
        return Promise.resolve({
            items: [],
            more: false
        });
    }

    /**
     * Returns the type delay for searching.
     * @returns The delay in milliseconds.
     */
    protected getTypeDelay() {
        return ((this.options as any)['typeDelay'] ?? 200);
    }

    /**
     * Returns the text for the empty item.
     * @returns The empty item text.
     */
    protected emptyItemText() {
        return this.domNode.getAttribute("placeholder") ??
            SelectEditorTexts.EmptyItemText;
    }

    /**
     * Returns the page size for paged searches.
     * @returns The page size.
     */
    protected getPageSize(): number {
        return (this.options as any)['pageSize'] ?? 100;
    }

    /**
     * Returns the id field name.
     * @returns The id field.
     */
    protected getIdField() {
        return (this.options as any)['idField'];
    }

    /**
     * Returns the id of an item.
     * @param item - The item.
     * @returns The item id.
     */
    protected itemId(item: TItem): string {
        var value = (item as any)[this.getIdField()];
        if (value == null)
            return '';
        return value.toString();
    }

    /**
     * Returns the text field name.
     * @returns The text field.
     */
    protected getTextField() {
        return (this.options as any)['textField'] ?? this.getIdField();
    }

    /**
     * Returns the display text of an item.
     * @param item - The item.
     * @returns The item text.
     */
    protected itemText(item: TItem): string {
        var value = (item as any)[this.getTextField()];
        if (value == null)
            return '';
        return value.toString();
    }

    /**
     * Whether an item is disabled.
     * @param item - The item.
     * @returns True when disabled.
     */
    protected itemDisabled(item: TItem): boolean {
        return false;
    }

    /**
     * Maps an item to a combobox item.
     * @param item - The item.
     * @returns The combobox item.
     */
    protected mapItem(item: TItem): ComboboxItem {
        return {
            id: this.itemId(item),
            text: this.itemText(item),
            disabled: this.itemDisabled(item),
            source: item
        };
    }

    /**
     * Maps a list of items to combobox items.
     * @param items - The items.
     * @returns The combobox items.
     */
    protected mapItems(items: TItem[]): ComboboxItem[] {
        return items.map(bindThis(this).mapItem);
    }

    /**
     * Whether the selection can be cleared.
     * @returns True when clear is allowed.
     */
    protected allowClear() {
        return (this.options as ComboboxEditorOptions).allowClear != null ?
            !!(this.options as ComboboxEditorOptions).allowClear : this.emptyItemText() != null;
    }

    /**
     * Whether multiple items can be selected.
     * @returns True when multiple.
     */
    protected isMultiple() {
        return !!(this.options as ComboboxEditorOptions).multiple;
    }

    /**
     * Aborts any pending search query.
     */
    protected abortPendingQuery() {
        this.combobox?.abortPendingQuery();
    }

    /**
     * Returns the combobox options for this editor.
     * @returns Combobox options.
     */
    protected getComboboxOptions(): ComboboxOptions {
        var emptyItemText = this.emptyItemText();
        var opt: ComboboxOptions = {
            multiple: this.isMultiple(),
            placeholder: emptyItemText || null,
            allowClear: this.allowClear(),
            arbitraryValues: this.isAutoComplete()
        }

        if (this.hasAsyncSource()) {
            opt.search = query => this.asyncSearch(query).then(result => {
                let items = this.mapItems(result.items || []);

                if (query.initSelection) {
                    const itemById: typeof this._itemById = Object.create(null);
                    for (var x of items) {
                        itemById[x.id] = x;
                    }
                    const newItems = (query.idList || []).map(id => itemById[id]).filter(x => x != null);
                    if (items.length == newItems.length) {
                        // if length is not equal, might be a case sensitivity issue, ignore ordering otherwise
                        items = newItems;
                    }
                }

                const mappedResult = {
                    items,
                    more: result.more
                };

                if (this.isAutoComplete() && query.idList &&
                    items.length < query.idList.length) {
                    for (var v of query.idList) {
                        if (!items.some(z => z.id == v)) {
                            items.push({
                                id: v,
                                text: v
                            });
                        }
                    }
                }

                this._itemById ??= {};
                for (var x of items)
                    this._itemById[x.id] = x;

                return mappedResult;
            });
        }
        else {
            opt.search = (query) => {
                let items: ComboboxItem[];
                if (query.initSelection) {
                    items = (query.idList || []).map(id => this._itemById[id] || this._items.find(z => z.id == id)).filter(x => x != null);
                }
                else {
                    items = ComboboxEditor.filterByText(this._items, x => x.text, query.searchTerm);
                }

                if (this.isAutoComplete() && query.idList &&
                    items.length < query.idList.length) {
                    this._itemById ??= {};
                    for (var v of query.idList) {
                        if (!items.some(z => z.id == v)) {
                            var item = {
                                id: v,
                                text: v
                            };
                            items.push(item);
                            this._itemById[item.id] = item;
                        }
                    }
                }

                return {
                    items: items.slice(query.skip, query.take ? (query.skip + query.take) : items.length),
                    more: query.take && items.length > 0 && items.length > query.skip + query.take
                };
            };
        }

        if ((this.options as ComboboxEditorOptions).minimumResultsForSearch != null)
            opt.minimumResultsForSearch = (this.options as ComboboxEditorOptions).minimumResultsForSearch;

        if (this.isAutoComplete() || this.useInplaceAdd())
            opt.createSearchChoice = this.getCreateSearchChoice(null);

        return opt;
    }

    /**
     * Returns whether the value is delimited.
     * @returns True when delimited.
     */
    get_delimited() {
        return !!(this.options as ComboboxEditorOptions).delimited;
    }

    /**
     * Returns the items in the editor.
     * @returns The items.
     */
    public get items(): ComboboxItem<TItem>[] {
        if (this.hasAsyncSource())
            throw new Error("Can't read items property of an async select editor!");

        return this._items || [];
    }

    /** Sets the items in the editor. */
    public set items(value: ComboboxItem<TItem>[]) {
        if (this.hasAsyncSource())
            throw new Error("Can't set items of an async select editor!");

        this._items = value || [];
        this._itemById = Object.create(null);
        for (var item of this._items)
            this._itemById[item.id] = item;
    }

    protected get itemById(): { [key: string]: ComboboxItem<TItem> } {
        if (this.hasAsyncSource())
            throw new Error("Can't read items property of an async select editor!");

        return this._itemById;
    }

    protected set itemById(value: { [key: string]: ComboboxItem<TItem> }) {
        if (this.hasAsyncSource())
            throw new Error("Can't set itemById of an async select editor!");

        this._itemById = value || Object.create(null);
    }

    /**
     * Clears all items from the editor.
     */
    public clearItems() {
        if (this.hasAsyncSource())
            throw new Error("Can't clear items of an async select editor!");

        this._items.length = 0;
        this._itemById = Object.create(null);
    }

    /**
     * Adds an item to the editor.
     * @param item - The item to add.
     */
    public addItem(item: ComboboxItem<TItem>) {
        if (this.hasAsyncSource())
            throw new Error("Can't add item to an async select editor!");

        this._items.push(item);
        this._itemById[item.id] = item;
    }

    /**
     * Adds an option to the editor.
     * @param key - The option id.
     * @param text - The display text.
     * @param source - Optional source item.
     * @param disabled - Whether the option is disabled.
     */
    public addOption(key: string, text: string, source?: any, disabled?: boolean) {
        this.addItem({
            id: key,
            text: text,
            source: source,
            disabled: disabled
        });
    }

    /**
     * Adds the in-place create button.
     * @param addTitle - Title for the add button.
     * @param editTitle - Title for the edit button.
     */
    protected addInplaceCreate(addTitle: string, editTitle: string) {
        var self = this;
        addTitle = (addTitle ?? SelectEditorTexts.InplaceAdd);
        editTitle = (editTitle ?? SelectEditorTexts.InplaceEdit);
        const inplaceButton = (<a class="inplace-button inplace-create" title={addTitle} onClick={e => {
            self.inplaceCreateClick(e as any);
        }}><b></b></a>) as HTMLElement;
        this.domNode.after(inplaceButton);

        this.getComboboxContainer()?.classList.add("has-inplace-button");
        this.domNode.classList.add("has-inplace-button");

        this.element.on("change", () => {
            var isNew = this.isMultiple() || !this.get_value();
            inplaceButton.title = (isNew ? addTitle : editTitle);
            inplaceButton.classList.toggle('edit', !isNew);
        });

        this.element.on("change", (e: any) => {
            if ((e.target.dataset.comboboxsettingvalue))
                return;
            if (this.isMultiple()) {
                var values = this.get_values();
                if (values.length > 0 && values[values.length - 1] == (-2147483648).toString()) {
                    this.set_values(values.slice(0, values.length - 1));
                    this.inplaceCreateClick(e);
                }
            }
            else if (this.get_value() == (-2147483648).toString()) {
                this.set_value(null);
                this.inplaceCreateClick(e);
            }
        });

        if (this.isMultiple()) {
            Fluent.on(this.getComboboxContainer(), 'dblclick.' + this.uniqueName, '.select2-search-choice', (e3: Event) => {
                var q = Fluent(e3.target);
                if (!q.hasClass('select2-search-choice')) {
                    q = q.closest('.select2-search-choice');
                }
                var index = Array.from(q.parent().getNode()?.children || []).indexOf(q.getNode());
                var values1 = this.get_values();
                if (index == null || index < 0 || index >= this.get_values().length) {
                    return;
                }
                (e3 as any)['editItem'] = values1[index];
                this.inplaceCreateClick(e3);
            });
        }
    }

    /**
     * Whether in-place add is enabled.
     * @returns True when enabled.
     */
    protected useInplaceAdd(): boolean {
        return !this.isAutoComplete() &&
            (this.options as ComboboxEditorOptions).inplaceAdd &&
            ((this.options as ComboboxEditorOptions).inplaceAddPermission == null ||
                Authorization.hasPermission((this.options as ComboboxEditorOptions).inplaceAddPermission));
    }

    /**
     * Whether arbitrary values are allowed.
     * @returns True when auto-complete is enabled.
     */
    protected isAutoComplete(): boolean {
        return !!(this.options as ComboboxEditorOptions).autoComplete;
    }

    /**
     * Returns a callback that creates a search choice for a term.
     * @param getName - Optional callback to get the name of an item.
     * @returns The search choice callback.
     */
    public getCreateSearchChoice(getName: (z: any) => string) {
        return (s: string) => {

            this.lastCreateTerm = s;
            s = (stripDiacritics(s) ?? '').toLowerCase();

            if (isTrimmedEmpty(s)) {
                return null;
            }

            if ((this._items || []).some((x: ComboboxItem<TItem>) => {
                var text = getName ? getName(x.source) : x.text;
                return stripDiacritics((text ?? '')).toLowerCase() == s;
            }))
                return null;

            if (!(this._items || []).some(x1 => {
                return (stripDiacritics(x1.text) ?? '').toLowerCase().indexOf(s) !== -1;
            })) {
                if (this.isAutoComplete()) {
                    return {
                        id: this.lastCreateTerm,
                        text: this.lastCreateTerm
                    };
                }

                return {
                    id: (-2147483648).toString(),
                    text: SelectEditorTexts.NoResultsClickToDefine
                };
            }

            if (this.isAutoComplete()) {
                return {
                    id: this.lastCreateTerm,
                    text: this.lastCreateTerm
                };
            }

            return {
                id: (-2147483648).toString(),
                text: SelectEditorTexts.ClickToDefine
            };
        }
    }

    /**
     * Sets the edit value from a source object.
     * @param source - The source object.
     * @param property - The property item.
     */
    setEditValue(source: any, property: PropertyItem) {
        var val = source[property.name];
        if (Array.isArray(val)) {
            this.set_values(val);
        }
        else {
            this.set_value((val == null ? null : val.toString()));
        }
    }

    /**
     * Gets the edit value into a target object.
     * @param property - The property item.
     * @param target - The target object.
     */
    getEditValue(property: PropertyItem, target: any) {
        if (!this.isMultiple() || this.get_delimited()) {
            target[property.name] = this.get_value();
        }
        else {
            target[property.name] = this.get_values();
        }
    }

    /**
     * Returns the combobox container element.
     * @returns The container element.
     */
    protected getComboboxContainer(): HTMLElement {
        return Combobox.getInstance(this.domNode)?.container;
    }

    /**
     * Returns the items in the editor.
     * @returns The items.
     */
    protected get_items() {
        return this.items;
    }

    /**
     * Returns the item-by-id map.
     * @returns The item map.
     */
    protected get_itemByKey() {
        return this.itemById;
    }

    /**
     * Filters items by text, matching the term against the item text.
     * @param items - The items to filter.
     * @param getText - Callback that returns the text of an item.
     * @param term - The search term.
     * @returns The filtered items.
     */
    public static filterByText<TItem>(items: TItem[], getText: (item: TItem) => string, term: string): TItem[] {
        if (term == null || term.length == 0)
            return items;

        term = stripDiacritics(term).toUpperCase();

        var contains: TItem[] = [];
        function filter(item: TItem): boolean {
            var text = getText(item);
            if (text == null || !text.length)
                return false;
            text = stripDiacritics(text).toUpperCase();
            if (text.startsWith(term))
                return true;
            if (text.indexOf(term) >= 0)
                contains.push(item);
            return false;
        }

        return items.filter(filter).concat(contains);
    }

    /**
     * Returns the current value.
     * @returns The value.
     */
    get_value() {
        return this.combobox ? this.combobox.getValue() : this.domNode?.value;
    }

    /**
     * Returns the current value.
     * @returns The value.
     */
    get value(): string {
        return this.get_value();
    }

    /**
     * Sets the current value.
     * @param value - The value to set.
     */
    set_value(value: string) {

        if (this.combobox) {
            this.combobox.setValue(value, /*triggerChange*/ true);
        } else if (this.domNode) {
            this.domNode.value = value;
        }

        this.updateInplaceReadOnly();
    }

    /** Sets the current value. */
    set value(v: string) {
        this.set_value(v);
    }

    /**
     * Returns the currently selected item.
     * @returns The selected item, or null.
     */
    get selectedItem(): TItem {
        let selectedValue = this.get_value();
        if (selectedValue && this._itemById) {
            let item = this._itemById[selectedValue];
            if (item)
                return item.source;
        }
        return null;
    }

    /**
     * Returns the currently selected items.
     * @returns The selected items.
     */
    get selectedItems(): TItem[] {
        let selectedValues = this.values;
        var result = [];
        for (var value of selectedValues) {
            if (value && this._itemById) {
                let item = this._itemById[value];
                if (item && item.source)
                    result.push(item.source);
                else
                    result.push(null);
            }
        }
        return result;
    }

    /**
     * Returns the current values.
     * @returns The values.
     */
    protected get_values(): string[] {
        return this.combobox?.getValues();
    }

    /**
     * Returns the current values.
     * @returns The values.
     */
    get values(): string[] {
        return this.get_values();
    }

    /**
     * Sets the current values.
     * @param value - The values to set.
     */
    protected set_values(value: string[]) {
        this.combobox?.setValues(value);
    }

    /** Sets the current values. */
    set values(value: string[]) {
        this.set_values(value);
    }

    /**
     * Returns the display text of the current selection.
     * @returns The text.
     */
    protected get_text(): string {
        var combobox = Combobox.getInstance(this.domNode);
        if (combobox)
            return combobox.getSelectedItems()?.map(x => x.text).join(", ");

        return this.domNode.value;
    }

    /**
     * Returns the display text of the current selection.
     * @returns The text.
     */
    get text(): string {
        return this.get_text();
    }

    /**
     * Returns whether the editor is read-only.
     * @returns True when read-only.
     */
    get_readOnly(): boolean {
        return this.domNode.getAttribute("readonly") != null;
    }

    private updateInplaceReadOnly(): void {
        var readOnly = this.get_readOnly() &&
            (this.isMultiple() || !this.value);
        let el = this.element.nextSibling(".inplace-create").getNode();
        if (el) {
            el.setAttribute('disabled', (readOnly ? 'disabled' : ''));
            el.style.opacity = (readOnly ? '0.1' : '');
            el.style.cursor = (readOnly ? 'default' : '');
        }
    }

    /**
     * Sets whether the editor is read-only.
     * @param value - True to enable read-only mode.
     */
    set_readOnly(value: boolean) {
        if (value !== this.get_readOnly()) {
            setElementReadOnly(this.domNode, value);
            this.updateInplaceReadOnly();
        }
    }

    /**
     * Returns the cascade value from a parent widget.
     * @param parent - The parent widget.
     * @returns The cascade value.
     */
    protected getCascadeFromValue(parent: Widget<any>) {
        return EditorUtils.getValue(parent);
    }

    declare protected cascadeLink: CascadedWidgetLink<Widget<any>>;

    /**
     * Sets the cascade-from parent id.
     * @param value - The parent id.
     */
    protected setCascadeFrom(value: string) {

        if (!value) {
            if (this.cascadeLink != null) {
                this.cascadeLink.set_parentID(null);
                this.cascadeLink = null;
            }
            (this.options as ComboboxEditorOptions).cascadeFrom = null;
            return;
        }

        this.cascadeLink = new CascadedWidgetLink<Widget<any>>(Widget, this, p => {
            this.set_cascadeValue(this.getCascadeFromValue(p));
        });

        this.cascadeLink.set_parentID(value);
        (this.options as ComboboxEditorOptions).cascadeFrom = value;
    }

    /**
     * Returns the cascade-from parent id.
     * @returns The parent id.
     */
    protected get_cascadeFrom(): string {
        return (this.options as ComboboxEditorOptions).cascadeFrom;
    }

    /**
     * Returns the cascade-from parent id.
     * @returns The parent id.
     */
    get cascadeFrom(): string {
        return this.get_cascadeFrom();
    }

    /**
     * Sets the cascade-from parent id.
     * @param value - The parent id.
     */
    protected set_cascadeFrom(value: string) {
        if (value !== (this.options as ComboboxEditorOptions).cascadeFrom) {
            this.setCascadeFrom(value);
            this.updateItems();
        }
    }

    /** Sets the cascade-from parent id. */
    set cascadeFrom(value: string) {
        this.set_cascadeFrom(value);
    }

    /**
     * Returns the cascade field name.
     * @returns The cascade field.
     */
    protected get_cascadeField() {
        return ((this.options as ComboboxEditorOptions).cascadeField ?? (this.options as ComboboxEditorOptions).cascadeFrom);
    }

    /**
     * Returns the cascade field name.
     * @returns The cascade field.
     */
    get cascadeField(): string {
        return this.get_cascadeField();
    }

    /**
     * Sets the cascade field name.
     * @param value - The cascade field.
     */
    protected set_cascadeField(value: string) {
        (this.options as ComboboxEditorOptions).cascadeField = value;
    }

    /** Sets the cascade field name. */
    set cascadeField(value: string) {
        this.set_cascadeField(value);
    }

    /**
     * Returns the cascade value.
     * @returns The cascade value.
     */
    protected get_cascadeValue(): any {
        return (this.options as ComboboxEditorOptions).cascadeValue;
    }

    /**
     * Returns the cascade value.
     * @returns The cascade value.
     */
    get cascadeValue(): any {
        return this.get_cascadeValue();
    }

    /**
     * Sets the cascade value and refreshes items.
     * @param value - The cascade value.
     */
    protected set_cascadeValue(value: any) {
        if ((this.options as ComboboxEditorOptions).cascadeValue !== value) {
            (this.options as ComboboxEditorOptions).cascadeValue = value;
            this.set_value(null);
            this.updateItems();
        }
    }

    /** Sets the cascade value. */
    set cascadeValue(value: any) {
        this.set_cascadeValue(value);
    }

    /**
     * Returns the filter field name.
     * @returns The filter field.
     */
    protected get_filterField() {
        return (this.options as ComboboxEditorOptions).filterField;
    }

    /**
     * Returns the filter field name.
     * @returns The filter field.
     */
    get filterField(): string {
        return this.get_filterField();
    }

    /**
     * Sets the filter field name.
     * @param value - The filter field.
     */
    protected set_filterField(value: string) {
        (this.options as ComboboxEditorOptions).filterField = value;
    }

    /** Sets the filter field name. */
    set filterField(value: string) {
        this.set_filterField(value);
    }

    /**
     * Returns the filter value.
     * @returns The filter value.
     */
    protected get_filterValue(): any {
        return (this.options as ComboboxEditorOptions).filterValue;
    }

    /**
     * Returns the filter value.
     * @returns The filter value.
     */
    get filterValue(): any {
        return this.get_filterValue();
    }

    /**
     * Sets the filter value and refreshes items.
     * @param value - The filter value.
     */
    protected set_filterValue(value: any) {
        if ((this.options as ComboboxEditorOptions).filterValue !== value) {
            (this.options as ComboboxEditorOptions).filterValue = value;
            this.set_value(null);
            this.updateItems();
        }
    }

    /** Sets the filter value. */
    set filterValue(value: any) {
        this.set_filterValue(value);
    }

    /**
     * Filters items by the cascade value.
     * @param items - The items to filter.
     * @returns The filtered items.
     */
    protected cascadeItems(items: TItem[]) {

        var val = this.get_cascadeValue();

        if (val == null || val === '') {

            if (this.get_cascadeField()) {
                return [];
            }

            return items;
        }

        var key = val.toString();
        var fld = this.get_cascadeField();

        return items.filter(x => {
            var itemKey = (x as any)[fld];
            return !!(itemKey != null && itemKey.toString() === key);
        });
    }

    /**
     * Filters items by the filter value.
     * @param items - The items to filter.
     * @returns The filtered items.
     */
    protected filterItems(items: TItem[]) {
        var val = this.get_filterValue();

        if (val == null || val === '') {
            return items;
        }

        var key = val.toString();
        var fld = this.get_filterField();

        return items.filter(x => {
            var itemKey = (x as any)[fld];
            return !!(itemKey != null && itemKey.toString() === key);
        });
    }

    /**
     * Refreshes the items in the editor.
     */
    protected updateItems() {
    }

    /**
     * Returns the dialog type used for in-place add.
     * @returns The dialog type.
     */
    protected getDialogType(): DialogType | PromiseLike<DialogType> {
        const opt = (this.options as ComboboxEditorOptions);
        if (opt?.dialogType && typeof opt.dialogType !== "string")
            return opt.dialogType as DialogType;

        const dialogTypeKey = (this as any).getDialogTypeKey();
        if (dialogTypeKey)
            return DialogTypeRegistry.getOrLoad(dialogTypeKey);

        return null;
    }

    /** @deprecated Override getDialogType() instead */
    protected getDialogTypeKey(): string {
        if (typeof (this.options as ComboboxEditorOptions).dialogType === "string") {
            return (this.options as ComboboxEditorOptions).dialogType as string;
        }

        return null;
    }

    /**
     * Creates an edit dialog for in-place add.
     * @param callback - Callback invoked with the created dialog.
     */
    protected createEditDialog(callback: (dlg: IEditDialog) => void): void {
        const dialogType = this.getDialogType();
        const then = (dialogType: DialogType) => {
            var dialog = new dialogType({}).init?.();
            callback?.(dialog as unknown as IEditDialog);
        }
        isPromiseLike(dialogType) ? dialogType.then(then) : then(dialogType);
    }

    /** Callback invoked to initialize a new entity for in-place add. */
    declare public onInitNewEntity: (entity: TItem) => void;

    /**
     * Initializes a new entity with cascade/filter values.
     * @param entity - The new entity.
     */
    protected initNewEntity(entity: TItem) {
        if (this.get_cascadeField()) {
            (entity as any)[this.get_cascadeField()] = this.get_cascadeValue();
        }

        if (this.get_filterField()) {
            (entity as any)[this.get_filterField()] = this.get_filterValue();
        }

        if (this.onInitNewEntity != null) {
            this.onInitNewEntity(entity);
        }
    }

    /**
     * Sets the edit dialog to read-only.
     * @param dialog - The dialog.
     */
    protected setEditDialogReadOnly(dialog: any): void {
        // an ugly workaround
        dialog.element &&
            (dialog.element as Fluent).findFirst &&
            (dialog.element as Fluent).findFirst('.tool-button.delete-button')
                .addClass('disabled')
                .off('click', void 0);
    }

    /**
     * Hook invoked when the edit dialog data changes.
     */
    protected editDialogDataChange() {
    }

    /**
     * Sets the search term on a new entity.
     * @param entity - The new entity.
     * @param term - The search term.
     * @param dialog - The edit dialog.
     */
    protected setTermOnNewEntity(entity: TItem, term: string, dialog: any) {
        if (term && typeof dialog?.getNameProperty === "function") {
            const nameProperty = dialog.getNameProperty() as string;
            if (nameProperty) {
                (entity as any)[nameProperty] = term;
            }
        }
    }

    /**
     * Handles the in-place create button click.
     * @param e - The click event.
     */
    protected inplaceCreateClick(e: Event) {

        if (this.get_readOnly() &&
            ((this.isMultiple() && !(e as any)['editItem']) || !this.value))
            return;

        this.createEditDialog(dialog => {

            if (this.get_readOnly())
                this.setEditDialogReadOnly(dialog);

            SubDialogHelper.bindToDataChange(dialog, this, (dci) => {
                this.editDialogDataChange();
                this.updateItems();
                this.lastCreateTerm = null;

                if ((dci.operationType === 'create' || dci.operationType === 'update') &&
                    dci.entityId != null) {
                    var id = dci.entityId.toString();

                    if (this.isMultiple()) {
                        var values = this.get_values().slice();
                        if (values.indexOf(id) < 0) {
                            values.push(id);
                        }
                        this.set_values(null);
                        this.set_values(values.slice());
                    }
                    else {
                        this.set_value(null);
                        this.set_value(id);
                    }
                }
                else if (this.isMultiple() && dci.operationType === 'delete' &&
                    dci.entityId != null) {
                    var id1 = dci.entityId.toString();
                    var values1 = this.get_values().slice();

                    var idx1 = values1.indexOf(id1);
                    if (idx1 >= 0)
                        values1.splice(idx1, 1);

                    this.set_values(values1.slice());
                }
                else if (!this.isMultiple()) {
                    this.set_value(null);
                }
            }, true);

            var editItem = (e as any)['editItem'];
            if (editItem != null) {
                dialog.load(editItem, () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                });
            }
            else if (this.isMultiple() || !this.get_value()) {
                var entity: TItem = {} as any;
                this.setTermOnNewEntity(entity, this.lastCreateTerm?.trim() ?? '', dialog);
                this.initNewEntity(entity);
                dialog.load(entity, () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                });
            }
            else {
                dialog.load(this.get_value(), () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                });
            }
        });
    }

    public openDropdown() {
        Combobox.getInstance(this.domNode)?.openDropdown();
    }

    declare public openDialogAsPanel: boolean;
}

