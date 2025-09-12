import { FormatterContext } from "@serenity-is/sleekgrid";
import { formatterTypeInfo, nsSerenity, registerType, stringFormat } from "../../base";
import { Formatter } from "../../slick";

export class MinuteFormatter implements Formatter {
    static typeInfo = formatterTypeInfo(nsSerenity); static { registerType(this); }

    format(ctx: FormatterContext) {
        return MinuteFormatter.format(ctx.value);
    }

    static format(value: number): string {
        var hour = Math.floor(value / 60);
        var minute = value - hour * 60;
        var hourStr, minuteStr;

        if (value == null || isNaN(value))
            return '';

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
