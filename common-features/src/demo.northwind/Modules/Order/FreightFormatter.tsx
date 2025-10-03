import { Formatter, faIcon, formatterTypeInfo, registerType } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class FreightFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsDemoNorthwind); static { registerType(this); }

    format(ctx: FormatterContext): FormatterResult {
        if (ctx.value == null)
            return "";

        return <>{ctx.value} <i className={[faIcon("balance-scale", "secondary"), "text-opacity-75"]}></i></>;
    }
}
