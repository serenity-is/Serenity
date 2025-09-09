import { localTextTableSymbol } from "./symbols";
import { getGlobalObject } from "./system";

function getTable(): { [key: string]: string } {
    let localTextTable = getGlobalObject()[localTextTableSymbol];
    if (!localTextTable)
        getGlobalObject()[localTextTableSymbol] = localTextTable = {};
    return localTextTable;
}

/**
 * Adds local text entries to the localization table.
 * @param obj The object containing key/value pairs to add. If a string is provided, it will be added as a key with the prefix (second argument) as its value.
 * @param pre The prefix to add to each key. If obj is a string, this will be the value for that key.
 */
export function addLocalText(obj: string | Record<string, string | Record<string, any>>, pre?: string) {
    if (!obj)
        return;

    let table = getTable();

    if (typeof obj === "string") {
        table[obj] = pre;
        return;
    }

    pre ??= '';
    for (let k of Object.keys(obj)) {
        let actual = pre + k;
        let o = obj[k];
        if (typeof (o) === 'object') {
            addLocalText(o, actual + '.');
        }
        else {
            table[actual] = o;
        }
    }
}

/**
 * Retrieves a localized string from the localization table.
 * @param key The key of the localized string.
 * @param defaultText The default text to return if the key is not found.
 * @returns The localized string or the default text.
 */
export function localText(key: string, defaultText?: string): string {
    return getTable()[key] ?? defaultText ?? key ?? '';
}

/**
 * Tries to retrieve a localized string from the localization table.
 * @param key The key of the localized string.
 * @returns The localized string or undefined if not found.
 */
export function tryGetText(key: string): string {
    return getTable()[key];
}

/**
 * Proxies text retrieval for localization.
 * @param o The original object.
 * @param p The prefix for the keys.
 * @param t The translation table.
 * @returns A proxy object for localized text retrieval.
 */
export function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object {
    return new Proxy(o, {
        get: (_: Object, y: string) => {
            if (typeof y === "symbol")
                return;
            var tv = t[y];
            if (tv == null)
                return localText(p + y);
            else {
                var z = o[y];
                if (z != null)
                    return z;
                o[y] = z = proxyTexts({}, p + y + '.', tv);
                return z;
            }
        },
        ownKeys: (_: Object) => Object.keys(t)
    });
}

/**
 * A list of languages with their IDs and display texts.
 */
export type LanguageList = { id: string, text: string }[];

/**
 * Options for translating texts.
 */
export type TranslateTextsOptions = {
    /** The source language ID */
    SourceLanguageID?: string,
    /** An array of inputs for translation */
    Inputs: {
        /** The text key to be translated */
        TextKey?: string,
        /** The target language ID */
        TargetLanguageID?: string,
        /** The source text to be translated */
        SourceText?: string
    }[]
};

/**
 * The result of a translation operation.
 */
export type TranslateTextsResult = {
    /** An array of resulting translations */
    Translations?: {
        /** The text key that was translated */
        TextKey?: string,
        /** The target language ID */
        TargetLanguageID?: string,
        /** The translated text */
        TranslatedText?: string
    }[]
};

/**
 * Configuration for translation services.
 */
export const TranslationConfig = {
    /** Retrieves the list of available languages */
    getLanguageList: null as () => LanguageList,
    /** A function to translate texts based on provided options */
    translateTexts: null as (opt: TranslateTextsOptions) => PromiseLike<TranslateTextsResult>
}

/** @deprecated prefer localText for better discoverability */
export const text = localText;

export namespace LT {
    /** @deprecated Use addLocalText */
    export const add = addLocalText;
    /** @deprecated Use localText */
    export const getDefault = localText;
}

const global = getGlobalObject();
const serenity = global.Serenity || (global.Serenity = {});
serenity.LT = serenity.LT || {};
serenity.LT.add = serenity.addLocalText = addLocalText;