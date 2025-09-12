import { FormatterContext } from "@serenity-is/sleekgrid";
import { Culture, formatDate, formatterTypeInfo, htmlEncode, nsSerenity, parseISODateTime, registerType } from "../../base";
import { Formatter } from "../../slick";

export class DateFormatter implements Formatter {
    static typeInfo = formatterTypeInfo(nsSerenity); static { registerType(this); }

    constructor(public readonly props: { displayFormat?: string } = {}) {
        this.props ??= {};
        this.props.displayFormat ??= Culture.dateFormat;
    }

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

    public get displayFormat() { return this.props.displayFormat; }
    public set displayFormat(value) { this.props.displayFormat = value; }

    format(ctx: FormatterContext): string {
        return DateFormatter.format(ctx.value, this.displayFormat);
    }
}