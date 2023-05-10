import { Culture, Enum, format, formatDate, formatNumber, getAttributes, getTypeFullName, htmlEncode, isEmptyOrNull, ISlickFormatter, parseDecimal, parseISODateTime, replaceAll, resolveUrl, safeCast, startsWith, tryGetText } from "@serenity-is/corelib/q";
import { Formatter } from "@serenity-is/corelib/slick";
import { Column, FormatterContext } from "@serenity-is/sleekgrid";
import { Decorators, EnumKeyAttribute } from "../../decorators";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";

export interface IInitializeColumn {
    initializeColumn(column: Column): void;
}

@Decorators.registerInterface('Serenity.IInitializeColumn')
export class IInitializeColumn {
}

@Decorators.registerFormatter('Serenity.BooleanFormatter')
export class BooleanFormatter implements Formatter {
    format(ctx: FormatterContext) {

        if (ctx.value == null) {
            return '';
        }

        var text;
        if (!!ctx.value) {
            text = tryGetText(this.trueText);
            if (text == null) {
                text = this.trueText;
                if (text == null) {
                    text = tryGetText('Dialogs.YesButton') ?? 'Yes';
                }
            }
        }
        else {
            text = tryGetText(this.falseText);
            if (text == null) {
                text = this.falseText;
                if (text == null) {
                    text = tryGetText('Dialogs.NoButton') ?? 'No';
                }
            }
        }

        return htmlEncode(text);
    }

    @Decorators.option()
    public falseText: string;

    @Decorators.option()
    public trueText: string;
}

@Decorators.registerFormatter('Serenity.CheckboxFormatter')
export class CheckboxFormatter implements Formatter {
    format(ctx: FormatterContext) {
        return '<span class="check-box no-float readonly slick-edit-preclick ' + (!!ctx.value ? ' checked' : '') + '"></span>';
    }
}

@Decorators.registerFormatter('Serenity.DateFormatter')
export class DateFormatter implements Formatter {
    constructor() {
        this.displayFormat = Culture.dateFormat;
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

            if (date == null) {
                return htmlEncode(value);
            }
        }
        else {
            return value.toString();
        }

        return htmlEncode(formatDate(date, format));
    }

    @Decorators.option()
    public displayFormat: string;

    format(ctx: FormatterContext): string {
        return DateFormatter.format(ctx.value, this.displayFormat);
    }
}

@Decorators.registerFormatter('Serenity.DateTimeFormatter')
export class DateTimeFormatter extends DateFormatter {
    constructor() {
        super();
        this.displayFormat = Culture.dateTimeFormat;
    }
}

@Decorators.registerFormatter('Serenity.EnumFormatter')
export class EnumFormatter implements Formatter {

    format(ctx: FormatterContext): string {
        return EnumFormatter.format(EnumTypeRegistry.get(this.enumKey), ctx.value);
    }

    @Decorators.option()
    enumKey: string;

    static format(enumType: any, value: any) {

        if (value == null) {
            return '';
        }

        var name = Enum.toString(enumType, value);
        var enumKeyAttr = getAttributes(enumType, EnumKeyAttribute, false);
        var enumKey = ((enumKeyAttr.length > 0) ? enumKeyAttr[0].value : getTypeFullName(enumType));
        return EnumFormatter.getText(enumKey, name);
    }

    static getText(enumKey: string, name: string) {
        if (isEmptyOrNull(name)) {
            return '';
        }

        return htmlEncode(tryGetText('Enums.' + enumKey + '.' + name) ?? name);
    }

    static getName(enumType: any, value: any) {
        if (value == null) {
            return '';
        }
        return Enum.toString(enumType, value);
    }
}

@Decorators.registerFormatter('Serenity.FileDownloadFormatter', [ISlickFormatter, IInitializeColumn])
export class FileDownloadFormatter implements Formatter, IInitializeColumn {

    format(ctx: FormatterContext): string {
        var dbFile = safeCast(ctx.value, String);
        if (isEmptyOrNull(dbFile)) {
            return '';
        }

        var downloadUrl = FileDownloadFormatter.dbFileUrl(dbFile);
        var originalName = (!isEmptyOrNull(this.originalNameProperty) ?
            safeCast(ctx.item[this.originalNameProperty], String) : null);

        originalName = (originalName ?? '');
        var text = format((this.displayFormat ?? '{0}'),
            originalName, dbFile, downloadUrl);

        var iconClass = this.iconClass ?? "fa fa-download";

        if (iconClass.startsWith("fa-"))
            iconClass = "fa " + iconClass;

        return "<a class='file-download-link' target='_blank' href='" +
            htmlEncode(downloadUrl) + "'><i class='" + iconClass + "'></i> " + htmlEncode(text) + '</a>';
    }

    static dbFileUrl(filename: string): string {
        filename = replaceAll((filename ?? ''), '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];
        if (!isEmptyOrNull(this.originalNameProperty)) {
            column.referencedFields.push(this.originalNameProperty);
            return;
        }
    }

    @Decorators.option()
    displayFormat: string;

    @Decorators.option()
    originalNameProperty: string;

    @Decorators.option()
    iconClass: string;
}

@Decorators.registerFormatter('Serenity.MinuteFormatter')
export class MinuteFormatter implements Formatter {

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

        return format('{0}:{1}', hourStr, minuteStr);
    }
}

@Decorators.registerFormatter('Serenity.NumberFormatter')
export class NumberFormatter {
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
        if (dbl == null)
            return '';

        return htmlEncode(value.toString());
    }

    @Decorators.option()
    displayFormat: string;
}

@Decorators.registerFormatter('Serenity.UrlFormatter', [ISlickFormatter, IInitializeColumn])
export class UrlFormatter implements Formatter, IInitializeColumn {

    format(ctx: FormatterContext): string {
        var url = (!isEmptyOrNull(this.urlProperty) ?
            (ctx.item[this.urlProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (isEmptyOrNull(url))
            return '';

        if (!isEmptyOrNull(this.urlFormat))
            url = format(this.urlFormat, url);

        if (url != null && startsWith(url, '~/'))
            url = resolveUrl(url);

        var display = (!isEmptyOrNull(this.displayProperty) ?
            (ctx.item[this.displayProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (!isEmptyOrNull(this.displayFormat))
            display = format(this.displayFormat, display);

        var s = "<a href='" + htmlEncode(url) + "'";
        if (!isEmptyOrNull(this.target))
            s += " target='" + this.target + "'";

        s += '>' + htmlEncode(display) + '</a>';

        return s;
    }

    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];

        if (!isEmptyOrNull(this.displayProperty)) {
            column.referencedFields.push(this.displayProperty);
            return;
        }

        if (!isEmptyOrNull(this.urlProperty)) {
            column.referencedFields.push(this.urlProperty);
            return;
        }
    }

    @Decorators.option()
    displayProperty: string;

    @Decorators.option()
    displayFormat: string;

    @Decorators.option()
    urlProperty: string;

    @Decorators.option()
    urlFormat: string;

    @Decorators.option()
    target: string;
}