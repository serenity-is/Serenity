import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatNumber, formatterTypeInfo, htmlEncode, nsSerenity, parseDecimal, registerType } from "../../base";
import { Formatter } from "../../slick";

/** Formats numeric values via {@link formatNumber} (default `"0.##"`). */
export class NumberFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * @param props.displayFormat - Number format string (default `"0.##"`).
     */
    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
    }

    /** @param ctx - Formatter context. @returns Formatted number string. */
    format(ctx: FormatterContext): string {
        return NumberFormatter.format(ctx.value, this.displayFormat);
    }

    /**
     * Static helper to format any numeric-like value.
     * @param value - Number or numeric string.
     * @param format - Format string (default `"0.##"`).
     * @returns Formatted string.
     */
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
