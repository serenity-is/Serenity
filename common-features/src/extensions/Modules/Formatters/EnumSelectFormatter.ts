import { EnumTypeRegistry, Formatter, formatterTypeInfo, htmlEncode, localText, registerType, SelectEditorTexts } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class EnumSelectFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsExtensions); static { registerType(this); }

    constructor(public readonly props: { enumKey?: string, allowClear?: boolean, emptyItemText?: string } = {}) {
        this.props ??= {}
        this.props.allowClear ??= true;
    }

    format(ctx: FormatterContext) {
        var enumType = EnumTypeRegistry.get(this.props.enumKey ?? "EnumKeyOptionNotSpecified!");

        var sb = "<select>";
        if (this.props.allowClear) {
            sb += '<option value="">';
            sb += htmlEncode(this.props.emptyItemText ?? SelectEditorTexts.EmptyItemText);
            sb += '</option>';
        }

        for (var x of Object.keys(enumType).filter(v => !isNaN(parseInt(v, 10)))) {
            sb += '<option value="' + htmlEncode(x) + '"';
            if (x == ctx.value)
                sb += " selected";
            var name = enumType[x];
            sb += ">";
            sb += htmlEncode(localText("Enums." + this.props.enumKey + "." + name, name));
            sb += "</option>";
        }

        sb += "</select>";

        return sb;
    }

    get enumKey() { return this.props.enumKey }
    set enumKey(value) { this.props.enumKey = value }

    get allowClear() { return this.props.allowClear }
    set allowClear(value) { this.props.allowClear = value }

    get emptyItemText() { return this.props.emptyItemText }
    set emptyItemText(value) { this.props.emptyItemText = value }
}