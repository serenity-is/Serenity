import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatNumber, formatterTypeInfo, htmlEncode, nsSerenity, parseDecimal, registerType } from "../../base";
import { Formatter } from "../../slick";

/** Formats numeric values via {@link formatNumber} (default `"0.##"`). */
export class NumberFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a new NumberFormatter.
     * @param props - Formatter options.
     * @param props.displayFormat - Number format string (default `"0.##"`).
     */
    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
    }

    /**
     * Formats the cell value as a number string.
     * @param ctx - Formatter context containing the cell value.
     * @returns Formatted number string.
     */
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

    /** Gets the number display format. @returns The display format string. */
    get displayFormat() { return this.props.displayFormat; }
    /**
     * Sets the number display format.
     * @param value - The display format string.
     */
    set displayFormat(value) { this.props.displayFormat = value; }
}
