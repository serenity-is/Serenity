import { Column, FormatterContext, Grid, GridOptions } from "@serenity-is/sleekgrid"
import { Decorators } from "../../decorators";
import { IGetEditValue, IReadOnly, ISetEditValue } from "../../interfaces";
import { Culture, getLookup, htmlEncode, isEmptyOrNull, ListResponse, Lookup, PropertyItem, ScriptData, trimToNull, tryGetText } from "../../q";
import { ReflectionUtils } from "../../types/reflectionutils";
import { DataGrid } from "../datagrid/datagrid";
import { GridSelectAllButtonHelper, GridUtils, SlickFormatting, SlickTreeHelper } from "../helpers/slickhelpers";
import { ToolButton } from "../widgets/toolbar";
import { Widget } from "../widgets/widget";
import { CascadedWidgetLink } from "./cascadedwidgetlink";
import { EditorUtils } from "./editorutils";

export interface CheckTreeItem<TSource> {
    isSelected?: boolean;
    hideCheckBox?: boolean;
    isAllDescendantsSelected?: boolean;
    id?: string;
    text?: string;
    parentId?: string;
    children?: CheckTreeItem<TSource>[];
    source?: TSource;
}

@Decorators.registerEditor('Serenity.CheckTreeEditor', [IGetEditValue, ISetEditValue, IReadOnly])
@Decorators.element("<div/>")
export class CheckTreeEditor<TItem extends CheckTreeItem<any>, TOptions> extends DataGrid<TItem, TOptions>
    implements IGetEditValue, ISetEditValue, IReadOnly {

    private byId: { [key: string]: TItem };

    constructor(div: JQuery, opt?: TOptions) {
        super(div, opt);

        div.addClass('s-CheckTreeEditor');
        this.updateItems();
    }

    protected getIdProperty() {
        return "id";
    }

    protected getTreeItems(): TItem[] {
        return [];
    }

    protected updateItems(): void {
        var items = this.getTreeItems();
        var itemById: Record<any, TItem> = {};
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            item.children = [];
            if (!isEmptyOrNull(item.id)) {
                itemById[item.id] = item;
            }
            if (!isEmptyOrNull(item.parentId)) {
                var parent = itemById[item.parentId];
                if (parent != null) {
                    parent.children.push(item);
                }
            }
        }
        this.view.addData({ Entities: items, Skip: 0, Take: 0, TotalCount: items.length });
        this.updateSelectAll();
        this.updateFlags();
    }

    getEditValue(property: PropertyItem, target: any): void {
        if (this.getDelimited())
            target[property.name] = this.get_value().join(",");
        else
            target[property.name] = this.get_value();
    }

    setEditValue(source: any, property: PropertyItem): void {
        var value = source[property.name];
        this.set_value(value);
    }

    protected getButtons(): ToolButton[] {
        var selectAllText = this.getSelectAllText();
        if (isEmptyOrNull(selectAllText)) {
            return null;
        }

        var self = this;
        var buttons: ToolButton[] = [];
        buttons.push(GridSelectAllButtonHelper.define(function () {
            return self;
        }, function (x) {
            return x.id;
        }, function (x1) {
            return x1.isSelected;
        }, (x2, v) => {
            if (x2.isSelected !== v) {
                x2.isSelected = v;
                this.itemSelectedChanged(x2);
            }
        }, null, () => {
            this.updateFlags();
        }));

        return buttons;
    }

    protected itemSelectedChanged(item: TItem): void {
    }

    protected getSelectAllText(): string {
        return tryGetText('Controls.CheckTreeEditor.SelectAll') ?? 'Select All';
    }

    protected isThreeStateHierarchy(): boolean {
        return false;
    }

    protected createSlickGrid(): Grid {
        this.element.addClass('slick-no-cell-border').addClass('slick-no-odd-even');
        var result = super.createSlickGrid();
        this.element.addClass('slick-hide-header');
        result.resizeCanvas();
        return result;
    }

    protected onViewFilter(item: TItem): boolean {
        if (!super.onViewFilter(item)) {
            return false;
        }

        var items = this.view.getItems();
        var self = this;
        return SlickTreeHelper.filterCustom(item, function (x) {
            if (x.parentId == null) {
                return null;
            }

            if (self.byId == null) {
                self.byId = {};
                for (var i = 0; i < items.length; i++) {
                    var o = items[i];
                    if (o.id != null) {
                        self.byId[o.id] = o;
                    }
                }
            }

            return self.byId[x.parentId];
        });
    }

    protected getInitialCollapse(): boolean {
        return false;
    }

    protected onViewProcessData(response: ListResponse<TItem>): ListResponse<TItem> {
        response = super.onViewProcessData(response);
        this.byId = null;
        SlickTreeHelper.setIndents(response.Entities, function (x) {
            return x.id;
        }, function (x1) {
            return x1.parentId;
        }, this.getInitialCollapse());
        return response;
    }

    protected onClick(e: JQueryEventObject, row: number, cell: number): void {
        super.onClick(e, row, cell);

        if (!e.isDefaultPrevented()) {
            SlickTreeHelper.toggleClick(e, row, cell, this.view, function (x) {
                return x.id;
            });
        }

        if (e.isDefaultPrevented()) {
            return;
        }

        var target = $(e.target);
        if (target.hasClass('check-box')) {
            e.preventDefault();

            if (this._readOnly)
                return;

            var checkedOrPartial = target.hasClass('checked') || target.hasClass('partial');
            var item = this.itemAt(row);
            var anyChanged = item.isSelected !== !checkedOrPartial;
            this.view.beginUpdate();
            try {
                if (item.isSelected !== !checkedOrPartial) {
                    item.isSelected = !checkedOrPartial;
                    this.view.updateItem(item.id, item);
                    this.itemSelectedChanged(item);
                }
                anyChanged = this.setAllSubTreeSelected(item, item.isSelected) || anyChanged;
                this.updateSelectAll();
                this.updateFlags();
            }
            finally {
                this.view.endUpdate();
            }
            if (anyChanged) {
                this.element.triggerHandler('change');
            }
        }
    }

    protected updateSelectAll(): void {
        GridSelectAllButtonHelper.update(this, function (x) {
            return x.isSelected;
        });
    }

    protected updateFlags(): void {
        var view = this.view;
        var items = view.getItems();
        var threeState = this.isThreeStateHierarchy();
        if (!threeState) {
            return;
        }
        view.beginUpdate();
        try {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.children == null || item.children.length === 0) {
                    var allsel = this.getDescendantsSelected(item);
                    if (allsel !== item.isAllDescendantsSelected) {
                        item.isAllDescendantsSelected = allsel;
                        view.updateItem(item.id, item);
                    }
                    continue;
                }
                var allSelected = this.allDescendantsSelected(item);
                var selected = allSelected || this.anyDescendantsSelected(item);
                if (allSelected !== item.isAllDescendantsSelected || selected !== item.isSelected) {
                    var selectedChange = item.isSelected !== selected;
                    item.isAllDescendantsSelected = allSelected;
                    item.isSelected = selected;
                    view.updateItem(item.id, item);
                    if (selectedChange) {
                        this.itemSelectedChanged(item);
                    }
                }
            }
        }
        finally {
            view.endUpdate();
        }
    }

    protected getDescendantsSelected(item: TItem): boolean {
        return true;
    }

    protected setAllSubTreeSelected(item: TItem, selected: boolean): boolean {
        var result = false;
        for (var i = 0; i < item.children.length; i++) {
            var sub = item.children[i];
            if (sub.isSelected !== selected) {
                result = true;
                sub.isSelected = selected;
                this.view.updateItem(sub.id, sub as TItem);
                this.itemSelectedChanged(sub as TItem);
            }
            if (sub.children.length > 0) {
                result = this.setAllSubTreeSelected(sub as TItem, selected) || result;
            }
        }
        return result;
    }

    protected allItemsSelected() {
        for (var i = 0; i < this.rowCount(); i++) {
            var row = this.itemAt(i);
            if (!row.isSelected) {
                return false;
            }
        }

        return this.rowCount() > 0;
    }

    protected allDescendantsSelected(item: TItem): boolean {
        if (item.children.length > 0) {
            for (var i = 0; i < item.children.length; i++) {
                var sub = item.children[i];
                if (!sub.isSelected) {
                    return false;
                }

                if (!this.allDescendantsSelected(sub as TItem)) {
                    return false;
                }
            }
        }
        return true;
    }

    protected getDelimited() {
        return !!!!(this.options as any)['delimited'];
    }

    protected anyDescendantsSelected(item: TItem): boolean {
        if (item.children.length > 0) {
            for (var i = 0; i < item.children.length; i++) {
                var sub = item.children[i];
                if (sub.isSelected) {
                    return true;
                }
                if (this.anyDescendantsSelected(sub as TItem)) {
                    return true;
                }
            }
        }
        return false;
    }

    protected getColumns(): Column[] {
        var self = this;
        var columns: Column[] = [];
        columns.push({
            field: 'text', name: 'Kayıt', width: 80, format: SlickFormatting.treeToggle(function () {
                return self.view;
            }, function (x) {
                return x.id;
            }, ctx => {
                var cls = 'check-box';
                var item = ctx.item;
                if (item.hideCheckBox) {
                    return this.getItemText(ctx);
                }
                var threeState = this.isThreeStateHierarchy();
                if (item.isSelected) {
                    if (threeState && !item.isAllDescendantsSelected) {
                        cls += ' partial';
                    }
                    else {
                        cls += ' checked';
                    }
                }
                if (this._readOnly)
                    cls += ' readonly';
                return '<span class="' + cls + '"></span>' + this.getItemText(ctx);
            })
        });
        return columns;
    }

    protected getItemText(ctx: FormatterContext): string {
        return htmlEncode(ctx.value);
    }

    protected getSlickOptions(): GridOptions {
        var opt = super.getSlickOptions();
        opt.forceFitColumns = true;
        return opt;
    }

    protected sortItems(): void {
        if (!this.moveSelectedUp()) {
            return;
        }
        var oldIndexes: Record<string, number> = {};
        var list = this.view.getItems();
        var i = 0;
        for (var $t1 = 0; $t1 < list.length; $t1++) {
            var x = list[$t1];
            oldIndexes[x.id] = i++;
        }
        list.sort(function (x1, y) {
            if (x1.isSelected && !y.isSelected) {
                return -1;
            }
            if (y.isSelected && !x1.isSelected) {
                return 1;
            }
            var c = Culture.stringCompare(x1.text, y.text);
            if (c !== 0) {
                return c;
            }
            return oldIndexes[x1.id] < oldIndexes[y.id] ? -1 : (oldIndexes[x1.id] > oldIndexes[y.id] ? 1 : 0);
        });

        this.view.setItems(list, true);
    }

    protected moveSelectedUp(): boolean {
        return false;
    }

    private _readOnly: boolean;

    public get_readOnly() {
        return this._readOnly;
    }

    public set_readOnly(value: boolean) {
        if (!!this._readOnly != !!value) {
            this._readOnly = !!value;
            this.view.refresh();
        }
    }

    private get_value(): string[] {
        var list = [];
        var items = this.view.getItems();
        for (var i = 0; i < items.length; i++) {
            if (items[i].isSelected) {
                list.push(items[i].id);
            }
        }
        return list;
    }

    public get value() {
        return this.get_value();
    }

    private set_value(value: string | string[]) {

        var selected: Record<string, boolean> = {};
        if (value != null) {
            if (typeof value == "string") {
                value = value.split(',')
                    .map(x => trimToNull(x))
                    .filter(x => x != null);
            }

            for (var i = 0; i < value.length; i++) {
                selected[value[i]] = true;
            }
        }

        this.view.beginUpdate();
        try {
            var items = this.view.getItems();
            for (var i1 = 0; i1 < items.length; i1++) {
                var item = items[i1];
                var select = selected[item.id];
                if (select !== item.isSelected) {
                    item.isSelected = select;
                    this.view.updateItem(item.id, item);
                }
            }
            this.updateSelectAll();
            this.updateFlags();
            this.sortItems();
        }
        finally {
            this.view.endUpdate();
        }
    }

    public set value(v: string[]) {
        this.set_value(v);
    }
}

