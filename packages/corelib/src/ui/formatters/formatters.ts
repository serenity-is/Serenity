import { Culture, DialogTexts, Enum, faIcon, formatDate, formatNumber, getCustomAttribute, getTypeFullName, htmlEncode, iconClassName, localText, parseDecimal, parseISODateTime, resolveUrl, stringFormat, tryGetText } from "@serenity-is/base";
import { Column, FormatterContext } from "@serenity-is/sleekgrid";
import { Decorators, EnumKeyAttribute } from "../../decorators";
import { replaceAll } from "../../q";
import { Formatter } from "../../slick";
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

        if (ctx.value == null)
            return '';

        if (!!ctx.value)
            return ctx.escape(localText(this.trueText, this.trueText ?? DialogTexts.YesButton));

        return ctx.escape(localText(this.falseText, this.falseText ?? DialogTexts.NoButton));
    }

    @Decorators.option()
    public falseText: string;

    @Decorators.option()
    public trueText: string;
}

export class CheckboxFormatter extends Formatter {
    static override typeName = this.registerFormatter("Serenity.CheckboxFormatter")

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

            if (date == null || isNaN(date.valueOf())) {
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
        var enumKeyAttr = getCustomAttribute(enumType, EnumKeyAttribute, false);
        var enumKey = enumKeyAttr ? enumKeyAttr.value : getTypeFullName(enumType);
        return EnumFormatter.getText(enumKey, name);
    }

    static getText(enumKey: string, name: string) {
        if (!name)
            return '';

        return htmlEncode(tryGetText('Enums.' + enumKey + '.' + name) ?? name);
    }

    static getName(enumType: any, value: any) {
        if (value == null) {
            return '';
        }
        return Enum.toString(enumType, value);
    }
}

@Decorators.registerFormatter('Serenity.FileDownloadFormatter', [IInitializeColumn])
export class FileDownloadFormatter implements Formatter, IInitializeColumn {

    format(ctx: FormatterContext): string {
        var dbFile = ctx.value as string;
        if (!dbFile)
            return '';

        var downloadUrl = FileDownloadFormatter.dbFileUrl(dbFile);
        var originalName = this.originalNameProperty ?
            ctx.item[this.originalNameProperty] as string : null;

        originalName = (originalName ?? '');
        var text = stringFormat((this.displayFormat ?? '{0}'),
            originalName, dbFile, downloadUrl);

        var iconClass = iconClassName(this.iconClass ?? faIcon("download"));

        return "<a class='file-download-link' target='_blank' href='" +
            htmlEncode(downloadUrl) + "'><i class='" + iconClass + "'></i> " + htmlEncode(text) + '</a>';
    }

    static dbFileUrl(filename: string): string {
        filename = replaceAll((filename ?? ''), '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];
        if (this.originalNameProperty) {
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

        return stringFormat('{0}:{1}', hourStr, minuteStr);
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
        if (dbl == null || isNaN(dbl))
            return value?.toString() ?? '';

        return htmlEncode(formatNumber(dbl, format));
    }

    @Decorators.option()
    displayFormat: string;
}

@Decorators.registerFormatter('Serenity.UrlFormatter', [IInitializeColumn])
export class UrlFormatter implements Formatter, IInitializeColumn {

    format(ctx: FormatterContext): string {
        var url = (this.urlProperty ?
            (ctx.item[this.urlProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (!url)
            return '';

        if (this.urlFormat)
            url = stringFormat(this.urlFormat, url);

        url = resolveUrl(url);

        var display = (this.displayProperty ?
            (ctx.item[this.displayProperty] ?? '').toString() :
            (ctx.value ?? '').toString());

        if (this.displayFormat)
            display = stringFormat(this.displayFormat, display);

        var s = "<a href='" + htmlEncode(url) + "'";
        if (this.target)
            s += " target='" + this.target + "'";

        s += '>' + htmlEncode(display) + '</a>';

        return s;
    }

    initializeColumn(column: Column): void {
        column.referencedFields = column.referencedFields || [];

        if (this.displayProperty) {
            column.referencedFields.push(this.displayProperty);
        }

        if (this.urlProperty) {
            column.referencedFields.push(this.urlProperty);
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