interface Q {
    formatDate(date: Date, format?: string): string;
    htmlEncode(s: any): string;
    resolveUrl(url: string): string;
    text(s: string): string;
    trim(s: string): string;
    notifyError(msg: string): void;
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

declare var Q: Q;
declare var Q$Config: Q$Config;
declare var Q$Culture: Q$Culture;
declare var Serenity: Serenity

declare module ss {
    export class Exception {
    }
}