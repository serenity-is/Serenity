namespace Serenity {

    export interface LookupEditorOptions {
        lookupKey?: string;
        minimumResultsForSearch?: any;
        autoComplete?: boolean;
        inplaceAdd?: boolean;
        inplaceAddPermission?: string;
        dialogType?: string;
        cascadeFrom?: string;
        cascadeField?: string;
        cascadeValue?: any;
        filterField?: string;
        filterValue?: any;
        multiple?: boolean;
        delimited?: boolean;
    }

    @Serenity.Decorators.registerEditor("Serenity.LookupEditorBase")
    export class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {

        constructor(input: JQuery, opt?: TOptions) {
            super(input, opt);

            this.setCascadeFrom(this.options.cascadeFrom);

            var self = this;
            if (!this.isAsyncWidget()) {
                this.updateItems();
                Q.ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, function () {
                    self.updateItems();
                });
            }

            if (!this.options.autoComplete &&
                this.options.inplaceAdd &&
                (this.options.inplaceAddPermission == null ||
                    Q.Authorization.hasPermission(this.options.inplaceAddPermission))) {
                this.addInplaceCreate(Q.text('Controls.SelectEditor.InplaceAdd'), null);
            }
        }

        protected initializeAsync(): PromiseLike<void> {
            return this.updateItemsAsync().then(() => {
                Q.ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, () => {
                    this.updateItemsAsync();
                });
            }, null);
        }

        destroy(): void {
            Q.ScriptData.unbindFromChange(this.uniqueName);
            Serenity.Select2Editor.prototype.destroy.call(this);
        }

        protected getLookupKey(): string {
            if (this.options.lookupKey != null) {
                return this.options.lookupKey;
            }

            var key = (ss as any).getTypeFullName((ss as any).getInstanceType(this));

            var idx = key.indexOf('.');
            if (idx >= 0) {
                key = key.substring(idx + 1);
            }

            if (Q.endsWith(key, 'Editor')) {
                key = key.substr(0, key.length - 6);
            }

            return key;
        }

        protected getLookup(): Q.Lookup<TItem> {
            return Q.getLookup<TItem>(this.getLookupKey());
        }

        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>> {
            return Promise.resolve().then(() => {
                var key = this.getLookupKey();
                return Q.getLookupAsync<TItem>(key);
            }, null);
        }

        protected getItems(lookup: Q.Lookup<TItem>) {
            return this.filterItems(this.cascadeItems(lookup.items));
        }

        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>) {
            var textValue = lookup.textFormatter ? lookup.textFormatter(item) : item[lookup.textField];
            return textValue == null ? '' : textValue.toString();
        }

        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>) {
            return false;
        }

        public updateItems() {
            var lookup = this.getLookup();
            this.clearItems();
            var items = this.getItems(lookup);
            for (var $t1 = 0; $t1 < items.length; $t1++) {
                var item = items[$t1];
                var text = this.getItemText(item, lookup);
                var disabled = this.getItemDisabled(item, lookup);
                var idValue = item[lookup.idField];
                var id = (idValue == null ? '' : idValue.toString());
                this.addItem({
                    id: id,
                    text: text,
                    source: item,
                    disabled: disabled
                });
            }
        }

        public updateItemsAsync(): PromiseLike<void> {
            return this.getLookupAsync().then((lookup) => {
                this.clearItems();
                var items = this.getItems(lookup);
                for (var $t1 = 0; $t1 < items.length; $t1++) {
                    var item = items[$t1];
                    var text = this.getItemText(item, lookup);
                    var disabled = this.getItemDisabled(item, lookup);
                    var idValue = item[lookup.idField];
                    var id = (idValue == null ? '' : idValue.toString());
                    this.addItem({ id: id, text: text, source: item, disabled: disabled });
                }
            }, null);
        }

        protected getDialogTypeKey() {
            if (this.options.dialogType != null) {
                return this.options.dialogType;
            }

            return this.getLookupKey();
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

        protected inplaceCreateClick(e: JQueryEventObject) {

            if (this.get_readOnly() &&
                ((this.multiple && !e['editItem']) || !this.value))
                return;

            var self = this;
            this.createEditDialog(dialog => {

                // an ugly workaround
                if (this.get_readOnly() &&
                    (dialog as any).element)
                    (dialog as any).element
                        .find('.tool-button.delete-button')
                        .addClass('disabled')
                        .unbind('click');

                Serenity.SubDialogHelper.bindToDataChange(dialog, this, (x, dci) => {
                    Q.reloadLookup(this.getLookupKey());
                    self.updateItems();
                    this.lastCreateTerm = null;

                    if ((dci.type === 'create' || dci.type === 'update') &&
                        dci.entityId != null) {
                        var id = dci.entityId.toString();

                        if (this.multiple) {
                            var values = self.get_values().slice();
                            if (values.indexOf(id) < 0) {
                                values.push(id);
                            }
                            self.set_values(null);
                            self.set_values(values.slice());
                        }
                        else {
                            self.set_value(null);
                            self.set_value(id);
                        }
                    }
                    else if (this.multiple && dci.type === 'delete' &&
                        dci.entityId != null) {
                        var id1 = dci.entityId.toString();
                        var values1 = self.get_values().slice();

                        var idx1 = values1.indexOf(id1);
                        if (idx1 >= 0)
                            values1.splice(idx1, 1);

                        self.set_values(values1.slice());
                    }
                    else if (!this.multiple) {
                        self.set_value(null);
                    }
                }, true);

                var editItem = e['editItem'];
                if (editItem != null) {
                    dialog.load(editItem, () => {
                        (dialog as any).dialogOpen(this.openDialogAsPanel);
                    }, null);
                }
                else if (this.multiple || Q.isEmptyOrNull(this.get_value())) {
                    var entity: TItem = {} as any;
                    entity[this.getLookup().textField] = Q.trimToEmpty(this.lastCreateTerm);
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
                this.options.cascadeFrom = null;
                return;
            }

            this.cascadeLink = new Serenity.CascadedWidgetLink<Widget<any>>(Widget, this, p => {
                this.set_cascadeValue(this.getCascadeFromValue(p));
            });

            this.cascadeLink.set_parentID(value);
            this.options.cascadeFrom = value;
        }

        protected isAutoComplete() {
            return this.options != null && this.options.autoComplete;
        }

        protected getSelect2Options() {
            var opt = super.getSelect2Options();

            if (this.options.minimumResultsForSearch != null)
                opt.minimumResultsForSearch = this.options.minimumResultsForSearch;

            if (this.options.autoComplete)
                opt.createSearchChoice = this.getCreateSearchChoice(null);
            else if (this.options.inplaceAdd && (this.options.inplaceAddPermission == null ||
                Q.Authorization.hasPermission(this.options.inplaceAddPermission))) {
                opt.createSearchChoice = this.getCreateSearchChoice(null);
            }

            if (this.options.multiple)
                opt.multiple = true;

            opt.allowClear = Q.coalesce((this.options as any).allowClear, true);
            return opt;
        }

        protected get_cascadeFrom(): string {
            return this.options.cascadeFrom;
        }

        get cascadeFrom(): string {
            return this.get_cascadeFrom();
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
            return Q.coalesce(this.options.cascadeField, this.options.cascadeFrom);
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
                this.set_value(null);
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
                this.set_value(null);
                this.updateItems();
            }
        }

        set filterValue(value: any) {
            this.set_filterValue(value);
        }

        public openDialogAsPanel: boolean;
    }    

    @Decorators.registerEditor('Serenity.LookupEditor')
    export class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions) {
            super(hidden, opt);
        }
    }
}