declare namespace Serenity {

    class DateFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format: string): string;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
    }

    class DateTimeFormatter extends DateFormatter {
    }

    class EnumFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(enumType: Function, value: any): string;
        static getText(enumKey: string, name: string): string;
        static getText(value: any): string;
        static getName(value: any): string;
        get_enumKey(): string;
        set_enumKey(value: string): void;
    }

    class FileDownloadFormatter {
        format(ctx: Slick.FormatterContext): string;
        static dbFileUrl(filename: string): string;
        initializeColumn(column: Slick.Column): void;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
        get_originalNameProperty(): string;
        set_originalNameProperty(value: string): void;
    }

    class NumberFormatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any, format: string): string;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
    }

    class MinuteFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        static format(value: any): string;
    }

    class UrlFormatter implements Slick.Formatter {
        format(ctx: Slick.FormatterContext): string;
        get_displayProperty(): string;
        set_displayProperty(value: string): void;
        get_displayFormat(): string;
        set_displayFormat(value: string): void;
        get_urlProperty(): string;
        set_urlProperty(value: string): void;
        get_urlFormat(): string;
        set_urlFormat(value: string): void;
        get_target(): string;
        set_target(value: string): void;
    }

    namespace FormatterTypeRegistry {
        function get(key: string): Function;
        function initialize(): void;
        function reset(): void;
    }
}