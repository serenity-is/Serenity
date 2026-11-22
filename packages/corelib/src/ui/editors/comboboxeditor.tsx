import { bindThis } from "@serenity-is/domwise";
import { Authorization, Fluent, IconClassName, PropertyItem, SelectEditorTexts, iconClassName, isPromiseLike, localText, nsSerenity, setElementReadOnly, stringFormat } from "../../base";
import { ValidationHelper } from "../../compat";
import { IEditDialog, IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../interfaces";
import { DialogType } from "../../types/dialogtype";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";
import { Combobox, ComboboxItem, ComboboxOptions, ComboboxSearchQuery, ComboboxSearchResult, stripDiacritics } from "./combobox";
import { EditorUtils } from "./editorutils";
import { EditorProps, EditorWidget } from "./editorwidget";
import { Select2, type Select2QueryOptions } from "./select2";

/**
 * Common options shared by combobox-based editors.
 */
export interface ComboboxCommonOptions {
    /** Whether the selection can be cleared. */
    allowClear?: boolean;
    /** Whether multiple items can be selected. */
    delimited?: boolean;
    /**
     * Minimum results required to show the search box. 0 shows it for any number of results, and a negative value,
     * e.g. -1, hides it permanently, see `AlwaysShowSearch` and `NeverShowSearch` of the attributes. When it is not set,
     * it is {@link ComboboxEditorDefaults.minimumResultsForSearch}.
     */
    minimumResultsForSearch?: number;
    /** Whether multiple items can be selected. */
    multiple?: boolean;
    /** Name of the item field containing the icon class. */
    iconField?: string;
    /**
     * Shows a multiple editor as a checklist, instead of a combobox with a choice for each selected item: the results
     * have check boxes, of which the ones that are selected stay in the list, the first item selects all the results
     * that are shown, or clears them, and the selection is shown as one summary, e.g. "A, B, C +2". It has no effect on a
     * single editor.
     */
    checkList?: boolean;
    /**
     * The icon of the placeholder, e.g. the icon of the field or of the filter, which the editor shows when nothing is
     * selected.
     */
    placeholderIcon?: IconClassName;
    /**
     * The field of an item, e.g. the column of a lookup, that holds its local text key, which the editor localizes, or
     * "text" when the text of an item is its key. When it is not set, the text of an item is shown as it is, which is
     * what a lookup or a list service does by default, as their texts are data, and may look like keys.
     */
    textKeyField?: string;
    /**
     * The field of an item, e.g. the column of a lookup, that holds its CSS class, e.g. "text-danger", which the editor
     * adds to the item in its results, as a class of the item itself does.
     */
    cssClassField?: string;
    /**
     * Whether a checklist, see {@link checkList}, has an item that selects all the results that are shown, or clears
     * them when they are all selected. Its text tells which: "Select all", "Select all matching" when something is
     * typed, or "Select shown" when there are more results than the ones that are loaded, e.g. of an asynchronous
     * source, whose results come in pages. Default is true.
     */
    selectAll?: boolean;
    /**
     * How many of the selected items the summary of a checklist, see {@link checkList}, names before it counts the
     * rest, e.g. 3 for "A, B, C +2". Zero names none of them, and shows how many are selected, e.g. "4 selected", and
     * -1 names them all, and shows how many are selected at the end, e.g. "A, B, C, D (4)", of which the styles cut what
     * does not fit, see `SummaryCountOnly` and `SummaryNamesAll` of the attributes. The other negative values are
     * reserved for summaries to come, which name them all until then. When it is not set, or null, it is
     * {@link ComboboxEditorDefaults.maxSummaryItems}, which is -1.
     */
    maxSummaryItems?: number;
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
    /** Whether the in-place add dialog opens as a panel. */
    openDialogAsPanel?: boolean;
}

/**
 * Options for the {@link ComboboxEditor}.
 */
export interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
}

