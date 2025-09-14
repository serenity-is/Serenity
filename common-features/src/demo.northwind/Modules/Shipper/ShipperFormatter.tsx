import { Formatter, faIcon, formatterTypeInfo, registerType } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { nsDemoNorthwind } from "../ServerTypes/Namespaces";

export class ShipperFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsDemoNorthwind); static { registerType(this); }
    
    format(ctx: FormatterContext): FormatterResult {

        if (!ctx.value) {
            return <>{ctx.value}</>;
        }

        return (<><i class={["text-info", faIcon(ctx.value == "Speedy Express" ? "plane" :
            (ctx.value == "Federal Shipping" ? "ship" : "truck")), "text-opacity-75"]} /> {ctx.value}</>);
    }
}
