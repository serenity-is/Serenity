import Select2 from "@optionaldeps/select2";
import { Authorization, PropertyItem, any, isEmptyOrNull, isTrimmedEmpty, localText, startsWith, trimToEmpty, trimToNull } from "../../q";
import { Decorators } from "../../decorators";
import { IEditDialog, IGetEditValue, IReadOnly, ISetEditValue, IStringValue } from "../../interfaces";
import { DialogTypeRegistry } from "../../types/dialogtyperegistry";
import { ReflectionUtils } from "../../types/reflectionutils";
import { SubDialogHelper } from "../helpers/subdialoghelper";
import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";
import { EditorUtils } from "./editorutils";

export interface Select2CommonOptions {
    allowClear?: boolean;
    delimited?: boolean;
    minimumResultsForSearch?: any;
    multiple?: boolean;
}

export interface Select2FilterOptions {
    cascadeFrom?: string;
    cascadeField?: string;
    cascadeValue?: any;
    filterField?: string;
    filterValue?: any;
}

export interface Select2InplaceAddOptions {
    inplaceAdd?: boolean;
    inplaceAddPermission?: string;
    dialogType?: string;
    autoComplete?: boolean;
}

export interface Select2EditorOptions extends Select2FilterOptions, Select2InplaceAddOptions, Select2CommonOptions {
}

export interface Select2SearchPromise {
    abort?(): void;
    catch?(callback: () => void): void;
    fail?(callback: () => void): void;
}

export interface Select2SearchQuery {
    searchTerm?: string;
    idList?: string[];
    skip?: number;
    take?: number;
    checkMore?: boolean;
}

export interface Select2SearchResult<TItem> {
    items: TItem[];
    more: boolean;
}

@Decorators.registerClass('Serenity.Select2Editor',
    [ISetEditValue, IGetEditValue, IStringValue, IReadOnly])
