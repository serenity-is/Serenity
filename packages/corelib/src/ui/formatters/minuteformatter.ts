import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, nsSerenity, registerType, stringFormat } from "../../base";
import { Formatter } from "../../slick";

/** Formats an integer minute count as `HH:mm` (e.g. 90 → `"01:30"`). */
export class MinuteFormatter implements Formatter {
    static [Symbol.typeInfo] = formatterTypeInfo(nsSerenity); static { registerType(this); }

    /**
     * Formats the cell value as `HH:mm`.
     * @param ctx - Formatter context containing the minute value.
     * @returns `HH:mm` string.
     */
    format(ctx: FormatterContext) {
        return MinuteFormatter.format(ctx.value);
    }

    /**
     * Static helper to format minutes.
     * @param value - Total minutes.
     * @returns `HH:mm` string or empty if invalid.
     */
    static format(value: number): string {
        if (value == null || (value as any) === '' || !isFinite(value))
            return '';

        var hour = Math.floor(value / 60);
        var minute = value - hour * 60;
        var hourStr, minuteStr;

        if (hour < 10)
            hourStr = '0' + hour;
        else
            hourStr = hour.toString();

        if (minute < 10)
            minuteStr = '0' + minute;
        else
            minuteStr = minute.toString();

        return stringFormat('{0}:{1}', hourStr, minuteStr);
    }
}
