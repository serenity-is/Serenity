namespace Serenity {

    export interface LookupEditorOptions extends Select2EditorOptions {
        lookupKey?: string;
        async?: boolean;
    }

    @Serenity.Decorators.registerEditor("Serenity.LookupEditorBase")
    export class LookupEditorBase<TOptions extends LookupEditorOptions, TItem> extends Select2Editor<TOptions, TItem> {

        constructor(input: JQuery, opt?: TOptions) {
            super(input, opt);

            var self = this;

            this.updateItems();
            Q.ScriptData.bindToChange('Lookup.' + this.getLookupKey(), this.uniqueName, function () {
                self.updateItems();
            });
        }

        destroy(): void {
            Q.ScriptData.unbindFromChange(this.uniqueName);
            super.destroy();
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

        protected getLookupAsync(): PromiseLike<Q.Lookup<TItem>> {
            return Q.getLookupAsync<TItem>(this.getLookupKey());
        }

        protected getLookup(): Q.Lookup<TItem> {
            return Q.getLookup<TItem>(this.getLookupKey());
        }

        protected getItems(lookup: Q.Lookup<TItem>) {
            return this.filterItems(this.cascadeItems(lookup.items));
        }

        protected getItemText(item: TItem, lookup: Q.Lookup<TItem>) {
            if (lookup == null)
                return super.itemText(item);

            var textValue = lookup.textFormatter ? lookup.textFormatter(item) : item[lookup.textField];
            return textValue == null ? '' : textValue.toString();
        }

        protected getItemDisabled(item: TItem, lookup: Q.Lookup<TItem>) {
            return super.itemDisabled(item);
        }

        public updateItems() {

            var updateItemsFor = (lookup: Q.Lookup<TItem>) => {
                this.clearItems();
                var items = this.getItems(lookup);
                for (var item of items) {
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

            if (this.options.async)
                this.getLookupAsync().then(updateItemsFor);
            else
                updateItemsFor(this.getLookup());
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