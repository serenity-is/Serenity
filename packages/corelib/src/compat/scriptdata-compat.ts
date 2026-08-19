import {
    Lookup, ensureScriptDataSync, getColumnsScript, getFormScript, getGlobalObject, getLookupAsync, getRemoteData, getRemoteDataAsync, getScriptData, getScriptDataHash,
    peekScriptData, reloadLookupAsync, setScriptData, type PropertyItem, type PropertyItemsData
} from "../base";

/**
 * Legacy `ScriptData` namespace compat shim.
 * Wraps the modern `../base` script-data APIs (`getScriptData`, `setScriptData`, `ensureScriptDataSync`, etc.)
 * under the old `Q.ScriptData` / `Serenity.ScriptData` surface. All members delegate to the new APIs.
 * @deprecated Prefer importing `getScriptData`, `setScriptData`, `ensureScriptDataSync`, `peekScriptData`, etc. directly from `@serenity-is/corelib`. Kept for backward compatibility.
 */
export namespace ScriptData {
    /** Alias for {@link canLoadScriptData}. @deprecated Use {@link canLoadScriptData} or `peekScriptData` / `getScriptDataHash` directly. */
    export const canLoad = canLoadScriptData;
    /** Alias for {@link ensureScriptDataSync}. @deprecated Use `ensureScriptDataSync` directly. */
    export const ensure = ensureScriptDataSync;
    /** Alias for {@link setScriptData}. @deprecated Use `setScriptData` directly. */
    export const set = setScriptData;

    /**
     * Binds a callback to the `scriptdatachange.<name>` document event.
     * @param name - Dynamic script name (event namespace suffix).
     * @param onChange - Callback invoked when script data for `name` changes.
     * @returns An unbind function that removes the listener and clears the callback, or `void` when `document` is unavailable.
     */
    export function bindToChange(name: string, onChange: () => void): void | (() => void) {
        if (typeof document !== "undefined" && document.addEventListener) {
            const unbind = function () {
                onChange && typeof document !== "undefined" && document.removeEventListener?.('scriptdatachange.' + name, onChange);
                onChange = null;
            }
            document.addEventListener('scriptdatachange.' + name, onChange);
            return unbind;
        }
    }

    /**
     * Synchronously reloads a dynamic script by clearing its cache and re-ensuring it.
     * @param name - Dynamic script name.
     * @param dynJS - When `true`, passed through to the underlying `ensure` call (legacy flag).
     * @returns The reloaded script data.
     * @deprecated Prefer `getScriptData(name, true)` or `getScriptDataAsync`. Kept for legacy callers.
     */
    export function reload<TData = any>(name: string, dynJS?: boolean): TData {
        getScriptDataHash(name, true);
        setScriptData(name, null);
        return ensure(name, dynJS);
    }

    /**
     * Asynchronously reloads a dynamic script, bypassing the cache.
     * @param name - Dynamic script name.
     * @returns A promise resolving to the reloaded script data.
     * @deprecated Prefer `getScriptData(name, true)` directly.
     */
    export async function reloadAsync<TData = any>(name: string): Promise<TData> {
        return await getScriptData(name, true);
    }
}

/**
 * Checks whether a dynamic script with the given name is available in the cache or is a registered script name.
 * Compat shim for the legacy `Q.canLoadScriptData` global; delegates to `peekScriptData` and `getScriptDataHash`.
 * @param name - Dynamic script name (e.g., `"Lookup.Administration.User"`).
 * @returns `true` if the script is already cached or its hash is registered; otherwise `false`.
 * @deprecated Prefer `peekScriptData` / `getScriptDataHash` checks or `getScriptData` directly.
 */
export function canLoadScriptData(name: string) {
    return peekScriptData(name) != null || getScriptDataHash(name) != null;
}

/**
 * Synchronously retrieves a lookup by key.
 * Compat shim for `Q.getLookup`; delegates to `ScriptData.ensure('Lookup.' + key)`.
 * @param key - Lookup key (e.g., `"Administration.User"`).
 * @returns The {@link Lookup} instance for the key.
 * @deprecated Prefer `getLookupAsync` or direct `getScriptData` usage. Kept for legacy synchronous callers.
 */
