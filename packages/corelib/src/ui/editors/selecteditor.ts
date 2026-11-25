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
     * Replaces the items of the editor, e.g. when they depend on the value of another field. The value is kept while
     * it is one of the new items, and cleared otherwise.
     * @param items - The items, as for {@link SelectEditorOptions.items}.
     */
    setItems(items: any[]) {
        const value = this.value;
        this.options.items = items;
        this.updateItems();
        if (value)
            this.value = this.itemById[value] ? value : null;
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
        const items = this.getItems();
        this.clearItems();

        for (const item of items) {
            if (item == null)
                continue;

            // an item of the editor is a value, a [value, text] pair, or an object with the fields of a combobox item,
            // e.g. one that a form field gives, see SelectItemAttribute; an object gives its text, and the text of its
            // key when it has one, see textKeyField, without a fallback to its value, which the one that gives the items
            // is to make, e.g. as the text of an item that has no key
            if (typeof item === "string") {
                this.addOption(item, item, item, false);
                continue;
            }

            if (Array.isArray(item)) {
                this.addOption(item[0], item[1] ?? item[0], item, false);
                continue;
            }

            const obj = item as ComboboxItem;
            const id = obj.id == null ? "" : String(obj.id);
            this.addItem({
                id: id,
                text: this.localizeItemText(obj.text, item),
                disabled: !!obj.disabled,
                icon: obj.icon,
                cssClass: obj.cssClass,
                source: item
            });
        }
    }
}

/**
 * Options for the {@link SelectEditor}.
 */
export interface SelectEditorOptions extends ComboboxCommonOptions {
    /**
     * Items to display; each is a value, a [value, text] pair, or an object with the fields of a combobox item, e.g.
     * `{ id, text, textKey, icon }`, whose text key is localized when the editor gives its field, see
     * {@link ComboboxCommonOptions.textKeyField}.
     */
    items?: any[];
    /** Text for the empty option. */
    emptyOptionText?: string;
}