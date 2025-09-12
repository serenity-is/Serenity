import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatNumber, formatterTypeInfo, htmlEncode, nsSerenity, parseDecimal, registerType } from "../../base";
import { Formatter } from "../../slick";

export class NumberFormatter implements Formatter {
    static typeInfo = formatterTypeInfo(nsSerenity); static { registerType(this); }
    
    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
    }

    format(ctx: FormatterContext): string {
        return NumberFormatter.format(ctx.value, this.displayFormat);
    }

    static format(value: any, format?: string): string {
        format = (format ?? '0.##');
        if (value == null)
            return '';

        if (typeof (value) === 'number') {
            if (isNaN(value))
                return '';

            return htmlEncode(formatNumber(value, format));
        }

        var dbl = parseDecimal(value.toString());
        if (dbl == null || isNaN(dbl))
            return value?.toString() ?? '';

        return htmlEncode(formatNumber(dbl, format));
    }

    get displayFormat() { return this.props.displayFormat; }
    set displayFormat(value) { this.props.displayFormat = value; }
}