export interface CheckLookupEditorOptions {
    lookupKey?: string;
    checkedOnTop?: boolean;
    showSelectAll?: boolean;
    hideSearch?: boolean;
    delimited?: boolean;
    cascadeFrom?: string;
    cascadeField?: string;
    cascadeValue?: any;
    filterField?: string;
    filterValue?: any;
}

@Decorators.registerEditor("Serenity.CheckLookupEditor")
export class CheckLookupEditor<TItem = any> extends CheckTreeEditor<CheckTreeItem<TItem>, CheckLookupEditorOptions> {

    private searchText: string;
    private enableUpdateItems: boolean;

    constructor(div: JQuery, options: CheckLookupEditorOptions) {
        super(div, options);

        this.enableUpdateItems = true;
        this.setCascadeFrom(this.options.cascadeFrom);
        this.updateItems();
        ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName,
            () => this.updateItems());
    }

    protected updateItems() {
        if (this.enableUpdateItems)
            super.updateItems();
    }

    protected getLookupKey() {
        return this.options.lookupKey;
    }

    protected getButtons(): ToolButton[] {
        return super.getButtons() ?? (this.options.hideSearch ? null : []);
    }

    protected createToolbarExtensions() {
        super.createToolbarExtensions();

        GridUtils.addQuickSearchInputCustom(this.toolbar.element, (field, text) => {
            this.searchText = Select2.util.stripDiacritics(text || '').toUpperCase();
            this.view.setItems(this.view.getItems(), true);
        });
    }

    protected getSelectAllText(): string {
        if (!this.options.showSelectAll)
            return null;

        return super.getSelectAllText();
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

    protected getLookupItems(lookup: Lookup<TItem>): TItem[] {
        return this.filterItems(this.cascadeItems(lookup.items));
    }

    protected getTreeItems() {
        var lookup = getLookup<TItem>(this.options.lookupKey);
        var items = this.getLookupItems(lookup);
        return items.map(item => <CheckTreeItem<TItem>>{
            id: ((item as any)[lookup.idField] ?? "").toString(),
            text: ((item as any)[lookup.textField] ?? "").toString(),
            source: item
        });
    }

    protected onViewFilter(item: CheckTreeItem<TItem>) {
        return super.onViewFilter(item) &&
            (isEmptyOrNull(this.searchText) ||
                Select2.util.stripDiacritics(item.text || '')
                    .toUpperCase().indexOf(this.searchText) >= 0);
    }

    protected moveSelectedUp(): boolean {
        return this.options.checkedOnTop;
    }

    protected get_cascadeFrom(): string {
        return this.options.cascadeFrom;
    }

    get cascadeFrom(): string {
        return this.get_cascadeFrom();
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
            this.options.cascadeFrom = null;
            return;
        }

        this.cascadeLink = new CascadedWidgetLink<Widget<any>>(Widget, this, p => {
            this.set_cascadeValue(this.getCascadeFromValue(p));
        });

        this.cascadeLink.set_parentID(value);
        this.options.cascadeFrom = value;
    }

    protected set_cascadeFrom(value: string) {
        if (value !== this.options.cascadeFrom) {
            this.setCascadeFrom(value);
            this.updateItems();
        }
    }

    set cascadeFrom(value: string) {
        this.set_cascadeFrom(value);
    }

    protected get_cascadeField() {
        return (this.options.cascadeField ?? this.options.cascadeFrom);
    }

    get cascadeField(): string {
        return this.get_cascadeField();
    }

    protected set_cascadeField(value: string) {
        this.options.cascadeField = value;
    }

    set cascadeField(value: string) {
        this.set_cascadeField(value);
    }

    protected get_cascadeValue(): any {
        return this.options.cascadeValue;
    }

    get cascadeValue(): any {
        return this.get_cascadeValue();
    }

    protected set_cascadeValue(value: any) {
        if (this.options.cascadeValue !== value) {
            this.options.cascadeValue = value;
            this.value = [];
            this.updateItems();
        }
    }

    set cascadeValue(value: any) {
        this.set_cascadeValue(value);
    }

    protected get_filterField() {
        return this.options.filterField;
    }

    get filterField(): string {
        return this.get_filterField();
    }

    protected set_filterField(value: string) {
        this.options.filterField = value;
    }

    set filterField(value: string) {
        this.set_filterField(value);
    }

    protected get_filterValue(): any {
        return this.options.filterValue;
    }

    get filterValue(): any {
        return this.get_filterValue();
    }

    protected set_filterValue(value: any) {
        if (this.options.filterValue !== value) {
            this.options.filterValue = value;
            this.value = null;
            this.updateItems();
        }
    }

    set filterValue(value: any) {
        this.set_filterValue(value);
    }
}