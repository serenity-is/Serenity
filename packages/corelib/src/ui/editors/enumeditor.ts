import { Enum, EnumKeyAttribute, getCustomAttribute, getTypeFullName, IconClassName, isPromiseLike, localText, nsSerenity, peekTypeInfo, PropertyItem } from "../../base";
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
    /**
     * Whether the enum is a set of flags, like a [Flags] enum on the server side. Along with the multiple option, the value is a
     * number, the members selected OR-ed together, or null for none, and the members combining others, like SourceAdapter of the
     * list request capabilities, are offered too, while none (zero) is not. Without multiple, a single member is selected as usual.
     * When it is not set, it is what the enum is registered with, see the enumFlags option of registerEnum.
     */
    flags?: boolean;
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
     * Whether the enum is a set of flags, by the flags option, or else by the registration of the enum, when it is already
     * loaded.
     * @returns True for a set of flags.
     */
    protected isFlags(): boolean {
        if (this.options.flags != null)
            return !!this.options.flags;

        const enumType = this.options.enumType ?? (this.options.enumKey ? EnumTypeRegistry.tryGet(this.options.enumKey) : null);
        return !!(enumType && !isPromiseLike(enumType) && peekTypeInfo(enumType)?.enumFlags);
    }

    /**
     * Whether several members of a set of flags are selected, whose value is the number they make together.
     * @returns True for a set of flags with the multiple option.
     */
    protected isMultipleFlags(): boolean {
        return this.isMultiple() && this.isFlags();
    }

    /**
     * Maps an enum value to a combobox item.
     * @param value The enum value.
     * @param opt The enum key and type.
     * @returns The combobox item representing the enum value.
     */
    protected mapEnum(value: number | string, opt: { enumKey: string, enumType: any }): ComboboxItem<any> {
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
    protected mapIcon(value: number | string, opt: { enumKey: string, enumType: any }): IconClassName | undefined {
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

            const flags = this.isMultipleFlags();
            const values = Enum.getValues(enumType);
            for (const value of values) {
                // none of a set of flags is selecting none of its members, while the ones combining others, like
                // SourceAdapter, are offered as shortcuts
                if (flags && !(typeof value === "number" && value > 0))
                    continue;

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
     * Gets or sets the value of a set of flags with the multiple option, which is the members selected OR-ed together, or null when
     * none is selected. Setting it selects the fewest members that make it: the ones combining others first, the larger first, as
     * long as all their bits are in the value and some are not covered yet, and then the single bits left.
     */
    get flagsValue(): number | null {
        const values = this.get_values() ?? [];
        return values.length ? values.reduce((sum, value) => sum | Number(value), 0) : null;
    }

    set flagsValue(value: number | string | null) {
        const flags = value == null || value === "" ? 0 : Number(value);
        const members = Object.keys(this.itemById ?? {}).map(Number)
            .filter(member => member > 0 && (member & flags) === member)
            .sort((a, b) => bitCount(b) - bitCount(a) || b - a);

        let covered = 0;
        const selected = new Set<string>();
        for (const member of members) {
            if ((member & ~covered) !== 0) {
                selected.add(String(member));
                covered |= member;
            }
        }

        // in the order of the members, not the one they were picked in
        this.set_values(Object.keys(this.itemById ?? {}).filter(id => selected.has(id)));
    }

    /**
     * Sets the edit value from a source object, which is a number for several members of a set of flags.
     * @param source - The source object.
     * @param property - The property item.
     */
    override setEditValue(source: any, property: PropertyItem) {
        if (this.isMultipleFlags())
            this.flagsValue = source[property.name];
        else
            super.setEditValue(source, property);
    }

    /**
     * Gets the edit value into a target object, which is a number for several members of a set of flags.
     * @param property - The property item.
     * @param target - The target object.
     */
    override getEditValue(property: PropertyItem, target: any) {
        if (this.isMultipleFlags())
            target[property.name] = this.flagsValue;
        else
            super.getEditValue(property, target);
    }

    /**
     * Whether the editor allows clearing the selection.
     * @returns True when clear is allowed.
     */
    protected override allowClear() {
        return (this.options.allowClear ?? true);
    }
}

/** The number of bits a member of a set of flags has, which is more than one for one combining others. */
function bitCount(value: number): number {
    let count = 0;
    for (; value; value &= value - 1)
        count++;
    return count;
}
