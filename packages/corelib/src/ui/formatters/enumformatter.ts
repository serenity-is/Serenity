import { FormatterContext } from "@serenity-is/sleekgrid";
import { Enum, formatterTypeInfo, getCustomAttribute, getTypeFullName, htmlEncode, isPromiseLike, localText, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";
import { EnumKeyAttribute } from "../../types/attributes";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";

export class EnumFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public readonly props: { enumKey?: string } = {}) {
        this.props ??= {};
    }

    format(ctx: FormatterContext): string | Element {
        var enumType = EnumTypeRegistry.getOrLoad(this.enumKey);
        if (isPromiseLike(enumType)) {
            const node = document.createElement("span");
            enumType.then(() => {
                const text = new Text(EnumFormatter.format(enumType, ctx.value));
                node.parentElement && node.replaceWith(text);
            });
            return node;
        }
        return EnumFormatter.format(enumType, ctx.value);
    }

    get enumKey() { return this.props.enumKey; }
    set enumKey(value: string) { this.props.enumKey = value; }

    static format(enumType: any, value: any) {

        if (value == null) {
            return '';
        }

        var name = Enum.toString(enumType, value);
        var enumKeyAttr = getCustomAttribute(enumType, EnumKeyAttribute, false);
        var enumKey = enumKeyAttr ? enumKeyAttr.value : getTypeFullName(enumType);
        return EnumFormatter.getText(enumKey, name);
    }

    static getText(enumKey: string, name: string) {
        if (!name)
            return '';

        return htmlEncode(localText("Enums." + enumKey + '.' + name, name));
    }

    static getName(enumType: any, value: any) {
        if (value == null) {
            return '';
        }
        return Enum.toString(enumType, value);
    }
}
