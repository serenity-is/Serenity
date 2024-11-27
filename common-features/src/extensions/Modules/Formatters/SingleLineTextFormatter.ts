import { Decorators, Formatter, htmlEncode, toSingleLine } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

@Decorators.registerFormatter('Serenity.Extensions.SingleLineTextFormatter')
export class SingleLineTextFormatter implements Formatter {
    format(ctx: FormatterContext) {
        return SingleLineTextFormatter.formatValue(ctx.value);
    }

    public static formatValue(value: string) {
        var div = document.createElement("div");
        div.innerHTML = value ?? '';
        return htmlEncode(toSingleLine(div.textContent));
    }
}