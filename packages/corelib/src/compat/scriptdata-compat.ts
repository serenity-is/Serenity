import {
    Lookup, ensureScriptDataSync, getColumnsScript, getFormScript, getGlobalObject, getLookupAsync, getRemoteData, getRemoteDataAsync, getScriptData, getScriptDataHash,
    peekScriptData, reloadLookupAsync, setScriptData, type PropertyItem, type PropertyItemsData
} from "../base";

export namespace ScriptData {
    export const canLoad = canLoadScriptData;
    export const ensure = ensureScriptDataSync;
    export const set = setScriptData;

    export function bindToChange(name: string, onChange: () => void): void | (() => void) {
        if (typeof document !== "undefined" && document.addEventListener) {
            var unbind = function () {
                onChange && typeof document !== "undefined" && document.removeEventListener?.('scriptdatachange.' + name, onChange);
                onChange = null;
            }
            document.addEventListener('scriptdatachange.' + name, onChange);
            return unbind;
        }
    }

    export function reload<TData = any>(name: string, dynJS?: boolean): TData {
        getScriptDataHash(name, true);
        setScriptData(name, null);
        return ensure(name, dynJS);
    }

    export async function reloadAsync<TData = any>(name: string): Promise<TData> {
        return await getScriptData(name, true);
    }
}

/**
 * Check if a dynamic script with provided name is available in the cache
 * or it is a registered script name
 * @param name Dynamic script name
 * @returns True if already available or registered
 */
export function canLoadScriptData(name: string) {
    return peekScriptData(name) != null || getScriptDataHash(name) != null;
}

export function getLookup<TItem>(key: string): Lookup<TItem> {
    return ScriptData.ensure('Lookup.' + key);
}

export function reloadLookup<TItem = any>(key: string): Lookup<TItem> {
    return ScriptData.reload('Lookup.' + key);
}

export function getColumns(key: string): PropertyItem[] {
    return getColumnsData(key).items;
}

export async function getColumnsAsync(key: string): Promise<PropertyItem[]> {
    return (await getColumnsScript(key)).items;
}

export function getColumnsData(key: string): PropertyItemsData {
    return ScriptData.ensure('Columns.' + key);
}

export const getColumnsDataAsync = getColumnsScript;

export function getForm(key: string): PropertyItem[] {
    return getFormData(key).items;
}

export async function getFormAsync(key: string): Promise<PropertyItem[]> {
    return (await getFormScript(key)).items;
}

export function getFormData(key: string): PropertyItemsData {
    return ScriptData.ensure('Form.' + key);
}

export const getFormDataAsync = getFormScript;

var compatExports = {
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
for (var i in compatExports)
    if (serenity[i] == null)
        serenity[i] = (compatExports as any)[i];