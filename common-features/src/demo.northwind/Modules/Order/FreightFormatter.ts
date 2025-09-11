import { Formatter, FormatterBase, faIcon } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

export class FreightFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.Demo.Northwind.FreightFormatter");

    format(ctx: FormatterContext) {
        if (ctx.value == null)
            return "";

        return `${ctx.escape()} <i class="${faIcon("balance-scale", "secondary")} text-opacity-75"></i>`;
    }
}
