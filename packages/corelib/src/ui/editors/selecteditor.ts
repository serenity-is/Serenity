import { Decorators } from "../../types/decorators";
import { EditorProps } from "../widgets/widget";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";

@Decorators.registerClass('Serenity.SelectEditor')
export class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    getItems() {
        return this.options.items || [];
    }

    protected emptyItemText() {
        if (this.options.emptyOptionText) {
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
                var text = isStrings ? item : (item[1] ?? item[0]);
                this.addOption(key, text, item, false);
            }
        }
    }
}

export interface SelectEditorOptions extends ComboboxCommonOptions {
    items?: any[];
    emptyOptionText?: string;
}