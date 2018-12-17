﻿
namespace Serenity {

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

    @Serenity.Decorators.registerClass('Serenity.Select2Editor',
        [Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly])
    @Serenity.Decorators.element("<input type=\"hidden\"/>")
    export class Select2Editor<TOptions, TItem> extends Widget<TOptions> implements
        Serenity.ISetEditValue, Serenity.IGetEditValue, Serenity.IStringValue, Serenity.IReadOnly {

        public items: Select2Item[];
        protected itemById: Q.Dictionary<Select2Item>
        protected pageSize: number = 100;
        protected lastCreateTerm: string;

        constructor(hidden: JQuery, opt?: any) {
            super(hidden, opt);

            this.items = [];
            this.itemById = {};
            var emptyItemText = this.emptyItemText();
            if (emptyItemText != null) {
                hidden.attr('placeholder', emptyItemText);
            }
            var select2Options = this.getSelect2Options();
            hidden.select2(select2Options);
            hidden.attr('type', 'text');

            // jquery validate to work
            hidden.bind('change.' + this.uniqueName, <any>function (e: JQueryEventObject, x: boolean) {
                if (!!(Serenity.WX.hasOriginalEvent(e) || !x)) {
                    if (Serenity.ValidationHelper.getValidator(hidden) != null) {
                        hidden.valid();
                    }
                }
            });

            this.setCascadeFrom((this.options as Select2EditorOptions).cascadeFrom);

            if (this.useInplaceAdd())
                this.addInplaceCreate(Q.text('Controls.SelectEditor.InplaceAdd'), null);
        }

        destroy() {
            if (this.element != null) {
                this.element.select2('destroy');
            }

			super.destroy();
        }

        protected emptyItemText() {
            return Q.coalesce(this.element.attr('placeholder'),
                Q.text('Controls.SelectEditor.EmptyItemText'));
        }

        protected allowClear() {
            return (this.options as Select2EditorOptions).allowClear != null ?
                !!(this.options as Select2EditorOptions).allowClear : this.emptyItemText() != null;
        }

        protected isMultiple() {
            return !!(this.options as Select2EditorOptions).multiple;
        }

        protected getSelect2Options(): Select2Options {
            var emptyItemText = this.emptyItemText();
            var opt: Select2Options = {
                data: this.items,
                multiple: this.isMultiple(),
                placeHolder: (!Q.isEmptyOrNull(emptyItemText) ? emptyItemText : null),
                allowClear: this.allowClear(),
                createSearchChoicePosition: 'bottom',
                query: (query) => {
                    var term = (Q.isEmptyOrNull(query.term) ? '' : Select2.util.stripDiacritics(
                        Q.coalesce(query.term, '')).toUpperCase());

                    var results = this.items.filter(function (item) {
                        return term == null || Q.startsWith(Select2.util.stripDiacritics(
                            Q.coalesce(item.text, '')).toUpperCase(), term);
                    });

                    results.push(...this.items.filter(item1 =>
                        term != null && !Q.startsWith(Select2.util.stripDiacritics(
                            Q.coalesce(item1.text, '')).toUpperCase(), term) &&
                        Select2.util.stripDiacritics(Q.coalesce(item1.text, ''))
                            .toUpperCase().indexOf(term) >= 0));

                    query.callback({
                        results: results.slice((query.page - 1) * this.pageSize, query.page * this.pageSize),
                        more: results.length >= query.page * this.pageSize
                    });
                },
                initSelection: (element, callback) => {
                    var val = element.val();
                    var isAutoComplete = this.isAutoComplete();
                    if (this.isMultiple()) {
                        var list = [];
                        var $t1 = val.split(',');
                        for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                            var z = $t1[$t2];
                            var item2 = this.itemById[z];
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
                    var it = this.itemById[val];
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

        public clearItems() {
            (ss as any).clear(this.items);
            this.itemById = {};
        }

        public addItem(item: Select2Item) {
            this.items.push(item);
            this.itemById[item.id] = item;
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
            addTitle = Q.coalesce(addTitle, Q.text('Controls.SelectEditor.InplaceAdd'));
            editTitle = Q.coalesce(editTitle, Q.text('Controls.SelectEditor.InplaceEdit'));
            var inplaceButton = $('<a><b/></a>')
                .addClass('inplace-button inplace-create')
                .attr('title', addTitle)
                .insertAfter(this.element).click(function (e) {
                    self.inplaceCreateClick(e);
                });

            this.get_select2Container().add(this.element).addClass('has-inplace-button');

            Serenity.WX.change(this, (e1: JQueryEventObject) => {
                var isNew = this.isMultiple() || Q.isEmptyOrNull(this.get_value());
                inplaceButton.attr('title', (isNew ? addTitle : editTitle)).toggleClass('edit', !isNew);
            });

            Serenity.WX.changeSelect2(this, (e2: JQueryEventObject) => {
                if (this.isMultiple()) {
                    var values = this.get_values();
                    if (values.length > 0 && values[values.length - 1] == (-2147483648).toString()) {
                        this.set_values(values.slice(0, values.length - 1));
                        this.inplaceCreateClick(e2);
                    }
                }
                else if (this.get_value() == (-2147483648).toString()) {
                    this.set_value(null);
                    this.inplaceCreateClick(e2);
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
                    e3['editItem'] = values1[index];
                    this.inplaceCreateClick(e3);
                });
            }
        }

        protected useInplaceAdd(): boolean {
            return !this.isAutoComplete() &&
                (this.options as Select2EditorOptions).inplaceAdd &&
                ((this.options as Select2EditorOptions).inplaceAddPermission == null ||
                    Q.Authorization.hasPermission((this.options as Select2EditorOptions).inplaceAddPermission));
        }

        protected isAutoComplete(): boolean {
            return !!(this.options as Select2EditorOptions).autoComplete;
        }

        public getCreateSearchChoice(getName: (z: any) => string) {
            return (s: string) => {

                this.lastCreateTerm = s;
                s = Q.coalesce(Select2.util.stripDiacritics(s), '').toLowerCase();

                if (Q.isTrimmedEmpty(s)) {
                    return null;
                }

                if (Q.any(this.get_items(), (x: Select2Item) => {
                        var text = getName ? getName(x.source) : x.text;
                        return Select2.util.stripDiacritics(Q.coalesce(text, '')).toLowerCase() == s;
                }))
                    return null;

                if (!Q.any(this.get_items(), x1 => {
                    return Q.coalesce(Select2.util.stripDiacritics(x1.text), '').toLowerCase().indexOf(s) !== -1;
                })) {
                    if (this.isAutoComplete()) {
                        return {
                            id: this.lastCreateTerm,
                            text: this.lastCreateTerm
                        };
                    }

                    return {
                        id: (-2147483648).toString(),
                        text: Q.text('Controls.SelectEditor.NoResultsClickToDefine')
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
                    text: Q.text('Controls.SelectEditor.ClickToDefine')
                };
            }
        }

        setEditValue(source: any, property: PropertyItem) {
            var val = source[property.name];
            if(Q.isArray(val)) {
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

        protected get_select2Container() {
            return this.element.prevAll('.select2-container');
        }

        protected get_items() {
            return this.items;
        }

        protected get_itemByKey() {
            return this.itemById;
        }

        get_value() {
            var val;
            if (this.element.data('select2')) {
                val = this.element.select2('val');
                if (val != null && Q.isArray(val)) {
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
                if (!Q.isEmptyOrNull(value) && this.isMultiple()) {
                    val = value.split(String.fromCharCode(44)).map(function (x) {
                        return Q.trimToNull(x);
                    }).filter(function (x1) {
                        return x1 != null;
                    });
                }

                this.element.select2('val', val)
                    .triggerHandler('change', [true]);

                this.updateInplaceReadOnly();
            }
        }

        set value(v: string) {
            this.set_value(v);
        }

        protected get_values(): string[] {

            var val = this.element.select2('val');
            if (val == null) {
                return [];
            }

			if (Q.isArray(val)) {
                return val;
            }

            var str = val;
            if (Q.isEmptyOrNull(str)) {
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
            return Q.coalesce(this.element.select2('data'), {}).text;
        }

        get text(): string {
            return this.get_text();
        }

        get_readOnly(): boolean {
            return !Q.isEmptyOrNull(this.element.attr('readonly'));
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
                Serenity.EditorUtils.setReadonly(this.element, value);
                this.updateInplaceReadOnly();
            }
        }

        set readOnly(value: boolean) {
            this.set_readOnly(value);
        }

        protected getCascadeFromValue(parent: Serenity.Widget<any>) {
            return Serenity.EditorUtils.getValue(parent);
        }

        protected cascadeLink: Serenity.CascadedWidgetLink<Widget<any>>;

        protected setCascadeFrom(value: string) {

            if (Q.isEmptyOrNull(value)) {
                if (this.cascadeLink != null) {
                    this.cascadeLink.set_parentID(null);
                    this.cascadeLink = null;
                }
                (this.options as Select2EditorOptions).cascadeFrom = null;
                return;
            }

            this.cascadeLink = new Serenity.CascadedWidgetLink<Widget<any>>(Widget, this, p => {
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
            return Q.coalesce((this.options as Select2EditorOptions).cascadeField, (this.options as Select2EditorOptions).cascadeFrom);
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

                if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                    return [];
                }

                return items;
            }

            var key = val.toString();
            var fld = this.get_cascadeField();

            return items.filter(x => {
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
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
                var itemKey = Q.coalesce(x[fld], Serenity.ReflectionUtils.getPropertyValue(x, fld));
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
            var dialogType = Serenity.DialogTypeRegistry.get(dialogTypeKey);
            Serenity.Widget.create({
                type: dialogType,
                init: x => callback(x as any)
            });
        }

        public onInitNewEntity: (entity: TItem) => void;

        protected initNewEntity(entity: TItem) {
            if (!Q.isEmptyOrNull(this.get_cascadeField())) {
                entity[this.get_cascadeField()] = this.get_cascadeValue();
            }

            if (!Q.isEmptyOrNull(this.get_filterField())) {
                entity[this.get_filterField()] = this.get_filterValue();
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
                ((this.isMultiple() && !e['editItem']) || !this.value))
                return;

            this.createEditDialog(dialog => {

                if (this.get_readOnly())
                    this.setEditDialogReadOnly(dialog);

                Serenity.SubDialogHelper.bindToDataChange(dialog, this, (x, dci) => {
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

                var editItem = e['editItem'];
                if (editItem != null) {
                    dialog.load(editItem, () => {
                        (dialog as any).dialogOpen(this.openDialogAsPanel);
                    }, null);
                }
                else if (this.isMultiple() || Q.isEmptyOrNull(this.get_value())) {
                    var entity: TItem = {} as any;
                    this.setTermOnNewEntity(entity, Q.trimToEmpty(this.lastCreateTerm));
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

    declare namespace Select2Extensions {
        function select2(element: JQuery): JQuery;
        function select2(element: JQuery, options: Select2Options): JQuery;
        function select2(element: JQuery, action: string): JQuery;
        function select2(element: JQuery, option: string, value: any): JQuery;
        function select2(element: JQuery, option: string): any;
    }

    export interface Select2Item {
        id: string;
        text: string;
        source?: any;
        disabled?: boolean;
    }

    @Serenity.Decorators.registerClass('Serenity.SelectEditor')
    export class SelectEditor extends Select2Editor<SelectEditorOptions, Select2Item> {
        constructor(hidden: JQuery, opt?: SelectEditorOptions) {
            super(hidden, opt);
            this.updateItems();
        }

        getItems() {
            return this.options.items || [];
        }

        protected emptyItemText() {
            if (!Q.isEmptyOrNull(this.options.emptyOptionText)) {
                return this.options.emptyOptionText;
            }
            return super.emptyItemText();
        }

        updateItems() {
            var items = this.getItems();
            this.clearItems();

            if (items.length > 0) {
                var isStrings = typeof (items[0]) === 'string';
                for (var item of items) {
                    var key = isStrings ? item : item[0];
                    var text = isStrings ? item : Q.coalesce(item[1], item[0]);
                    this.addOption(key, text, item, false);
                }
            }
        }
    }

    export interface SelectEditorOptions extends Select2CommonOptions {
        items?: any[];
        emptyOptionText?: string;
    }
}