/**
 * The defaults of the combobox editors, see {@link ComboboxEditor.defaults}, which an application can change once, e.g.
 * in its script initialization, before the editors are created. An option of an editor takes precedence over them.
 */
export interface ComboboxEditorDefaults {
    /**
     * The text of the empty item, which is the placeholder of an editor that has nothing selected, when the editor has
     * no placeholder of its own. Default is an empty string, which shows no text, but still lets the selection be
     * cleared, while `null` shows no placeholder, and does not let it be cleared. A callback is called each time it is
     * needed, so it can return a local text that is not loaded yet when the default is set, e.g. to show "--select--"
     * as before: `ComboboxEditor.defaults.emptyItemText = () => SelectEditorTexts.EmptyItemText`.
     */
    emptyItemText: string | (() => string);
    /** The default of {@link ComboboxCommonOptions.minimumResultsForSearch}. Not set by default, so the select shows a search box for any number of results. */
    minimumResultsForSearch?: number;
    /**
     * The default of {@link ComboboxCommonOptions.maxSummaryItems}. Default is -1, which names them all, with the count
     * of the selection after them.
     */
    maxSummaryItems: number;
}

/**
 * Base editor that renders a searchable combobox over a set of items.
 * @typeParam P - Widget props type.
 * @typeParam TItem - The item type.
 */
