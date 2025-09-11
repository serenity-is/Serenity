import { Formatter, FormatterBase, htmlEncode, toSingleLine } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

export class SingleLineTextFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.Extensions.SingleLineTextFormatter");

    format(ctx: FormatterContext) {
        return SingleLineTextFormatter.formatValue(ctx.value);
    }

    public static formatValue(value: string) {
        var div = document.createElement("div");
        div.innerHTML = value ?? '';
        return htmlEncode(toSingleLine(div.textContent));
    }
}