export function getLookup<TItem>(key: string): Lookup<TItem> {
    return ScriptData.ensure('Lookup.' + key);
}

/**
 * Synchronously reloads a lookup by key.
 * Compat shim for `Q.reloadLookup`; delegates to `ScriptData.reload('Lookup.' + key)`.
 * @param key - Lookup key.
 * @returns The reloaded {@link Lookup} instance.
 * @deprecated Prefer `reloadLookupAsync` or `getScriptData(key, true)`.
 */
export function reloadLookup<TItem = any>(key: string): Lookup<TItem> {
    return ScriptData.reload('Lookup.' + key);
}

/**
 * Synchronously retrieves column metadata for a row/form key.
 * Compat shim for `Q.getColumns`; delegates to `getColumnsData(key).items`.
 * @param key - Columns key (usually a row or entity type name).
 * @returns The array of {@link PropertyItem} column definitions, or an empty array if not found.
 * @deprecated Prefer `getColumnsAsync` / `getColumnsScript` for async loading. Kept for legacy synchronous callers.
 */
export function getColumns(key: string): PropertyItem[] {
    return getColumnsData(key)?.items ?? [];
}

/**
 * Asynchronously retrieves column metadata for a row/form key.
 * Compat shim for `Q.getColumnsAsync`; delegates to `getColumnsScript(key)`.
 * @param key - Columns key.
 * @returns A promise resolving to the array of {@link PropertyItem} column definitions.
 */
export async function getColumnsAsync(key: string): Promise<PropertyItem[]> {
    return (await getColumnsScript(key)).items;
}

/**
 * Synchronously retrieves the full columns data object for a key.
 * Compat shim for `Q.getColumnsData`; delegates to `ScriptData.ensure('Columns.' + key)`.
 * @param key - Columns key.
 * @returns The {@link PropertyItemsData} containing `items` and related metadata.
 * @deprecated Prefer `getColumnsDataAsync` / `getColumnsScript`.
 */
export function getColumnsData(key: string): PropertyItemsData {
    return ScriptData.ensure('Columns.' + key);
}

/** Alias for {@link getColumnsScript}. Compat shim for `Q.getColumnsDataAsync`. */
export const getColumnsDataAsync = getColumnsScript;

/**
 * Synchronously retrieves form metadata for a key.
 * Compat shim for `Q.getForm`; delegates to `getFormData(key).items`.
 * @param key - Form key (usually a form type name).
 * @returns The array of {@link PropertyItem} form field definitions, or an empty array if not found.
 * @deprecated Prefer `getFormAsync` / `getFormScript` for async loading.
 */
export function getForm(key: string): PropertyItem[] {
    return getFormData(key)?.items ?? [];
}

/**
 * Asynchronously retrieves form metadata for a key.
 * Compat shim for `Q.getFormAsync`; delegates to `getFormScript(key)`.
 * @param key - Form key.
 * @returns A promise resolving to the array of {@link PropertyItem} form field definitions.
 */
export async function getFormAsync(key: string): Promise<PropertyItem[]> {
    return (await getFormScript(key)).items;
}

/**
 * Synchronously retrieves the full form data object for a key.
 * Compat shim for `Q.getFormData`; delegates to `ScriptData.ensure('Form.' + key)`.
 * @param key - Form key.
 * @returns The {@link PropertyItemsData} containing `items` and related metadata.
 * @deprecated Prefer `getFormDataAsync` / `getFormScript`.
 */
export function getFormData(key: string): PropertyItemsData {
    return ScriptData.ensure('Form.' + key);
}

/** Alias for {@link getFormScript}. Compat shim for `Q.getFormDataAsync`. */
export const getFormDataAsync = getFormScript;

const compatExports = {
    canLoadScriptData,
    getColumns,
    getColumnsAsync,
    getForm,
    getFormAsync,
    getLookup,
    getLookupAsync,
    getRemoteData,
    getRemoteDataAsync,
    reloadLookup,
    reloadLookupAsync,
    ScriptData
}

let global = getGlobalObject();
let serenity = (global.Serenity || (global.Serenity = Object.create(null)));
for (const i in compatExports)
    if (serenity[i] == null)
        serenity[i] = (compatExports as any)[i];