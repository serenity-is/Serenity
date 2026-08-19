import { nsSerenity } from "../../base";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

/**
 * An editor that renders a select of items from a static list.
 * @typeParam P - Widget props type.
 */
export class SelectEditor<P extends SelectEditorOptions = SelectEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates a select editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    /**
     * Returns the items to display in the editor.
     * @returns The list of items.
     */
    getItems() {
        return this.options.items || [];
    }

    /**
     * Returns the text for the empty option.
     * @returns The empty option text.
     */
    protected override emptyItemText() {
        if (this.options.emptyOptionText) {
            return this.options.emptyOptionText;
        }
        return super.emptyItemText();
    }

    /**
     * Loads the configured items into the editor.
     */
    override updateItems() {
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

/**
 * Options for the {@link SelectEditor}.
 */
export interface SelectEditorOptions extends ComboboxCommonOptions {
    /** Items to display; each is a value or a [value, text] pair. */
    items?: any[];
    /** Text for the empty option. */
    emptyOptionText?: string;
}