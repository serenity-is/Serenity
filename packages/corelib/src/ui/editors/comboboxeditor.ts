import { Fluent, PropertyItem, localText } from "../../base";
import { IEditDialog, IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../interfaces";
import { Authorization, ValidationHelper, isTrimmedEmpty } from "../../q";
import { Decorators } from "../../types/decorators";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { ReflectionUtils } from "../../types/reflectionutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { EditorProps, Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";
import { Combobox, ComboboxItem, ComboboxOptions, ComboboxSearchQuery, ComboboxSearchResult, stripDiacritics } from "./combobox";
import { EditorUtils } from "./editorutils";

export interface ComboboxCommonOptions {
    allowClear?: boolean;
    delimited?: boolean;
    minimumResultsForSearch?: any;
    multiple?: boolean;
}

export interface ComboboxFilterOptions {
    cascadeFrom?: string;
    cascadeField?: string;
    cascadeValue?: any;
    filterField?: string;
    filterValue?: any;
}

export interface ComboboxInplaceAddOptions {
    inplaceAdd?: boolean;
    inplaceAddPermission?: string;
    dialogType?: string;
    autoComplete?: boolean;
}

export interface ComboboxEditorOptions extends ComboboxFilterOptions, ComboboxInplaceAddOptions, ComboboxCommonOptions {
}

@Decorators.registerClass('Serenity.ComboboxEditor',
    [ISetEditValue, IGetEditValue, IStringValue, IReadOnly])
export class ComboboxEditor<P, TItem> extends Widget<P> implements
    ISetEditValue, IGetEditValue, IStringValue, IReadOnly {

    static override createDefaultElement() { return Fluent("input").attr("type", "hidden").getNode(); }
    declare readonly domNode: HTMLInputElement;

    private combobox: Combobox;
    private _items: ComboboxItem<TItem>[];
    private _itemById: { [key: string]: ComboboxItem<TItem> };
    protected lastCreateTerm: string;

    constructor(props: EditorProps<P>) {
        super(props);

        let hidden = this.domNode;

        this._items = [];
        this._itemById = {};
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
            this.addInplaceCreate(localText('Controls.SelectEditor.InplaceAdd'), null);
    }

    destroy() {
        this.combobox?.dispose();
        this.combobox = null;
        super.destroy();
    }

    protected hasAsyncSource(): boolean {
        return false;
    }

    protected asyncSearch(query: ComboboxSearchQuery): PromiseLike<ComboboxSearchResult<TItem>> {
        return Promise.resolve({
            items: [],
            more: false
        });
    }

    protected getTypeDelay() {
        return ((this.options as any)['typeDelay'] ?? 200);
    }

    protected emptyItemText() {
        return this.domNode.getAttribute("placeholder") ??
            localText('Controls.SelectEditor.EmptyItemText');
    }

    protected getPageSize(): number {
        return (this.options as any)['pageSize'] ?? 100;
    }

    protected getIdField() {
        return (this.options as any)['idField'];
    }

    protected itemId(item: TItem): string {
        var value = (item as any)[this.getIdField()];
        if (value == null)
            return '';
        return value.toString();
    }

    protected getTextField() {
        return (this.options as any)['textField'] ?? this.getIdField();
    }

    protected itemText(item: TItem): string {
        var value = (item as any)[this.getTextField()];
        if (value == null)
            return '';
        return value.toString();
    }

    protected itemDisabled(item: TItem): boolean {
        return false;
    }

    protected mapItem(item: TItem): ComboboxItem {
        return {
            id: this.itemId(item),
            text: this.itemText(item),
            disabled: this.itemDisabled(item),
            source: item
        };
    }

    protected mapItems(items: TItem[]): ComboboxItem[] {
        return items.map(this.mapItem.bind(this));
    }

    protected allowClear() {
        return (this.options as ComboboxEditorOptions).allowClear != null ?
            !!(this.options as ComboboxEditorOptions).allowClear : this.emptyItemText() != null;
    }

    protected isMultiple() {
        return !!(this.options as ComboboxEditorOptions).multiple;
    }

    protected abortPendingQuery() {
        this.combobox?.abortPendingQuery();
    }

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
                var items = this.mapItems(result.items || []);
                var mappedResult = {
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
                var items;
                if (query.initSelection) {
                    items = this._items.filter(x => query.idList?.includes(x.id))
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

    get_delimited() {
        return !!(this.options as ComboboxEditorOptions).delimited;
    }

    public get items(): ComboboxItem<TItem>[] {
        if (this.hasAsyncSource())
            throw new Error("Can't read items property of an async select editor!");

        return this._items || [];
    }

    public set items(value: ComboboxItem<TItem>[]) {
        if (this.hasAsyncSource())
            throw new Error("Can't set items of an async select editor!");

        this._items = value || [];
        this._itemById = {};
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

        this._itemById = value || {};
    }

    public clearItems() {
        if (this.hasAsyncSource())
            throw new Error("Can't clear items of an async select editor!");

        this._items.length = 0;
        this._itemById = {};
    }

    public addItem(item: ComboboxItem<TItem>) {
        if (this.hasAsyncSource())
            throw new Error("Can't add item to an async select editor!");

        this._items.push(item);
        this._itemById[item.id] = item;
    }

    public addOption(key: string, text: string, source?: any, disabled?: boolean) {
        this.addItem({
            id: key,
            text: text,
            source: source,
            disabled: disabled
        });
    }

    protected addInplaceCreate(addTitle: string, editTitle: string) {
        var self = this;
        addTitle = (addTitle ?? localText('Controls.SelectEditor.InplaceAdd'));
        editTitle = (editTitle ?? localText('Controls.SelectEditor.InplaceEdit'));
        var inplaceButton = Fluent("a")
            .class('inplace-button inplace-create')
            .attr('title', addTitle)
            .append(Fluent("b"))
            .insertAfter(this.domNode)
            .on("click", function (e) {
                self.inplaceCreateClick(e as any);
            });

        this.getComboboxContainer()?.classList.add("has-inplace-button");
        this.domNode.classList.add("has-inplace-button");

        this.element.on("change", () => {
            var isNew = this.isMultiple() || !this.get_value();
            inplaceButton.attr('title', (isNew ? addTitle : editTitle)).toggleClass('edit', !isNew);
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

    protected useInplaceAdd(): boolean {
        return !this.isAutoComplete() &&
            (this.options as ComboboxEditorOptions).inplaceAdd &&
            ((this.options as ComboboxEditorOptions).inplaceAddPermission == null ||
                Authorization.hasPermission((this.options as ComboboxEditorOptions).inplaceAddPermission));
    }

    protected isAutoComplete(): boolean {
        return !!(this.options as ComboboxEditorOptions).autoComplete;
    }

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
                    text: localText('Controls.SelectEditor.NoResultsClickToDefine')
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
                text: localText('Controls.SelectEditor.ClickToDefine')
            };
        }
    }

    setEditValue(source: any, property: PropertyItem) {
        var val = source[property.name];
        if (Array.isArray(val)) {
            this.set_values(val);
        }
        else {
            this.set_value((val == null ? null : val.toString()));
        }
    }

    getEditValue(property: PropertyItem, target: any) {
        if (!this.isMultiple() || this.get_delimited()) {
            target[property.name] = this.get_value();
        }
        else {
            target[property.name] = this.get_values();
        }
    }

    protected getComboboxContainer(): HTMLElement {
        return Combobox.getInstance(this.domNode)?.container;
    }

    protected get_items() {
        return this.items;
    }

    protected get_itemByKey() {
        return this.itemById;
    }

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

    get_value() {
        return this.combobox ? this.combobox.getValue() : this.domNode?.value;
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {

        if (this.combobox) {
            this.combobox.setValue(value, /*triggerChange*/ true);
        } else if (this.domNode) {
            this.domNode.value = value;
        }

        this.updateInplaceReadOnly();
    }

    set value(v: string) {
        this.set_value(v);
    }

    get selectedItem(): TItem {
        let selectedValue = this.get_value();
        if (selectedValue && this._itemById) {
            let item = this._itemById[selectedValue];
            if (item)
                return item.source;
        }
        return null;
    }

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

    protected get_values(): string[] {
        return this.combobox?.getValues();
    }

    get values(): string[] {
        return this.get_values();
    }

    protected set_values(value: string[]) {
        this.combobox?.setValues(value);
    }

    set values(value: string[]) {
        this.set_values(value);
    }

    protected get_text(): string {
        var combobox = Combobox.getInstance(this.domNode);
        if (combobox)
            return combobox.getSelectedItems()?.map(x => x.text).join(", ");

        return this.domNode.value;
    }

    get text(): string {
        return this.get_text();
    }

    get_readOnly(): boolean {
        return this.domNode.getAttribute("readonly") != null;
    }

    get readOnly(): boolean {
        return this.get_readOnly();
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

    set_readOnly(value: boolean) {
        if (value !== this.get_readOnly()) {
            value ? this.domNode.setAttribute("readonly", "readonly") : this.domNode.removeAttribute("readonly");
            this.updateInplaceReadOnly();
        }
    }

    set readOnly(value: boolean) {
        this.set_readOnly(value);
    }

    protected getCascadeFromValue(parent: Widget<any>) {
        return EditorUtils.getValue(parent);
    }

    protected cascadeLink: CascadedWidgetLink<Widget<any>>;

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

    protected get_cascadeFrom(): string {
        return (this.options as ComboboxEditorOptions).cascadeFrom;
    }

    get cascadeFrom(): string {
        return this.get_cascadeFrom();
    }

    protected set_cascadeFrom(value: string) {
        if (value !== (this.options as ComboboxEditorOptions).cascadeFrom) {
            this.setCascadeFrom(value);
            this.updateItems();
        }
    }

    set cascadeFrom(value: string) {
        this.set_cascadeFrom(value);
    }

    protected get_cascadeField() {
        return ((this.options as ComboboxEditorOptions).cascadeField ?? (this.options as ComboboxEditorOptions).cascadeFrom);
    }

    get cascadeField(): string {
        return this.get_cascadeField();
    }

    protected set_cascadeField(value: string) {
        (this.options as ComboboxEditorOptions).cascadeField = value;
    }

    set cascadeField(value: string) {
        this.set_cascadeField(value);
    }

    protected get_cascadeValue(): any {
        return (this.options as ComboboxEditorOptions).cascadeValue;
    }

    get cascadeValue(): any {
        return this.get_cascadeValue();
    }

    protected set_cascadeValue(value: any) {
        if ((this.options as ComboboxEditorOptions).cascadeValue !== value) {
            (this.options as ComboboxEditorOptions).cascadeValue = value;
            this.set_value(null);
            this.updateItems();
        }
    }

    set cascadeValue(value: any) {
        this.set_cascadeValue(value);
    }

    protected get_filterField() {
        return (this.options as ComboboxEditorOptions).filterField;
    }

    get filterField(): string {
        return this.get_filterField();
    }

    protected set_filterField(value: string) {
        (this.options as ComboboxEditorOptions).filterField = value;
    }

    set filterField(value: string) {
        this.set_filterField(value);
    }

    protected get_filterValue(): any {
        return (this.options as ComboboxEditorOptions).filterValue;
    }

    get filterValue(): any {
        return this.get_filterValue();
    }

    protected set_filterValue(value: any) {
        if ((this.options as ComboboxEditorOptions).filterValue !== value) {
            (this.options as ComboboxEditorOptions).filterValue = value;
            this.set_value(null);
            this.updateItems();
        }
    }

    set filterValue(value: any) {
        this.set_filterValue(value);
    }

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
            var itemKey = (x as any)[fld] ?? ReflectionUtils.getPropertyValue(x, fld);
            return !!(itemKey != null && itemKey.toString() === key);
        });
    }

    protected filterItems(items: TItem[]) {
        var val = this.get_filterValue();

        if (val == null || val === '') {
            return items;
        }

        var key = val.toString();
        var fld = this.get_filterField();

        return items.filter(x => {
            var itemKey = (x as any)[fld] ?? ReflectionUtils.getPropertyValue(x, fld);
            return !!(itemKey != null && itemKey.toString() === key);
        });
    }

    protected updateItems() {
    }

    protected getDialogTypeKey() {
        if ((this.options as ComboboxEditorOptions).dialogType != null) {
            return (this.options as ComboboxEditorOptions).dialogType;
        }

        return null;
    }

    protected createEditDialog(callback: (dlg: IEditDialog) => void) {
        var dialogTypeKey = this.getDialogTypeKey();
        var dialogType = DialogTypeRegistry.get(dialogTypeKey) as typeof Widget<{}>;
        var dialog = new dialogType({}).init();
        callback?.(dialog as unknown as IEditDialog);
    }

    public onInitNewEntity: (entity: TItem) => void;

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

    protected setEditDialogReadOnly(dialog: any): void {
        // an ugly workaround
        dialog.element &&
            (dialog.element as Fluent).findFirst &&
            (dialog.element as Fluent).findFirst('.tool-button.delete-button')
                .addClass('disabled')
                .off('click', void 0);
    }

    protected editDialogDataChange() {
    }

    protected setTermOnNewEntity(entity: TItem, term: string) {
    }

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
                }, null);
            }
            else if (this.isMultiple() || !this.get_value()) {
                var entity: TItem = {} as any;
                this.setTermOnNewEntity(entity, this.lastCreateTerm?.trim() ?? '');
                this.initNewEntity(entity);
                dialog.load(entity, () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                }, null);
            }
            else {
                dialog.load(this.get_value(), () => {
                    (dialog as any).dialogOpen(this.openDialogAsPanel);
                }, null);
            }
        });
    }

    public openDropdown() {
        Combobox.getInstance(this.domNode)?.openDropdown();
    }

    public openDialogAsPanel: boolean;
}

