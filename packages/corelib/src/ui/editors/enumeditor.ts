import { Enum, EnumKeyAttribute, getCustomAttribute, getTypeFullName, IconClassName, isPromiseLike, localText, nsSerenity } from "../../base";
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
     * Maps an enum value to a combobox item.
     * @param value The enum value.
     * @param opt The enum key and type.
     * @returns The combobox item representing the enum value.
     */
    protected mapEnum(value: number, opt: { enumKey: string, enumType: any }): ComboboxItem<any> {
        const name = Enum.toString(opt.enumType, value);
        return {
            id: String(value),
            text: localText("Enums." + opt.enumKey + "." + name, name),
            icon: this.mapIcon(value, opt)
        }
    }

    /**
     * Maps an enum value to an icon class name.
     * @param value The enum value.
     * @param opt The enum key and type.
     * @returns The icon class name representing the enum value, or undefined if none.
     */
    protected mapIcon(value: number, opt: { enumKey: string, enumType: any }): IconClassName | undefined {
        return Enum.getIconClass(opt.enumType, value);
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
            for (const value of values) {
                const item = this.mapEnum(value, { enumKey, enumType });
                this.addItem(item);
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