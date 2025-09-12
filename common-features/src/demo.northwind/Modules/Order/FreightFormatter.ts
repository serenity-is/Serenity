import { Formatter, faIcon, formatterTypeInfo, registerType } from "@serenity-is/corelib";
import { FormatterContext } from "@serenity-is/sleekgrid";

export class FreightFormatter implements Formatter {
    static typeInfo = formatterTypeInfo("Serenity.Demo.Northwind.FreightFormatter"); static { registerType(this); }

    format(ctx: FormatterContext) {
        if (ctx.value == null)
            return "";

        return `${ctx.escape()} <i class="${faIcon("balance-scale", "secondary")} text-opacity-75"></i>`;
    }
}
