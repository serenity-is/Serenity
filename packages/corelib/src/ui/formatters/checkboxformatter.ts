import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, registerType } from "../../base";
import { Formatter } from "../../slick";

export class CheckboxFormatter implements Formatter {
    static typeInfo = formatterTypeInfo("Serenity.CheckboxFormatter"); static { registerType(this); }
    
    format(ctx: FormatterContext) {
        return '<span class="check-box no-float readonly slick-edit-preclick ' + (!!ctx.value ? ' checked' : '') + '"></span>';
    }
}
