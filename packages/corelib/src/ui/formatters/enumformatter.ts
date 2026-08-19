import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { Enum, EnumKeyAttribute, formatterTypeInfo, getCustomAttribute, getTypeFullName, htmlEncode, isPromiseLike, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";

/** Renders enum values as localized text via `Enums.<EnumKey>.<Name>`. */
export class EnumFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * @param props.enumKey - Full enum key (e.g. `"MyProject.MyEnum"`). Resolved via {@link EnumTypeRegistry}.
     */
    constructor(public readonly props: { enumKey?: string } = {}) {
        this.props ??= {};
    }

    /** @param ctx - Formatter context containing enum value. @returns Localized enum text (or async placeholder). */
    format(ctx: FormatterContext): FormatterResult {
        var enumType = EnumTypeRegistry.getOrLoad(this.enumKey);
        if (isPromiseLike(enumType)) {
            const node = document.createElement("span");
            enumType.then(resolved => {
                const text = new Text(EnumFormatter.format(resolved, ctx.value));
                node.parentElement && node.replaceWith(text);
            });
            return node;
        }
        return EnumFormatter.format(enumType, ctx.value);
    }

    get enumKey() { return this.props.enumKey; }
    set enumKey(value: string) { this.props.enumKey = value; }

    /**
     * Formats an enum value given an enum type.
     * @param enumType - Registered enum object.
     * @param value - Enum numeric value.
     * @returns Localized display text.
     */
    static format(enumType: any, value: any) {

        if (value == null) {
            return '';
        }

        var name = Enum.toString(enumType, value);
        var enumKey = getCustomAttribute(enumType, EnumKeyAttribute, false)?.value ??
            getTypeFullName(enumType);
        return EnumFormatter.getText(enumKey, name);
    }

    /**
     * Gets localized text for an enum name.
     * @param enumKey - Enum key (e.g. `"MyEnum"`).
     * @param name - Member name.
     * @returns Localized string (falls back to name).
     */
    static getText(enumKey: string, name: string) {
        if (!name)
            return '';

        return htmlEncode(localText("Enums." + enumKey + '.' + name, name));
    }

    /**
     * Gets the member name for a value.
     * @param enumType - Enum object.
     * @param value - Numeric value.
     * @returns Enum member name or empty string.
     */
    static getName(enumType: any, value: any) {
        if (value == null) {
            return '';
        }
        return Enum.toString(enumType, value);
    }
}
