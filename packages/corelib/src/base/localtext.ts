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

const proxyTextsPrefixSymbol = Symbol.for("Serenity.proxyTextsPrefix");
const proxyTextsTemplateSymbol = Symbol.for("Serenity.proxyTextsTemplate");
const proxyTextsModeSymbol = Symbol.for("Serenity.proxyTextsMode");
const proxyTextsAsTryCache = new WeakMap<object, Object>();
const proxyTextsAsKeyCache = new WeakMap<object, Object>();

/** This handler is shared by all localization proxies that are initially created by proxyTexts method */
const localizationProxyHandler: ProxyHandler<Record<string, any>> = {
    get: (target: Record<string, any>, property: string | symbol, receiver: any) => {
        if (typeof property !== "string") {
            return (target as any)[property];
        }

        const prefix = (target as any)[proxyTextsPrefixSymbol] ?? '';
        const mode = (target as any)[proxyTextsModeSymbol] as "asTry" | "asKey";

        const tpl = (target as any)[proxyTextsTemplateSymbol];
        if (!tpl)
            return undefined;

        if (property === "asKey" || property === "asTry") {
            if (mode === property) {
                // proxy is already in the requested mode
                return function () {
                    return receiver;
                }
            }

            return function () {
                const cache = property === "asTry" ? proxyTextsAsTryCache : proxyTextsAsKeyCache;
                let cached = cache.get(target);
                if (!cached) {
                    cached = proxyTexts({}, prefix, tpl, property);
                    cache.set(target, cached);
                }
                return cached;
            }
        }

        const tpmval = tpl[property];
        const key = prefix + property;
        if (tpmval == null)
            return mode === "asTry" ? tryGetText(key) : (mode == "asKey" ? key : localText(key));

        const subProxy = target[property];
        if (subProxy != null)
            return subProxy;

        return target[property] = proxyTexts({}, key + '.', tpmval, mode);
    },
    ownKeys: (target: Record<string, any>) => Object.keys((target as any)[proxyTextsTemplateSymbol])
};

/**
 * Creates a proxy object for localized text retrieval with lazy loading and caching.
 * @param obj - The target object to proxy (usually an empty object {})
 * @param pfx - The key prefix for all text lookups
 * @param tpl - Template object defining the structure (object properties become nested proxies)
 * @param mode - The lookup mode: default is localText, "asTry"=tryGetText, "asKey"=return key
 * @returns A proxy object that provides localized text access
 * 
 * @example
 * const texts = proxyTexts({}, '', { user: { name: {} } });
 * texts.user.name.first // looks up "user.name.first" key
 * texts.user.asTry().name.first // returns undefined if not found
 * texts.user.asKey().name.first // returns "user.name.first"
 */
export function proxyTexts<T extends Record<string, any> = Record<string, any>>(obj: T, pfx: string, tpl: Record<string, any>, mode?: "asTry" | "asKey"):
    Record<string, any> & { asTry(): T; asKey(): T } {
    (obj as any)[proxyTextsPrefixSymbol] = pfx ?? '';
    (obj as any)[proxyTextsModeSymbol] = mode;
    (obj as any)[proxyTextsTemplateSymbol] = tpl;
    return new Proxy(obj, localizationProxyHandler) as any;
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

