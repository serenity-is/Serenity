import { Formatter, formatterTypeInfo, htmlEncode, registerType, toSingleLine } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

export class SingleLineTextFormatter implements Formatter {
    static typeInfo = formatterTypeInfo("Serenity.Extensions.SingleLineTextFormatter"); static { registerType(this); }

    format(ctx: FormatterContext) {
        return SingleLineTextFormatter.formatValue(ctx.value);
    }

    public static formatValue(value: string) {
        var div = document.createElement("div");
        div.innerHTML = value ?? '';
        return htmlEncode(toSingleLine(div.textContent));
    }
}