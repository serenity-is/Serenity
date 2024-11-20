import { getLookup, getLookupAsync, fieldsProxy } from "@serenity-is/corelib";

export interface LanguageRow {
    LanguageId?: string;
    LanguageName?: string;
}

export abstract class LanguageRow {
    static readonly idProperty = 'LanguageId';
    static readonly nameProperty = 'LanguageName';
    static readonly localTextPrefix = 'Administration.Language';
    static readonly lookupKey = 'Administration.Language';

    /** @deprecated use getLookupAsync instead */
    static getLookup() { return getLookup<LanguageRow>('Administration.Language') }
    static async getLookupAsync() { return getLookupAsync<LanguageRow>('Administration.Language') }

    static readonly deletePermission = 'Administration:Translation';
    static readonly insertPermission = 'Administration:Translation';
    static readonly readPermission = 'Administration:Translation';
    static readonly updatePermission = 'Administration:Translation';

    static readonly Fields = fieldsProxy<LanguageRow>();
}