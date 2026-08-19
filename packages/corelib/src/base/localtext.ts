import { localTextTableSymbol } from "./symbols";
import { getGlobalObject } from "./system";

function getTable(): { [key: string]: string } {
    let localTextTable = getGlobalObject()[localTextTableSymbol];
    if (!localTextTable)
        getGlobalObject()[localTextTableSymbol] = localTextTable = {};
    return localTextTable;
}

/**
 * Adds one or more entries to the global localization table.
 * @param obj - Either a single key (string) whose value is `pre`, or a nested object map where leaf string values are stored under dot-joined keys (recursively). Pass `null`/`undefined`/empty to no-op.
 * @param pre - Prefix prepended to each key, or the value when `obj` is a string. Defaults to `""` for object mode.
 * @remarks The table is stored on the global object under {@link localTextTableSymbol} and is shared across the application. Nested objects are flattened with `.` separators (e.g. `{ a: { b: "x" } }` with `pre="Ns."` registers `"Ns.a.b"`).
 * @example
 * ```ts
 * addLocalText({ "Db.Northwind.CustomerName": "Customer Name" });
 * addLocalText("Db.Northwind.CustomerName", "Customer Name");
 * addLocalText({ Customer: { Name: "Name" } }, "Db.Northwind.");
 * ```
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
 * Retrieves a localized string for a key, falling back gracefully.
 * @param key - Localization key to look up (e.g. `"Dialogs.YesButton"`). If falsy, an empty string or `defaultText` is returned.
 * @param defaultText - Optional fallback returned when the key is not found. When omitted, the key itself is returned.
 * @returns The localized value, `defaultText` if provided, the key itself, or `""` for nullish keys.
 * @example
 * ```ts
 * localText("Dialogs.YesButton"); // "Yes" if registered, otherwise "Dialogs.YesButton"
 * localText("Missing.Key", "Fallback"); // "Fallback"
 * ```
 */
export function localText(key: string, defaultText?: string): string {
    return getTable()[key] ?? defaultText ?? key ?? '';
}

/**
 * Tries to retrieve a localized string without falling back to the key.
 * @param key - Localization key to look up.
 * @returns The localized value if found, otherwise `undefined` (unlike {@link localText} which returns the key).
 */
export function tryGetText(key: string): string | undefined {
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
 * Creates a typed proxy that resolves nested property access to localized strings.
 * The proxy lazily wraps each level of `tpl`; property access concatenates `pfx` with the property name and performs the lookup according to `mode`.
 * @param obj - Target object to proxy (usually `{}`). Mutated in place with hidden symbols and returned as a `Proxy`.
 * @param pfx - Prefix prepended to every key lookup (e.g. `"Db.Northwind."`). Pass `""` for no prefix.
 * @param tpl - Template object whose shape defines the available text keys; leaf values determine nesting. `null`/`undefined` leaves resolve to string lookups, objects create nested sub-proxies.
 * @param mode - Lookup strategy: `undefined` uses {@link localText} (returns key on miss), `"asTry"` uses {@link tryGetText} (returns `undefined` on miss), `"asKey"` returns the generated key without any lookup.
 * @returns A proxy over `obj` augmented with `asTry()` and `asKey()` mode-switchers. Access a leaf string like `proxy.foo.bar` to get the localized text for `"<pfx>foo.bar"`.
 * @example
 * ```ts
 * const texts = proxyTexts({}, "", { user: { name: {} } });
 * texts.user.name.first // localText("user.name.first")
 * texts.user.asTry().name.first // tryGetText("user.name.first")
 * texts.user.asKey().name.first // "user.name.first"
 * ```
 */
export function proxyTexts<T extends Record<string, any> = Record<string, any>>(obj: T, pfx: string, tpl: Record<string, any>, mode?: "asTry" | "asKey"):
    Record<string, any> & { asTry(): T; asKey(): T } {
    (obj as any)[proxyTextsPrefixSymbol] = pfx ?? '';
    (obj as any)[proxyTextsModeSymbol] = mode;
    (obj as any)[proxyTextsTemplateSymbol] = tpl;
    return new Proxy(obj, localizationProxyHandler) as any;
}

/**
 * List of available languages for the translation UI.
 * Each entry pairs a language identifier with its display name.
 */
export type LanguageList = { /** Language identifier (e.g. `"en"`, `"tr"`). */ id: string, /** Human-readable language name. */ text: string }[];

/**
 * Options passed to {@link TranslationConfig.translateTexts} to request machine/human translations.
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
 * Result returned by {@link TranslationConfig.translateTexts} containing translated entries.
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
 * Global configuration hooks for the optional translation service integration.
 * Assign these before invoking translation features in the UI.
 */
export const TranslationConfig = {
    /** Retrieves the list of available languages */
    getLanguageList: null as () => LanguageList,
    /** A function to translate texts based on provided options */
    translateTexts: null as (opt: TranslateTextsOptions) => PromiseLike<TranslateTextsResult>
}

/**
 * Alias for {@link localText}.
 * @deprecated Prefer {@link localText} directly for better discoverability and consistency.
 */
export const text = localText;

/**
 * Legacy namespace for local-text helpers.
 * @deprecated Use the top-level {@link addLocalText} and {@link localText} functions instead.
 */
export namespace LT {
    /**
     * Alias for {@link addLocalText}.
     * @deprecated Use {@link addLocalText} directly.
     */
    export const add = addLocalText;
    /**
     * Alias for {@link localText}.
     * @deprecated Use {@link localText} directly.
     */
    export const getDefault = localText;
}

const global = getGlobalObject();
const serenity = global.Serenity || (global.Serenity = {});
if (!serenity.addLocalText) {
    serenity.addLocalText = addLocalText;
}
const lt = (serenity.LT || (serenity.LT = {}));
if (!lt.add) {
    lt.add = serenity.addLocalText;
}