@Decorators.element("<input type=\"hidden\"/>")
export class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements
    ISetEditValue, IGetEditValue, IStringValue, IReadOnly {

    private _items: Select2Item[];
    private _itemById: { [key: string]: Select2Item };
    protected lastCreateTerm: string;

    constructor(hidden: JQuery, opt?: any) {
        super(hidden, opt);

        this._items = [];
        this._itemById = {};
        var emptyItemText = this.emptyItemText();
        if (emptyItemText != null) {
            hidden.attr('placeholder', emptyItemText);
        }
        var select2Options = this.getSelect2Options();
        hidden.select2(select2Options);
        hidden.attr('type', 'text');

        // for jquery validate to work
        hidden.on('change.' + this.uniqueName, (e: any, valueSet) => {
            if (valueSet !== true && hidden.closest('form').data('validator'))
                    hidden.valid();
        });

        this.setCascadeFrom((this.options as Select2EditorOptions).cascadeFrom);

        if (this.useInplaceAdd())
            this.addInplaceCreate(localText('Controls.SelectEditor.InplaceAdd'), null);
    }

    destroy() {
        this.initSelectionPromise?.abort?.();
        this.abortPendingQuery();
        this.element?.select2?.('destroy');
        super.destroy();
    }

    protected hasAsyncSource(): boolean {
        return false;
    }

    protected asyncSearch(query: Select2SearchQuery, results: (result: Select2SearchResult<TItem>) => void): Select2SearchPromise {
        results({
            items: [],
            more: false
        });
        return null;
    }

    protected getTypeDelay() {
        return ((this.options as any)['typeDelay'] ?? 200);
    }

    protected emptyItemText() {
        return this.element.attr('placeholder') ??
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

    protected mapItem(item: TItem): Select2Item {
        return {
            id: this.itemId(item),
            text: this.itemText(item),
            disabled: this.itemDisabled(item),
            source: item
        };
    }

    protected mapItems(items: TItem[]): Select2Item[] {
        return items.map(this.mapItem.bind(this));
    }

    protected allowClear() {
        return (this.options as Select2EditorOptions).allowClear != null ?
            !!(this.options as Select2EditorOptions).allowClear : this.emptyItemText() != null;
    }

    protected isMultiple() {
        return !!(this.options as Select2EditorOptions).multiple;
    }

    private initSelectionPromise: Select2SearchPromise;
    private queryPromise: Select2SearchPromise;
    private typeTimeout: number;

    protected abortPendingQuery() {
        this.queryPromise?.abort?.();
        this.queryPromise = null;
        if (this.typeTimeout) {
            clearTimeout(this.typeTimeout);
            this.typeTimeout = null;
        }
    }

    protected getSelect2Options(): Select2Options {
        var emptyItemText = this.emptyItemText();
        var opt: Select2Options = {
            multiple: this.isMultiple(),
            placeHolder: (!isEmptyOrNull(emptyItemText) ? emptyItemText : null),
            allowClear: this.allowClear(),
            createSearchChoicePosition: 'bottom'
        }

        if (this.hasAsyncSource()) {
            opt.query = query => {
                var pageSize = this.getPageSize();
                var searchQuery: Select2SearchQuery = {
                    searchTerm: trimToNull(query.term),
                    skip: (query.page - 1) * pageSize,
                    take: pageSize,
                    checkMore: true
                }

                this.abortPendingQuery();

                var select2 = $(this.element).data('select2');
                select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');

                this.typeTimeout = setTimeout(() => {
                    this.abortPendingQuery();
                    select2?.search?.addClass?.('select2-active').parent().addClass('select2-active');
                    this.queryPromise = this.asyncSearch(searchQuery, result => {
                        select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
                        this.queryPromise = null;
                        query.callback({
                            results: this.mapItems(result.items),
                            more: result.more
                        });
                    });
                    (this.queryPromise?.catch || this.queryPromise?.fail)?.call(this.queryPromise, () => {
                        this.queryPromise = null;
                        select2?.search?.removeClass?.('select2-active').parent().removeClass('select2-active');
                    });
                }, !query.term ? 0 : this.getTypeDelay());
            }

            opt.initSelection = (element, callback) => {
                var val = element.val();
                if (val == null || val == '') {
                    callback(null);
                    return;
                }

                var isMultiple = this.isMultiple();
                var idList = isMultiple ? val.split(',') : [val]; 
                var searchQuery = {
                    idList: idList
                }

                this.initSelectionPromise?.abort?.();
                this.initSelectionPromise = this.asyncSearch(searchQuery, result => {
                    this.initSelectionPromise = null;
                    if (isMultiple) {
                        var items = (result.items || []).map(x => this.mapItem(x));
                        this._itemById = this._itemById || {};
                        for (var item of items)
                            this._itemById[item.id] = item;
                        if (this.isAutoComplete &&
                            items.length != idList.length) {
                            for (var v of idList) {
                                if (!any(items, z => z.id == v)) {
                                    items.push({
                                        id: v,
                                        text: v
                                    });
                                }
                            }
                        }
                        callback(items);
                    }
                    else if (!result.items || !result.items.length) {
                        if (this.isAutoComplete) {
                            callback({
                                id: val,
                                text: val
                            });
                        }
                        else
                            callback(null);
                    }
                    else {
                        var item = this.mapItem(result.items[0]);
                        this._itemById = this._itemById || {};
                        this._itemById[item.id] = item;
                        callback(item);
                    }
                });
                (this.initSelectionPromise.catch || this.initSelectionPromise.fail)?.call(this.initSelectionPromise, () => {
                    this.initSelectionPromise = null;
                });
            }
        }
        else {
            opt.data = this._items;
            opt.query = (query) => {
                var items = Select2Editor.filterByText(this._items, x => x.text, query.term);
                var pageSize = this.getPageSize();
                query.callback({
                    results: items.slice((query.page - 1) * pageSize, query.page * pageSize),
                    more: items.length >= query.page * pageSize
                });
            }
            opt.initSelection = (element, callback) => {
                var val = element.val();
                var isAutoComplete = this.isAutoComplete();
                if (this.isMultiple()) {
                    var list = [];
                    for (var z of val.split(',')) {
                        var item2 = this._itemById[z];
                        if (item2 == null && isAutoComplete) {
                            item2 = { id: z, text: z };
                            this.addItem(item2);
                        }
                        if (item2 != null) {
                            list.push(item2);
                        }
                    }
                    callback(list);
                    return;
                }
                var it = this._itemById[val];
                if (it == null && isAutoComplete) {
                    it = { id: val, text: val };
                    this.addItem(it);
                }
                callback(it);
            }
        }

        if ((this.options as Select2EditorOptions).minimumResultsForSearch != null)
            opt.minimumResultsForSearch = (this.options as Select2EditorOptions).minimumResultsForSearch;

        if (this.isAutoComplete() || this.useInplaceAdd())
            opt.createSearchChoice = this.getCreateSearchChoice(null);

        return opt;
    }

    get_delimited() {
        return !!(this.options as Select2EditorOptions).delimited;
    }

    public get items(): Select2Item[] {
        if (this.hasAsyncSource())
            throw new Error("Can't read items property of an async select editor!");

        return this._items || [];
    }

    public set items(value: Select2Item[]) {
        if (this.hasAsyncSource())
            throw new Error("Can't set items of an async select editor!");

        this._items = value || [];
        this._itemById = {};
        for (var item of this._items)
            this._itemById[item.id] = item;
    }

    protected get itemById(): { [key: string]: Select2Item } {
        if (this.hasAsyncSource())
            throw new Error("Can't read items property of an async select editor!");

        return this._itemById;
    }

    protected set itemById(value: { [key: string]: Select2Item }) {
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

    public addItem(item: Select2Item) {
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
        var inplaceButton = $('<a><b/></a>')
            .addClass('inplace-button inplace-create')
            .attr('title', addTitle)
            .insertAfter(this.element).click(function (e) {
                self.inplaceCreateClick(e);
            });

        this.get_select2Container().add(this.element).addClass('has-inplace-button');

        this.element.change(() => {
            var isNew = this.isMultiple() || isEmptyOrNull(this.get_value());
            inplaceButton.attr('title', (isNew ? addTitle : editTitle)).toggleClass('edit', !isNew);
        });

        this.element.change((e: any, valueSet: boolean) => {
            if (valueSet === true)
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
            this.get_select2Container().on('dblclick.' + this.uniqueName, '.select2-search-choice', (e3: JQueryEventObject) => {
                var q = $(e3.target);
                if (!q.hasClass('select2-search-choice')) {
                    q = q.closest('.select2-search-choice');
                }
                var index = q.index();
                var values1 = this.get_values();
                if (index < 0 || index >= this.get_values().length) {
                    return;
                }
                (e3 as any)['editItem'] = values1[index];
                this.inplaceCreateClick(e3);
            });
        }
    }

    protected useInplaceAdd(): boolean {
        return !this.isAutoComplete() &&
            (this.options as Select2EditorOptions).inplaceAdd &&
            ((this.options as Select2EditorOptions).inplaceAddPermission == null ||
                Authorization.hasPermission((this.options as Select2EditorOptions).inplaceAddPermission));
    }

    protected isAutoComplete(): boolean {
        return !!(this.options as Select2EditorOptions).autoComplete;
    }

    public getCreateSearchChoice(getName: (z: any) => string) {
        return (s: string) => {

            this.lastCreateTerm = s;
            s = (Select2.util.stripDiacritics(s) ?? '').toLowerCase();

            if (isTrimmedEmpty(s)) {
                return null;
            }

            var isAsyncSource = false;

            if (any(this._items || [], (x: Select2Item) => {
                    var text = getName ? getName(x.source) : x.text;
                    return Select2.util.stripDiacritics((text ?? '')).toLowerCase() == s;
            }))
                return null;

            if (!any(this._items || [], x1 => {
                return (Select2.util.stripDiacritics(x1.text) ?? '').toLowerCase().indexOf(s) !== -1;
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
        if(Array.isArray(val)) {
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

    protected get_select2Container(): JQuery {
        return this.element.prevAll('.select2-container');
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

        term = Select2.util.stripDiacritics(term).toUpperCase();

        var contains: TItem[] = [];
        function filter(item: TItem): boolean {
            var text = getText(item);
            if (text == null || !text.length)
                return false;
            text = Select2.util.stripDiacritics(text).toUpperCase();
            if (startsWith(text, term))
                return true;
            if (text.indexOf(term) >= 0)
                contains.push(item);
            return false;
        }

        return items.filter(filter).concat(contains);
    }

    get_value() {
        var val;
        if (this.element.data('select2')) {
            val = this.element.select2('val');
            if (val != null && Array.isArray(val)) {
                return val.join(',');
            }
        }
        else
            val = this.element.val();

        return val;
    }

    get value(): string {
        return this.get_value();
    }

    set_value(value: string) {

        if (value != this.get_value()) {
            var val: any = value;
            if (!isEmptyOrNull(value) && this.isMultiple()) {
                val = value.split(String.fromCharCode(44)).map(function (x) {
                    return trimToNull(x);
                }).filter(function (x1) {
                    return x1 != null;
                });
            }

            var el = this.element;
            el.select2('val', val);
            el.data('select2-change-triggered', true);
            try {
                el.triggerHandler('change', [true]); // valueSet: true
            } finally {
                el.data('select2-change-triggered', false);
            }

            this.updateInplaceReadOnly();
        }
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

        var val = this.element.select2('val');
        if (val == null) {
            return [];
        }

        if (Array.isArray(val)) {
            return val;
        }

        var str = val;
        if (isEmptyOrNull(str)) {
            return [];
        }

        return [str];
    }

    get values(): string[] {
        return this.get_values();
    }

    protected set_values(value: string[]) {

        if (value == null || value.length === 0) {
            this.set_value(null);
            return;
        }

        this.set_value(value.join(','));
    }

    set values(value: string[]) {
        this.set_values(value);
    }

    protected get_text(): string {
        return (this.element.select2('data') ?? {}).text;
    }

    get text(): string {
        return this.get_text();
    }

    get_readOnly(): boolean {
        return !isEmptyOrNull(this.element.attr('readonly'));
    }

    get readOnly(): boolean {
        return this.get_readOnly();
    }

    private updateInplaceReadOnly(): void {
        var readOnly = this.get_readOnly() &&
            (this.isMultiple() || !this.value);

        this.element.nextAll('.inplace-create')
            .attr('disabled', (readOnly ? 'disabled' : ''))
            .css('opacity', (readOnly ? '0.1' : ''))
            .css('cursor', (readOnly ? 'default' : ''));
    }

    set_readOnly(value: boolean) {
        if (value !== this.get_readOnly()) {
            this.element.attr("readonly", value ? "readonly" : null);
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

        if (isEmptyOrNull(value)) {
            if (this.cascadeLink != null) {
                this.cascadeLink.set_parentID(null);
                this.cascadeLink = null;
            }
            (this.options as Select2EditorOptions).cascadeFrom = null;
            return;
        }

        this.cascadeLink = new CascadedWidgetLink<Widget<any>>(Widget, this, p => {
            this.set_cascadeValue(this.getCascadeFromValue(p));
        });

        this.cascadeLink.set_parentID(value);
        (this.options as Select2EditorOptions).cascadeFrom = value;
    }

    protected get_cascadeFrom(): string {
        return (this.options as Select2EditorOptions).cascadeFrom;
    }

    get cascadeFrom(): string {
        return this.get_cascadeFrom();
    }

    protected set_cascadeFrom(value: string) {
        if (value !== (this.options as Select2EditorOptions).cascadeFrom) {
            this.setCascadeFrom(value);
            this.updateItems();
        }
    }

    set cascadeFrom(value: string) {
        this.set_cascadeFrom(value);
    }

    protected get_cascadeField() {
        return ((this.options as Select2EditorOptions).cascadeField ?? (this.options as Select2EditorOptions).cascadeFrom);
    }

    get cascadeField(): string {
        return this.get_cascadeField();
    }

    protected set_cascadeField(value: string) {
        (this.options as Select2EditorOptions).cascadeField = value;
    }

    set cascadeField(value: string) {
        this.set_cascadeField(value);
    }

    protected get_cascadeValue(): any {
        return (this.options as Select2EditorOptions).cascadeValue;
    }

    get cascadeValue(): any {
        return this.get_cascadeValue();
    }

    protected set_cascadeValue(value: any) {
        if ((this.options as Select2EditorOptions).cascadeValue !== value) {
            (this.options as Select2EditorOptions).cascadeValue = value;
            this.set_value(null);
            this.updateItems();
        }
    }

    set cascadeValue(value: any) {
        this.set_cascadeValue(value);
    }

    protected get_filterField() {
        return (this.options as Select2EditorOptions).filterField;
    }

    get filterField(): string {
        return this.get_filterField();
    }

    protected set_filterField(value: string) {
        (this.options as Select2EditorOptions).filterField = value;
    }

    set filterField(value: string) {
        this.set_filterField(value);
    }

    protected get_filterValue(): any {
        return (this.options as Select2EditorOptions).filterValue;
    }

    get filterValue(): any {
        return this.get_filterValue();
    }

    protected set_filterValue(value: any) {
        if ((this.options as Select2EditorOptions).filterValue !== value) {
            (this.options as Select2EditorOptions).filterValue = value;
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

            if (!isEmptyOrNull(this.get_cascadeField())) {
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
        if ((this.options as Select2EditorOptions).dialogType != null) {
            return (this.options as Select2EditorOptions).dialogType;
        }

        return null;
    }

    protected createEditDialog(callback: (dlg: IEditDialog) => void) {
        var dialogTypeKey = this.getDialogTypeKey();
        var dialogType = DialogTypeRegistry.get(dialogTypeKey);
        Widget.create({
            type: dialogType,
            init: x => callback(x as any)
        });
    }

    public onInitNewEntity: (entity: TItem) => void;

    protected initNewEntity(entity: TItem) {
        if (!isEmptyOrNull(this.get_cascadeField())) {
            (entity as any)[this.get_cascadeField()] = this.get_cascadeValue();
        }

        if (!isEmptyOrNull(this.get_filterField())) {
            (entity as any)[this.get_filterField()] = this.get_filterValue();
        }

        if (this.onInitNewEntity != null) {
            this.onInitNewEntity(entity);
        }
    }

    protected setEditDialogReadOnly(dialog: any): void {
        // an ugly workaround
        dialog.element && dialog.element
            .find('.tool-button.delete-button')
            .addClass('disabled')
            .unbind('click');
    }

    protected editDialogDataChange() {
    }

    protected setTermOnNewEntity(entity: TItem, term: string) {
    }

    protected inplaceCreateClick(e: JQueryEventObject) {

        if (this.get_readOnly() &&
            ((this.isMultiple() && !(e as any)['editItem']) || !this.value))
            return;

        this.createEditDialog(dialog => {

            if (this.get_readOnly())
                this.setEditDialogReadOnly(dialog);

            SubDialogHelper.bindToDataChange(dialog, this, (x, dci) => {
                this.editDialogDataChange();
                this.updateItems();
                this.lastCreateTerm = null;

                if ((dci.type === 'create' || dci.type === 'update') &&
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
                else if (this.isMultiple() && dci.type === 'delete' &&
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
            else if (this.isMultiple() || isEmptyOrNull(this.get_value())) {
                var entity: TItem = {} as any;
                this.setTermOnNewEntity(entity, trimToEmpty(this.lastCreateTerm));
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

    public openDialogAsPanel: boolean;
}