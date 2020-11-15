namespace Serenity {

    export interface LookupEditorOptions extends Select2EditorOptions {
        lookupKey?: string;
        async?: boolean;
    }

    @Serenity.Decorators.registerEditor("Serenity.LookupEditorBase")
    export class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {

        constructor(input: JQuery, opt?: TOptions) {
            super(input, opt);

            if (!this.hasAsyncSource()) {
                this.updateItems();
                var self = this;
                Q.ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, function () {
                    self.updateItems();
                });
            }
        }

        hasAsyncSource(): boolean {
            return !!this.options.async;
        }

        destroy(): void {
            if (!this.hasAsyncSource())
                Q.ScriptData.unbindFromChange(this.uniqueName);

            super.destroy();
        }

        protected getLookupKey(): string {
            if (this.options.lookupKey != null) {
                return this.options.lookupKey;
            }

            var key = Q.getTypeFullName(Q.getInstanceType(this));

            var idx = key.indexOf('.');
            if (idx >= 0) {
                key = key.substring(idx + 1);
            }

            if (Q.endsWith(key, 'Editor')) {
                key = key.substr(0, key.length - 6);
            }

            return key;
        }

        protected lookup: Q.Lookup<TItem>;

        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>> {
            return Q.getLookupAsync<TItem>(this.getLookupKey());
        }

        protected getLookup(): Q.Lookup<TItem> {
            return Q.getLookup<TItem>(this.getLookupKey());
        }

        protected getItems(lookup: Q.Lookup<TItem>) {
            return this.filterItems(this.cascadeItems(lookup.items));
        }

        protected getIdField() {
            return this.lookup != null ? this.lookup.idField : super.getIdField();
        }

        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>) {
            if (lookup == null)
                return super.itemText(item);

            var textValue = lookup.textFormatter ? lookup.textFormatter(item) : item[lookup.textField];
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

        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>) {
            return super.itemDisabled(item);
        }

        public updateItems() {
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
            entity[this.getLookup().textField] = term;
        }

        protected editDialogDataChange() {
            Q.reloadLookup(this.getLookupKey());
        }
    }

    @Decorators.registerEditor('Serenity.LookupEditor')
    export class LookupEditor extends LookupEditorBase<LookupEditorOptions, any> {
        constructor(hidden: JQuery, opt?: LookupEditorOptions) {
            super(hidden, opt);
        }
    }
}