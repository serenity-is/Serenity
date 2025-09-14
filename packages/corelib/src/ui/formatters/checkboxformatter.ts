import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, nsSerenity, registerType } from "../../base";
import { Formatter } from "../../slick";

export class CheckboxFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }
    
    format(ctx: FormatterContext) {
        return '<span class="check-box no-float readonly slick-edit-preclick ' + (!!ctx.value ? ' checked' : '') + '"></span>';
    }
}
