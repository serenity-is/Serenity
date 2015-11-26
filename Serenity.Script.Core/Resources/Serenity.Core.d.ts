declare module Q {
    function formatDate(date: Date, format?: string): string;
    function htmlEncode(s: any): string;
    function resolveUrl(url: string): string;
    function text(s: string): string;
    function trim(s: string): string;
    function notifyError(msg: string): void;
}

interface Q$Culture {
    dateFormat: string;
    dateOrder: string;
    dateSeparator: string;
    dateTimeFormat: string;
    decimalSeparator: string;
    get_groupSeperator(): string;
}

interface Q$Config {
    emailAllowOnlyAscii?: boolean;
}

interface Serenity {
    CustomValidation: Serenity.CustomValidation;
}

declare namespace Serenity {
    interface CustomValidation {
        registerValidationMethods(): void;
    }
}

declare var Q$Config: Q$Config;
declare var Q$Culture: Q$Culture;
declare var Serenity: Serenity

declare module ss {
    export class Exception {
    }
}