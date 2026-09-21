import { Enum, EnumKeyAttribute, getCustomAttribute, getTypeFullName, isPromiseLike, localText, nsSerenity } from "../../base";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";
import { ComboboxItem } from "./combobox";
import { ComboboxCommonOptions, ComboboxEditor } from "./comboboxeditor";
import { EditorProps } from "./editorwidget";

/**
 * Options for the {@link EnumEditor}.
 */
export interface EnumEditorOptions extends ComboboxCommonOptions {
    /** Key of the enum to load items from. */
    enumKey?: string;
    /** The enum type to load items from. */
    enumType?: any;
}

/**
 * An editor that renders a select of enum values.
 * @typeParam P - Widget props type.
 */
export class EnumEditor<P extends EnumEditorOptions = EnumEditorOptions> extends ComboboxEditor<P, ComboboxItem> {
    static override[Symbol.typeInfo] = this.registerEditor(nsSerenity);

    /**
     * Creates an enum editor.
     * @param props - Widget props.
     */
    constructor(props: EditorProps<P>) {
        super(props);

        this.updateItems();
    }

    /**
     * Loads the enum values into the editor.
     * @returns Void or a promise that resolves when items are loaded.
     */
    protected override updateItems(): void | PromiseLike<void> {
        this.clearItems();

        const enumType = this.options.enumType || EnumTypeRegistry.getOrLoad(this.options.enumKey);

        const then = (enumType: any) => {
            if (enumType == null)
                return;

            let enumKey = this.options.enumKey;

            if (enumKey == null && enumType != null) {
                enumKey = getCustomAttribute(enumType, EnumKeyAttribute, false)?.value ??
                    getTypeFullName(enumType);
            }

            const values = Enum.getValues(enumType);
            for (const x of values) {
                const name = Enum.toString(enumType, x);
                this.addOption(String(x),
                    localText("Enums." + enumKey + "." + name, name), null, false);
            }
        }

        if (isPromiseLike(enumType))
            return enumType.then(then);
        else
            then(enumType);
    }

    /**
     * Whether the editor allows clearing the selection.
     * @returns True when clear is allowed.
     */
    protected override allowClear() {
        return (this.options.allowClear ?? true);
    }
}