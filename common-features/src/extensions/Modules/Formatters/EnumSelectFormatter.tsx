import { EnumTypeRegistry, Formatter, formatterTypeInfo, htmlEncode, localText, registerType, SelectEditorTexts } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { nsExtensions } from "../ServerTypes/Namespaces";

export class EnumSelectFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsExtensions); static { registerType(this); }

    constructor(public readonly props: { enumKey?: string, allowClear?: boolean, emptyItemText?: string } = {}) {
        this.props ??= {}
        this.props.allowClear ??= true;
    }

    format(ctx: FormatterContext): FormatterResult {
        const enumType = EnumTypeRegistry.get(this.props.enumKey ?? "EnumKeyOptionNotSpecified!");

        return (
            <select>
                {this.props.allowClear && <option value="">
                    {htmlEncode(this.props.emptyItemText ?? SelectEditorTexts.EmptyItemText)}
                </option>}
                {Object.keys(enumType).filter(v => !isNaN(parseInt(v, 10))).map(x => (
                    <option value={x} selected={x == ctx.value}>
                        {localText("Enums." + this.props.enumKey + "." + enumType[x], enumType[x])}
                    </option>
                ))}
            </select>
        );
    }

    get enumKey() { return this.props.enumKey }
    set enumKey(value) { this.props.enumKey = value }

    get allowClear() { return this.props.allowClear }
    set allowClear(value) { this.props.allowClear = value }

    get emptyItemText() { return this.props.emptyItemText }
    set emptyItemText(value) { this.props.emptyItemText = value }
}