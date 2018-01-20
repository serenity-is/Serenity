namespace Serenity {

    export interface IInitializeColumn {
        initializeColumn(column: Slick.Column): void;
    }

    @Serenity.Decorators.registerInterface('Serenity.IInitializeColumn')
    export class IInitializeColumn {
    }

    import Option = Serenity.Decorators.option

    function Formatter(name: string, intf?: any[]) {
        return Decorators.registerFormatter('Serenity.' + name + 'Formatter', intf)
    }

    @Formatter('Boolean')
    export class BooleanFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext) {

            if (ctx.value == null) {
                return '';
            }

            var text;
            if (!!ctx.value) {
                text = Q.tryGetText(this.trueText);
                if (text == null) {
                    text = this.trueText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.YesButton'), 'Yes');
                    }
                }
            }
            else {
                text = Q.tryGetText(this.falseText);
                if (text == null) {
                    text = this.falseText;
                    if (text == null) {
                        text = Q.coalesce(Q.tryGetText('Dialogs.NoButton'), 'No');
                    }
                }
            }

            return Q.htmlEncode(text);
        }

        @Serenity.Decorators.option()
        public falseText: string;

        @Serenity.Decorators.option()
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
            this.set_displayFormat(Q.Culture.dateFormat);
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
                date = Q.parseISODateTime(value);

                if (date == null) {
                    return Q.htmlEncode(value);
                }
            }
            else {
                return value.toString();
            }

            return Q.htmlEncode(Q.formatDate(date, format));
        }

        private displayFormat: string;

        @Option()
        get_displayFormat(): string {
            return this.displayFormat;
        }

        set_displayFormat(value: string): void {
            this.displayFormat = value;
        }

        format(ctx: Slick.FormatterContext): string {
            return DateFormatter.format(ctx.value, this.get_displayFormat());
        }
    }

    @Formatter('DateTime')
    export class DateTimeFormatter extends DateFormatter {
        constructor() {
            super();
            this.set_displayFormat(Q.Culture.dateTimeFormat);
        }
    }

    @Formatter('Enum')
    export class EnumFormatter implements Slick.Formatter {

        format(ctx: Slick.FormatterContext): string {
            return EnumFormatter.format(EnumTypeRegistry.get(this.enumKey), ctx.value);
        }

        @Option()
        enumKey: string;

        static format(enumType: any, value: any) {

            if (value == null) {
                return '';
            }

            var name;
            try {
                name = (ss as any).Enum.toString(enumType, value);
            }
            catch (e) {
                e = (ss.Exception as any).wrap(e);
                if ((ss as any).isInstanceOfType(e, (ss as any).ArgumentException)) {
                    name = value.toString();
                }
                else {
                    throw e;
                }
            }

            var enumKeyAttr = (ss as any).getAttributes(enumType, EnumKeyAttribute, false);
            var enumKey = ((enumKeyAttr.length > 0) ? enumKeyAttr[0].value : (ss as any).getTypeFullName(enumType));
            return EnumFormatter.getText(enumKey, name);
        }

        static getText(enumKey: string, name: string) {
            if (Q.isEmptyOrNull(name)) {
                return '';
            }

            return Q.htmlEncode(Q.coalesce(Q.tryGetText('Enums.' + enumKey + '.' + name), name));
        }

        static getName(enumType: any, value: any) {
            if (value == null) {
                return '';
            }
            return (ss as any).Enum.toString(enumType, value);
        }
    }

    @Formatter('FileDownload', [ISlickFormatter, IInitializeColumn])
    export class FileDownloadFormatter implements Slick.Formatter, IInitializeColumn {

        format(ctx: Slick.FormatterContext): string {
            var dbFile = (ss as any).safeCast(ctx.value, String);
            if (Q.isEmptyOrNull(dbFile)) {
                return '';
            }

            var downloadUrl = FileDownloadFormatter.dbFileUrl(dbFile);
            var originalName = (!Q.isEmptyOrNull(this.originalNameProperty) ?
                (ss as any).safeCast(ctx.item[this.originalNameProperty], String) : null);

            originalName = Q.coalesce(originalName, '');
            var text = Q.format(Q.coalesce(this.displayFormat, '{0}'),
                originalName, dbFile, downloadUrl);

            return "<a class='file-download-link' target='_blank' href='" +
                Q.attrEncode(downloadUrl) + "'>" + Q.htmlEncode(text) + '</a>';
        }

        static dbFileUrl(filename: string): string {
            filename = Q.replaceAll(Q.coalesce(filename, ''), '\\', '/');
            return Q.resolveUrl('~/upload/') + filename;
        }

        initializeColumn(column: Slick.Column): void {
            column.referencedFields = column.referencedFields || [];
            if (!Q.isEmptyOrNull(this.originalNameProperty)) {
                column.referencedFields.push(this.originalNameProperty);
                return;
            }
        }

        @Option()
        displayFormat: string;

        @Option()
        originalNameProperty: string;
    }

    @Formatter('Minute')
    export class MinuteFormatter implements Slick.Formatter {

        format(ctx: Slick.FormatterContext) {
            return MinuteFormatter.format(ctx.value);
        }

        static format(value: number): string {
            var hour = (ss as any).Int32.trunc(Math.floor(value / 60));
            var minute = value - hour * 60;
            var hourStr, minuteStr;

            if (value != null || isNaN(value))
                return '';

            if (hour < 10)
                hourStr = '0' + hour;
            else
                hourStr = hour.toString();

            if (minute < 10)
                minuteStr = '0' + minute;
            else
                minuteStr = minute.toString();

            return Q.format('{0}:{1}', hourStr, minuteStr);
        }
    }

    @Formatter('Number')
    export class NumberFormatter {
        format(ctx: Slick.FormatterContext): string {
            return NumberFormatter.format(ctx.value, this.displayFormat);
        }

        static format(value: any, format?: string): string {
            format = Q.coalesce(format, '0.##');
            if (value == null)
                return '';

            if (typeof (value) === 'number') {
                if (isNaN(value))
                    return '';

                return Q.htmlEncode(Q.formatNumber(value, format));
            }

            var dbl = Q.parseDecimal(value.toString());
            if (dbl == null)
                return '';

            return Q.htmlEncode(value.toString());
        }

        @Option()
        displayFormat: string;
    }

    @Formatter('Url', [ISlickFormatter, IInitializeColumn])
    export class UrlFormatter implements Slick.Formatter, IInitializeColumn {

        format(ctx: Slick.FormatterContext): string {
            var url = (!Q.isEmptyOrNull(this.urlProperty) ?
                Q.coalesce(ctx.item[this.urlProperty], '').toString() :
                Q.coalesce(ctx.value, '').toString());

            if (Q.isEmptyOrNull(url))
                return '';

            if (!Q.isEmptyOrNull(this.urlFormat))
                url = Q.format(this.urlFormat, url);

            if (url != null && Q.startsWith(url, '~/'))
                url = Q.resolveUrl(url);

            var display = (!Q.isEmptyOrNull(this.displayProperty) ?
                Q.coalesce(ctx.item[this.displayProperty], '').toString() :
                Q.coalesce(ctx.value, '').toString());

            if (!Q.isEmptyOrNull(this.displayFormat))
                display = Q.format(this.displayFormat, display);

            var s = "<a href='" + Q.attrEncode(url) + "'";
            if (!Q.isEmptyOrNull(this.target))
                s += " target='" + this.target + "'";

            s += '>' + Q.htmlEncode(display) + '</a>';

            return s;
        }

        initializeColumn(column: Slick.Column): void {
            column.referencedFields = column.referencedFields || [];

            if (!Q.isEmptyOrNull(this.displayProperty)) {
                column.referencedFields.push(this.displayProperty);
                return;
            }

            if (!Q.isEmptyOrNull(this.urlProperty)) {
                column.referencedFields.push(this.urlProperty);
                return;
            }
        }

        @Option()
        displayProperty: string;

        @Option()
        displayFormat: string;

        @Option()
        urlProperty: string;

        @Option()
        urlFormat: string;

        @Option()
        target: string;
    }

    export namespace FormatterTypeRegistry {

        let knownTypes: Q.Dictionary<Function>;

        function setTypeKeysWithoutFormatterSuffix() {
            var suffix = 'formatter';
            for (var k of Object.keys(knownTypes)) {
                if (!Q.endsWith(k, suffix)) 
                    continue;
                
                var p = k.substr(0, k.length - suffix.length);
                if (Q.isEmptyOrNull(p))
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
            var assemblies = (ss as any).getAssemblies();
            for (var assembly of assemblies) {
                var types = (ss as any).getAssemblyTypes(assembly);
                for (var type of types) {
                    if (!(ss as any).isAssignableFrom(Serenity.ISlickFormatter, type))
                        continue;
                    
                    if ((ss as any).isGenericTypeDefinition(type))
                        continue;
                    
                    var fullName = (ss as any).getTypeFullName(type).toLowerCase();
                    knownTypes[fullName] = type;

                    for (var k of Q.Config.rootNamespaces) {
                        if (Q.startsWith(fullName, k.toLowerCase() + '.')) {
                            var kx = fullName.substr(k.length + 1).toLowerCase();
                            if (knownTypes[kx] == null) {
                                knownTypes[kx] = type;
                            }
                        }
                    }
                }
            }

            setTypeKeysWithoutFormatterSuffix();
        }

        export function get(key: string): Function {
            if (Q.isEmptyOrNull(key)) 
                throw new (ss as any).ArgumentNullException('key');
            
            initialize();

            var formatterType = knownTypes[key.toLowerCase()];
            if (formatterType == null) {
                throw new ss.Exception(Q.format(
                    "Can't find {0} formatter type!", key));
            }

            return formatterType;
        }

        export function reset() {
            knownTypes = null;
        }
    }
}