import { Column, FormatterContext, FormatterResult } from "@serenity-is/sleekgrid";
import { Culture, DialogTexts, Enum, faIcon, formatDate, formatNumber, formatterTypeInfo, FormatterTypeInfo, getCustomAttribute, getTypeFullName, htmlEncode, iconClassName, isPromiseLike, localText, parseDecimal, parseISODateTime, registerType, resolveUrl, stringFormat, StringLiteral, tryGetText, typeInfoProperty } from "../../base";
import { replaceAll } from "../../compat";
import { Formatter } from "../../slick";
import { EnumKeyAttribute } from "../../types/attributes";
import { EnumTypeRegistry } from "../../types/enumtyperegistry";

export interface IInitializeColumn {
    initializeColumn(column: Column): void;
}

export class IInitializeColumn {
    static typeInfo = formatterTypeInfo("Serenity.IInitializeColumn");
}

registerType(IInitializeColumn);

export abstract class FormatterBase implements Formatter {
    abstract format(ctx: FormatterContext): FormatterResult;

    protected static formatterTypeInfo<T>(typeName: StringLiteral<T>, interfaces?: any[]): FormatterTypeInfo<T> {
        if (Object.prototype.hasOwnProperty.call(this, typeInfoProperty) && this[typeInfoProperty])
            throw new Error(`Type ${this.name} already has a typeInfo property!`);
                
        const typeInfo = this.typeInfo = formatterTypeInfo(typeName, interfaces);
        registerType(this);
        return typeInfo;
    }

    static typeInfo: FormatterTypeInfo<"Serenity.FormatterBase">;
}

export class BooleanFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.BooleanFormatter");

    constructor(public readonly props: { falseText?: string, trueText?: string } = {}) {
        super();
        this.props ??= {};
    }

    format(ctx: FormatterContext) {

        if (ctx.value == null)
            return '';

        if (!!ctx.value)
            return ctx.escape(localText(this.trueText, this.trueText ?? DialogTexts.YesButton));

        return ctx.escape(localText(this.falseText, this.falseText ?? DialogTexts.NoButton));
    }

    public get falseText() { return this.props.falseText; }
    public set falseText(value) { this.props.falseText = value; }

    public get trueText() { return this.props.trueText; }
    public set trueText(value) { this.props.trueText = value; }
}

export class CheckboxFormatter extends FormatterBase {
    static override typeInfo = this.formatterTypeInfo("Serenity.CheckboxFormatter");
    
    format(ctx: FormatterContext) {
        return '<span class="check-box no-float readonly slick-edit-preclick ' + (!!ctx.value ? ' checked' : '') + '"></span>';
    }

}

export class DateFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.DateFormatter");

    constructor(public readonly props: { displayFormat?: string } = {}) {
        super();
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

export class DateTimeFormatter extends DateFormatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.DateTimeFormatter");
    constructor(props: { displayFormat?: string } = {}) {
        super({ displayFormat: Culture.dateTimeFormat, ...props });
    }
}

export class EnumFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.EnumFormatter");

    constructor(public readonly props: { enumKey?: string } = {}) {
        super();
        this.props ??= {};
    }

    format(ctx: FormatterContext): string | Element {
        var enumType = EnumTypeRegistry.getOrLoad(this.enumKey);
        if (isPromiseLike(enumType)) {
            const node = document.createElement("span");
            enumType.then(() => {
                const text = new Text(EnumFormatter.format(enumType, ctx.value));
                node.parentElement && node.replaceWith(text);
            });
            return node;
        }
        return EnumFormatter.format(enumType, ctx.value);
    }

    get enumKey() { return this.props.enumKey; }
    set enumKey(value: string) { this.props.enumKey = value; }

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

export class FileDownloadFormatter extends FormatterBase implements Formatter, IInitializeColumn {
    static override typeInfo = this.formatterTypeInfo("Serenity.IFileDownloadFormatter", [IInitializeColumn]);

    constructor(public readonly props: { displayFormat?: string, originalNameProperty?: string, iconClass?: string } = {}) {
        super();
        this.props ??= {};
    }

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

    get displayFormat() { return this.props.displayFormat; }
    set displayFormat(value) { this.props.displayFormat = value; }

    get originalNameProperty() { return this.props.originalNameProperty; }
    set originalNameProperty(value) { this.props.originalNameProperty = value; }

    get iconClass() { return this.props.iconClass; }
    set iconClass(value) { this.props.iconClass = value; }
}

export class MinuteFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.MinuteFormatter");

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

export class NumberFormatter extends FormatterBase implements Formatter {
    static override typeInfo = this.formatterTypeInfo("Serenity.NumberFormatter");
    
    constructor(public readonly props: { displayFormat?: string } = {}) {
        super();
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

export class UrlFormatter extends FormatterBase implements Formatter, IInitializeColumn {
    static override typeInfo = this.formatterTypeInfo("Serenity.UrlFormatter", [IInitializeColumn]);

    constructor(readonly props: { displayProperty?: string, displayFormat?: string, urlProperty?: string, urlFormat?: string, target?: string } = {}) {
        super();
        this.props ??= {};
    }

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

    get displayProperty() { return this.props.displayProperty }
    set displayProperty(value) { this.props.displayProperty = value }

    get displayFormat() { return this.props.displayFormat }
    set displayFormat(value) { this.props.displayFormat = value }

    get urlProperty() { return this.props.urlProperty }
    set urlProperty(value) { this.props.urlProperty = value }

    get urlFormat() { return this.props.urlFormat }
    set urlFormat(value) { this.props.urlFormat = value }

    get target() { return this.props.target }
    set target(value) { this.props.target = value }
}