export class ComboboxEditor<P, TItem> extends EditorWidget<P> implements
    ISetEditValue, IGetEditValue, IStringValue, IReadOnly {
    static override[Symbol.typeInfo] = this.registerClass(nsSerenity, [ISetEditValue, IGetEditValue, IStringValue, IReadOnly]);

    /** Creates the default hidden input element for the combobox.
     * @returns The hidden input element. */
    static override createDefaultElement() { return <input type="hidden" /> as HTMLInputElement; }
    /** The hidden input element that backs the combobox editor. */
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

        const hidden = this.domNode;

        this._items = [];
        this._itemById = Object.create(null);
        const emptyItemText = this.emptyItemText();
        if (emptyItemText != null) {
            hidden.setAttribute('placeholder', emptyItemText);
        }
        const comboboxOptions = this.getComboboxOptions();
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
     * Disposes the combobox, unbinds the cascade link and delegates to the base destroy.
     */
    override destroy() {
        this.combobox?.dispose();
        this.combobox = null;
        // the link otherwise relies on a disposing listener of this element, which the base destroy
        // removes along with the other namespaced handlers, leaving a change handler on the parent
        this.cascadeLink?.set_parentID(null);
        this.cascadeLink = null;
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
     * Returns the current state of the initial selection load.
     * "idle" means there is no pending or failed load. It does not guarantee
     * that an item is currently selected.
     */
    get selectionLoadState(): "idle" | "loading" | "error" {
        return this.combobox?.selectionLoadState ?? "idle";
    }

    /**
     * Waits for the current initial selection load, if necessary.
     * @returns A promise that resolves when the selection is loaded or rejects
     * if loading fails or is aborted. It resolves immediately when there is no
     * pending load.
     */
    ensureSelectionLoaded(): Promise<void> {
        return this.combobox?.ensureSelectionLoaded() ?? Promise.resolve();
    }

    /**
     * Waits for the current initial selection load and returns the selected item.
     * @returns The selected item, or null when nothing is selected.
     */
    async getSelectedItemAsync(): Promise<TItem> {
        await this.ensureSelectionLoaded();
        return this.selectedItem;
    }

    /**
     * Waits for the current initial selection load and returns the selected items.
     * @returns The selected items.
     */
    async getSelectedItemsAsync(): Promise<TItem[]> {
        await this.ensureSelectionLoaded();
        return this.selectedItems;
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
        const text = ComboboxEditor.defaults.emptyItemText;
        return this.domNode.getAttribute("placeholder") ?? (typeof text === "function" ? text() : text);
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
        const value = (item as any)[this.getIdField()];
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
        const value = (item as any)[this.getTextField()];
        // an item may have no text of its own, and only a key, which localizeItemText gives the text of
        return this.localizeItemText(value == null ? null : value.toString(), item);
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
     * Gets the name of the icon field.
     * @returns The name of the icon field.
     */
    protected getIconField() {
        return (this.options as any)['iconField'];
    }

    /**
     * Returns the icon of an item.
     * @param item - The item.
     * @returns The item icon.
     */
    protected itemIcon(item: TItem): IconClassName {
        const value = (item as any)[this.getIconField()];
        if (value != null)
            return value.toString() as IconClassName;
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
            icon: this.itemIcon(item),
            cssClass: this.itemCssClass(item),
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
        const options = this.options as ComboboxEditorOptions;
        // a checklist is a shape of a multiple editor, and a delimited value is one of more than one item, so both imply
        // it, as neither means anything on its own
        return !!options.multiple || !!options.checkList || !!options.delimited;
    }

    /**
     * Aborts any pending search query.
     */
    protected abortPendingQuery() {
        this.combobox?.abortPendingQuery();
    }

    /** The defaults of the combobox editors, see {@link ComboboxEditorDefaults}. */
    public static readonly defaults: ComboboxEditorDefaults = {
        emptyItemText: "",
        maxSummaryItems: -1
    };

    /**
     * Returns the combobox options for this editor.
     * @returns Combobox options.
     */
    protected getComboboxOptions(): ComboboxOptions {
        const emptyItemText = this.emptyItemText();
        const opt: ComboboxOptions = {
            multiple: this.isMultiple(),
            placeholder: emptyItemText || null,
            allowClear: this.allowClear(),
            arbitraryValues: this.isAutoComplete(),
            // the size of the pages that a source, e.g. a list service, is asked for, see getPageSize
            pageSize: this.getPageSize()
        }

        if (this.hasAsyncSource()) {
            opt.search = query => this.asyncSearch(query).then(result => {
                let items = this.mapItems(result.items || []);

                if (query.initSelection) {
                    const itemById: typeof this._itemById = Object.create(null);
                    for (const x of items) {
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
                    for (const v of query.idList) {
                        if (!items.some(z => z.id == v)) {
                            items.push({
                                id: v,
                                text: v
                            });
                        }
                    }
                }

                this._itemById ??= {};
                for (const x of items)
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
                    for (const v of query.idList) {
                        if (!items.some(z => z.id == v)) {
                            const item = {
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

        // the class of an item, e.g. text-danger, is a class of the result itself, which the styles of a select show,
        // along with the css of an item, which is what select2 reads by default
        opt.formatResultCssClass ??= item => [item?.cssClass, (item as any)?.css].filter(x => x).join(" ") || undefined;

        const minSearch = (this.options as ComboboxEditorOptions).minimumResultsForSearch ?? ComboboxEditor.defaults.minimumResultsForSearch;
        if (minSearch != null)
            opt.minimumResultsForSearch = minSearch;

        if (this.isAutoComplete() || this.useInplaceAdd())
            opt.createSearchChoice = this.getCreateSearchChoice(null);

        this.applyCheckList(opt);

        return opt;
    }

    /** The options of the editor that the comboboxes share, see {@link ComboboxCommonOptions}. */
    protected get comboboxOptions(): ComboboxCommonOptions {
        return this.options as ComboboxCommonOptions;
    }

    /** Whether the editor shows a multiple selection as a checklist, see {@link ComboboxCommonOptions.checkList}. */
    protected useCheckList() {
        return !!this.comboboxOptions.checkList && this.isMultiple();
    }

    /**
     * Gives a checklist its options: the check boxes and the item that selects all the results of the list, and the
     * summary that takes the place of the choice of each selected item.
     * @param opt - The options to change.
     */
    protected applyCheckList(opt: ComboboxOptions) {
        if (this.comboboxOptions.placeholderIcon)
            opt.placeholderIcon = this.comboboxOptions.placeholderIcon;

        if (!this.useCheckList())
            return;

        opt.checkList = true;
        opt.containerCssClass = [opt.containerCssClass, "select2-container-checklist"].filter(Boolean).join(" ");
        opt.dropdownCssClass = [opt.dropdownCssClass, "select2-dropdown-checklist"].filter(Boolean).join(" ");
        opt.formatResult = (item, container, query) => this.formatCheckListResult(item, container, query);
        opt.formatSelectAll = (item, container, query) => this.formatSelectAll(item, container, query);
        opt.selectionSummary = (items, container) => this.renderSelectionSummary(items, container);

        // the select adds the item to the results that are loaded, with a text that tells which they are, e.g. the ones
        // that are shown of an asynchronous source, whose results come in pages
        if (this.comboboxOptions.selectAll !== false)
            opt.selectAll = true;
    }

    /**
     * Renders a result of a checklist, with a check box before it, which the styles mark when it is selected.
     */
    protected formatCheckListResult(item: ComboboxItem, container: HTMLElement, query: Select2QueryOptions) {
        return <><i class="select2-checkbox" aria-hidden="true"></i>{this.formatCheckListContent(item, query)}</>;
    }

    /**
     * Renders the item that selects all the results of a checklist, or clears them, with a check box, and a text that
     * the select keeps up to date.
     */
    protected formatSelectAll(item: ComboboxItem, container: HTMLElement, query: Select2QueryOptions) {
        return <><i class="select2-checkbox" aria-hidden="true"></i>
            <span class="select2-select-all-text">{item.text}</span></>;
    }

    /** Renders the icon of an item, when it has one, and its text, which the matching part of the search is marked in. */
    protected formatCheckListContent(item: ComboboxItem, query: Select2QueryOptions) {
        return item.icon ?
            <><i class={iconClassName(item.icon)}></i> {Select2.highlightMatch(item.text, query?.term)}</> :
            Select2.highlightMatch(item.text, query?.term);
    }

    /**
     * Renders the selection of a checklist in one element, e.g. "A, B, C +2", with the full list in its tooltip, or
     * the placeholder of the editor, with {@link ComboboxCommonOptions.placeholderIcon}, when nothing is selected.
     */
    protected renderSelectionSummary(items: ComboboxItem[], container: HTMLElement) {
        container.replaceChildren();
        container.removeAttribute("title");

        if (!items.length) {
            container.classList.add("select2-selection-placeholder");
            if (this.comboboxOptions.placeholderIcon)
                container.append(<i class={iconClassName(this.comboboxOptions.placeholderIcon)}></i>);
            container.append(this.emptyItemText() ?? "");
            return;
        }

        container.classList.remove("select2-selection-placeholder");
        const named = items.filter(x => x.text != null && x.text !== "");
        // an option that is not given, or null, takes the default, which may also be not set
        const max = this.comboboxOptions.maxSummaryItems ?? ComboboxEditor.defaults.maxSummaryItems ?? -1;

        if (named.length)
            container.setAttribute("title", named.map(x => x.text).join(", "));

        // an editor whose items have no text, or one that is told to name none of them, shows the count of the selection
        if (!named.length || max === 0) {
            container.textContent = stringFormat(SelectEditorTexts.SelectedItems, items.length);
            return;
        }

        // a summary without a limit names every item, and leaves the cutting to the styles, as the width of the control
        // is what decides how many of the names are seen, so it shows how many are selected instead of the rest, which
        // is what -1 says, while the other negative ones are reserved for summaries to come
        const namesAll = max < 0;
        const shown = namesAll ? named : named.slice(0, max);

        // the count is an element of its own, which the styles keep whole, so that the names are the ones that are cut
        // when the summary does not fit, and the count is always seen
        container.append(<span class="select2-selection-summary-text">
            {shown.map((item, i) => <>
                {i ? ", " : null}
                {item.icon ? <><i class={iconClassName(item.icon)}></i>{" "}</> : null}
                {item.text}
            </>)}
        </span>);

        if (namesAll)
            container.append(<span class="select2-selection-summary-more">{"(" + items.length + ")"}</span>);
        else if (named.length > max)
            container.append(<span class="select2-selection-summary-more">{"+" + (named.length - max)}</span>);
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

    /** Sets the items in the editor.
     * @param value - The combobox items to set. */
    public set items(value: ComboboxItem<TItem>[]) {
        if (this.hasAsyncSource())
            throw new Error("Can't set items of an async select editor!");

        this._items = value || [];
        this._itemById = Object.create(null);
        for (const item of this._items)
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
            text: this.localizeItemText(text, source),
            cssClass: source?.cssClass ?? this.itemCssClass(source),
            source: source,
            disabled: disabled
        });
    }

    /**
     * Returns the field of an item that holds its local text key, see {@link ComboboxCommonOptions.textKeyField}, which
     * a lookup can also tell in its script, e.g. one of a table that keeps its texts as keys.
     */
    protected getTextKeyField(): string {
        return this.comboboxOptions.textKeyField;
    }

    /**
     * Returns the field of an item that holds its CSS class, see {@link ComboboxCommonOptions.cssClassField}, which a
     * lookup can also tell in its script.
     */
    protected getCssClassField(): string {
        return this.comboboxOptions.cssClassField;
    }

    /**
     * Returns the CSS class of an item, from the field that the editor or its source gives, see
     * {@link ComboboxCommonOptions.cssClassField}.
     */
    protected itemCssClass(item: any): string {
        const field = this.getCssClassField();
        const value = field ? item?.[field] : undefined;
        return value == null ? undefined : value.toString();
    }

    /**
     * Returns the text of an item, localized when the editor says which field of it holds the local text key, see
     * {@link ComboboxCommonOptions.textKeyField}. An item may have no text of its own, only the key, in which case its
     * text is what the key gives, and the key itself when there is no text for it, as a key is readable enough. A null
     * or an empty text is no text, which the styles show as an empty cell, not as a missing one.
     */
    protected localizeItemText(text: string | null | undefined, source?: any) {
        const field = this.getTextKeyField();
        if (!field)
            return text ?? "";

        const key = field === "text" ? text : source?.[field];
        if (key == null || key === "")
            return text ?? "";

        return localText(key, text ?? undefined);
    }

    /**
     * Adds the in-place create button.
     * @param addTitle - Title for the add button.
     * @param editTitle - Title for the edit button.
     */
    protected addInplaceCreate(addTitle: string, editTitle: string) {
        const self = this;
        addTitle = (addTitle ?? SelectEditorTexts.InplaceAdd);
        editTitle = (editTitle ?? SelectEditorTexts.InplaceEdit);
        const inplaceButton = (<a class="inplace-button inplace-create" title={addTitle} onClick={e => {
            self.inplaceCreateClick(e as any);
        }}><b></b></a>) as HTMLElement;
        this.domNode.after(inplaceButton);

        this.getComboboxContainer()?.classList.add("has-inplace-button");
        this.domNode.classList.add("has-inplace-button");

        this.element.on("change", () => {
            const isNew = this.isMultiple() || !this.get_value();
            inplaceButton.title = (isNew ? addTitle : editTitle);
            inplaceButton.classList.toggle('edit', !isNew);
        });

        this.element.on("change", (e: any) => {
            if ((e.target.dataset.comboboxsettingvalue))
                return;
            if (this.isMultiple()) {
                const values = this.get_values();
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
                let q = Fluent(e3.target);
                if (!q.hasClass('select2-search-choice')) {
                    q = q.closest('.select2-search-choice');
                }
                const index = Array.from(q.parent().getNode()?.children || []).indexOf(q.getNode());
                const values1 = this.get_values();
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

            if (!s?.trim().length) {
                return null;
            }

            if ((this._items || []).some((x: ComboboxItem<TItem>) => {
                const text = getName ? getName(x.source) : x.text;
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
        const val = source[property.name];
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

        const contains: TItem[] = [];
        function filter(item: TItem): boolean {
            let text = getText(item);
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

    /** Sets the current value.
     * @param v - The single value to set. */
    set value(v: string) {
        this.set_value(v);
    }

    /**
     * Returns the currently selected item.
     * @returns The selected item, or null.
     */
    get selectedItem(): TItem {
        const selectedValue = this.get_value();
        if (selectedValue && this._itemById) {
            const item = this._itemById[selectedValue];
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
        const selectedValues = this.values;
        const result = [];
        for (const value of selectedValues) {
            if (value && this._itemById) {
                const item = this._itemById[value];
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

    /** Sets the current values.
     * @param value - The array of selected values to set. */
    set values(value: string[]) {
        this.set_values(value);
    }

    /**
     * Returns the display text of the current selection.
     * @returns The text.
     */
    protected get_text(): string {
        const combobox = Combobox.getInstance(this.domNode);
        if (combobox)
            return combobox.getSelectedItems()?.map(x => x.text).join(", ");

        return this.domNode.value;
    }

    /**
     * Returns the display text of the current selection.
     * @returns The comma-joined display text of the selected item(s). */
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
        const readOnly = this.get_readOnly() &&
            (this.isMultiple() || !this.value);
        const el = this.element.nextSibling(".inplace-create").getNode();
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

    /** Sets the cascade-from parent id.
     * @param value - The parent editor id to cascade from. */
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

    /** Sets the cascade field name.
     * @param value - The field name used for cascading. */
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

    /** Sets the cascade value.
     * @param value - The cascade value to filter by. */
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

    /** Sets the filter field name.
     * @param value - The field name used for filtering. */
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

    /** Sets the filter value.
     * @param value - The filter value to apply. */
    set filterValue(value: any) {
        this.set_filterValue(value);
    }

    /**
     * Filters items by the cascade value.
     * @param items - The items to filter.
     * @returns The filtered items.
     */
    protected cascadeItems(items: TItem[]) {

        const val = this.get_cascadeValue();

        if (val == null || val === '') {

            if (this.get_cascadeField()) {
                return [];
            }

            return items;
        }

        const key = val.toString();
        const fld = this.get_cascadeField();

        return items.filter(x => {
            const itemKey = (x as any)[fld];
            return !!(itemKey != null && itemKey.toString() === key);
        });
    }

    /**
     * Filters items by the filter value.
     * @param items - The items to filter.
     * @returns The filtered items.
     */
    protected filterItems(items: TItem[]) {
        const val = this.get_filterValue();

        if (val == null || val === '') {
            return items;
        }

        const key = val.toString();
        const fld = this.get_filterField();

        return items.filter(x => {
            const itemKey = (x as any)[fld];
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

    /** [DEPRECATED] Override getDialogType() instead */
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
            const dialog = new dialogType({}).init?.();
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
                    const id = dci.entityId.toString();

                    if (this.isMultiple()) {
                        const values = this.get_values().slice();
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
                    const id1 = dci.entityId.toString();
                    const values1 = this.get_values().slice();

                    const idx1 = values1.indexOf(id1);
                    if (idx1 >= 0)
                        values1.splice(idx1, 1);

                    this.set_values(values1.slice());
                }
                else if (!this.isMultiple()) {
                    this.set_value(null);
                }
            }, true);

            const editItem = (e as any)['editItem'];
            if (editItem != null) {
                dialog.load(editItem, () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                });
            }
            else if (this.isMultiple() || !this.get_value()) {
                const entity: TItem = {} as any;
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

    /** Opens the combobox dropdown. */
    public openDropdown() {
        Combobox.getInstance(this.domNode)?.openDropdown();
    }

    /** Whether the in-place add dialog opens as a panel. */
    declare public openDialogAsPanel: boolean;
}

