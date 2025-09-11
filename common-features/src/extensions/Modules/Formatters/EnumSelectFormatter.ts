import { EnumTypeRegistry, Formatter, FormatterBase, htmlEncode, localText, tryGetText } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

export class EnumSelectFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.Extensions.EnumSelectFormatter");

    constructor(public readonly props: { enumKey?: string, allowClear?: boolean, emptyItemText?: string } = {}) {
        super();
        this.props ??= {}
        this.props.allowClear ??= true;
    }

    format(ctx: FormatterContext) {
        var enumType = EnumTypeRegistry.get(this.props.enumKey ?? "EnumKeyOptionNotSpecified!");

        var sb = "<select>";
        if (this.props.allowClear) {
            sb += '<option value="">';
            sb += htmlEncode(this.props.emptyItemText || localText("Controls.SelectEditor.EmptyItemText"));
            sb += '</option>';
        }

        for (var x of Object.keys(enumType).filter(v => !isNaN(parseInt(v, 10)))) {
            sb += '<option value="' + htmlEncode(x) + '"';
            if (x == ctx.value)
                sb += " selected";
            var name = enumType[x];
            sb += ">";
            sb += htmlEncode(tryGetText("Enums." + this.props.enumKey + "." + name) || name);
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