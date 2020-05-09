
namespace Serenity {

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