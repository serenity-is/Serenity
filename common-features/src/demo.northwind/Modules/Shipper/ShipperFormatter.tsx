import { Formatter, FormatterBase, faIcon } from "@serenity-is/corelib";
import { FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";

export class ShipperFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.Demo.Northwind.ShipperFormatter");
    
    format(ctx: FormatterContext): FormatterResult {

        if (!ctx.value) {
            return <>{ctx.value}</>;
        }

        return (<><i class={["text-info", faIcon(ctx.value == "Speedy Express" ? "plane" :
            (ctx.value == "Federal Shipping" ? "ship" : "truck")), "text-opacity-75"]} /> {ctx.value}</>);
    }
}
