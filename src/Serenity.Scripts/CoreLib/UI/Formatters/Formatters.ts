import { option, registerFormatter, registerInterface } from "../../Decorators";
import { Config } from "../../Q/Config";
import { ArgumentNullException, Exception } from "../../Q/Exceptions";
import { Culture, format, formatDate, formatNumber, parseDecimal, parseISODateTime } from "../../Q/Formatting";
import { attrEncode, htmlEncode } from "../../Q/Html";
import { tryGetText } from "../../Q/LocalText";
import { resolveUrl } from "../../Q/Services";
import { endsWith, isEmptyOrNull, replaceAll, startsWith } from "../../Q/Strings";
import { Enum, getAttributes, getTypeFullName, getTypes, isAssignableFrom, safeCast } from "../../Q/TypeSystem";
import { EnumKeyAttribute } from "../../Types/Attributes";
import { EnumTypeRegistry } from "../../Types/EnumTypeRegistry";
import { ISlickFormatter } from "../DataGrid/ISlickFormatter";

export interface IInitializeColumn {
    initializeColumn(column: Slick.Column): void;
}

@registerInterface('Serenity.IInitializeColumn')
export class IInitializeColumn {
}

function Formatter(name: string, intf?: any[]) {
    return registerFormatter('Serenity.' + name + 'Formatter', intf)
}

@Formatter('Boolean')
export class BooleanFormatter implements Slick.Formatter {
    format(ctx: Slick.FormatterContext) {

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

    @option()
    public falseText: string;

    @option()
    public trueText: string;
}

@Formatter('Checkbox')
export class CheckboxFormatter implements Slick.Formatter {
    format(ctx: Slick.FormatterContext) {
        return '<span class="check-box no-float readonly ' + (!!ctx.value ? ' checked' : '') + '"></span>';
    }
}

@Formatter('Date')
export class DateFormatter implements Slick.Formatter {
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

    @option()
    public displayFormat: string;

    format(ctx: Slick.FormatterContext): string {
        return DateFormatter.format(ctx.value, this.displayFormat);
    }
}

@Formatter('DateTime')
export class DateTimeFormatter extends DateFormatter {
    constructor() {
        super();
        this.displayFormat = Culture.dateTimeFormat;
    }
}

@Formatter('Enum')
export class EnumFormatter implements Slick.Formatter {

    format(ctx: Slick.FormatterContext): string {
        return EnumFormatter.format(EnumTypeRegistry.get(this.enumKey), ctx.value);
    }

    @option()
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

@Formatter('FileDownload', [ISlickFormatter, IInitializeColumn])
export class FileDownloadFormatter implements Slick.Formatter, IInitializeColumn {

    format(ctx: Slick.FormatterContext): string {
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

        return "<a class='file-download-link' target='_blank' href='" +
            attrEncode(downloadUrl) + "'>" + htmlEncode(text) + '</a>';
    }

    static dbFileUrl(filename: string): string {
        filename = replaceAll((filename ?? ''), '\\', '/');
        return resolveUrl('~/upload/') + filename;
    }

    initializeColumn(column: Slick.Column): void {
        column.referencedFields = column.referencedFields || [];
        if (!isEmptyOrNull(this.originalNameProperty)) {
            column.referencedFields.push(this.originalNameProperty);
            return;
        }
    }

    @option()
    displayFormat: string;

    @option()
    originalNameProperty: string;
}

@Formatter('Minute')
export class MinuteFormatter implements Slick.Formatter {

    format(ctx: Slick.FormatterContext) {
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

@Formatter('Number')
export class NumberFormatter {
    format(ctx: Slick.FormatterContext): string {
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

    @option()
    displayFormat: string;
}

@Formatter('Url', [ISlickFormatter, IInitializeColumn])
export class UrlFormatter implements Slick.Formatter, IInitializeColumn {

    format(ctx: Slick.FormatterContext): string {
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

        var s = "<a href='" + attrEncode(url) + "'";
        if (!isEmptyOrNull(this.target))
            s += " target='" + this.target + "'";

        s += '>' + htmlEncode(display) + '</a>';

        return s;
    }

    initializeColumn(column: Slick.Column): void {
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

    @option()
    displayProperty: string;

    @option()
    displayFormat: string;

    @option()
    urlProperty: string;

    @option()
    urlFormat: string;

    @option()
    target: string;
}

export namespace FormatterTypeRegistry {

    let knownTypes: { [key: string]: Function };

    function setTypeKeysWithoutFormatterSuffix() {
        var suffix = 'formatter';
        for (var k of Object.keys(knownTypes)) {
            if (!endsWith(k, suffix)) 
                continue;
            
            var p = k.substr(0, k.length - suffix.length);
            if (isEmptyOrNull(p))
                continue;
            
            if (knownTypes[p] != null)
                continue;
            
            knownTypes[p] = knownTypes[k];
        }
    }

    function initialize() {

        if (knownTypes) {
            return;
        }

        knownTypes = {};
        var types = getTypes();
        for (var type of types) {
            if (!isAssignableFrom(ISlickFormatter, type))
                continue;
                
            var fullName = getTypeFullName(type).toLowerCase();
            knownTypes[fullName] = type;

            for (var k of Config.rootNamespaces) {
                if (startsWith(fullName, k.toLowerCase() + '.')) {
                    var kx = fullName.substr(k.length + 1).toLowerCase();
                    if (knownTypes[kx] == null) {
                        knownTypes[kx] = type;
                    }
                }
            }
        }

        setTypeKeysWithoutFormatterSuffix();
    }

    export function get(key: string): Function {
        if (isEmptyOrNull(key)) 
            throw new ArgumentNullException('key');
        
        initialize();

        var formatterType = knownTypes[key.toLowerCase()];
        if (formatterType == null) {
            throw new Exception(format(
                "Can't find {0} formatter type!", key));
        }

        return formatterType;
    }

    export function reset() {
        knownTypes = null;
    }
}