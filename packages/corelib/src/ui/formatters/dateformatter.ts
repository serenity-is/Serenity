import { FormatterContext } from "@serenity-is/sleekgrid";
import { Culture, formatDate, formatterTypeInfo, htmlEncode, nsSerenity, parseISODateTime, registerType } from "../../base";
import { Formatter } from "../../slick";

/** Formats date values using {@link formatDate} / {@link Culture.dateFormat}. */
export class DateFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Creates a new DateFormatter.
     * @param props - Formatter options.
     * @param props.displayFormat - Date format string (default `Culture.dateFormat`).
     */
    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
        this.props.displayFormat ??= Culture.dateFormat;
    }

    /**
     * Static helper to format any date-like value.
     * @param value - Date instance or ISO string.
     * @param format - Format string (defaults to culture format).
     * @returns HTML-encoded formatted string.
     */
    static format(value: any, format?: string) {
        if (value == null) {
            return '';
        }

        var date: Date;

        if (value instanceof Date) {
            date = value;
        }
        else if (typeof value === 'string') {
            date = parseISODateTime(value);

            if (date == null || isNaN(date.valueOf())) {
                return htmlEncode(value);
            }
        }
        else {
            return value.toString();
        }

        return htmlEncode(formatDate(date, format));
    }

    /** Gets the date display format. @returns The display format string. */
    public get displayFormat() { return this.props.displayFormat; }
    /**
     * Sets the date display format.
     * @param value - The display format string.
     */
    public set displayFormat(value) { this.props.displayFormat = value; }

    /**
     * Formats the cell value as a date string.
     * @param ctx - Formatter context containing the cell value.
     * @returns HTML-encoded formatted date string.
     */
    format(ctx: FormatterContext): string {
        return DateFormatter.format(ctx.value, this.displayFormat);
    }
}