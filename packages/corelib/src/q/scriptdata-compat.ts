import {
    Lookup, getColumnsScript, getFormScript, getGlobalObject, getLookupAsync, getRemoteDataAsync, getScriptData, getScriptDataHash, handleScriptDataError, isPromiseLike, peekScriptData, reloadLookupAsync,
    requestFinished, requestStarting,
    resolveUrl, scriptDataHooks, setScriptData, type PropertyItem, type PropertyItemsData
} from "../base";

export namespace ScriptData {

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

    export const canLoad = canLoadScriptData;

    export function ensure<TData = any>(name: string, dynJS?: boolean): TData {
        var data = peekScriptData(name);
        if (data != null)
            return data;

        data = scriptDataHooks.fetchScriptData?.<TData>(name, true, dynJS);
        if (data !== void 0) {
            if (isPromiseLike(data))
                throw new Error("fetchScriptData hook must return data synchronously when sync is true.");
            if (name.startsWith("Lookup.") && (data as any)?.Items)
                data = new Lookup((data as any).Params, (data as any).Items) as any;
            set(name, data);
            return data;
        }

        var url = resolveUrl(dynJS ? '~/DynJS.axd/' : '~/DynamicData/') + name + (dynJS ? '.js' : '') + '?v=' + (getScriptDataHash(name) ?? new Date().getTime());
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url, false);
        requestStarting();
        try {

            xhr.send(null);
            if (xhr.status !== 200)
                handleScriptDataError(name, xhr.status, xhr.statusText);

            if (dynJS) {
                var script = document.createElement("script");
                script.text = xhr.responseText;
                document.head.appendChild(script).parentNode.removeChild(script);
                data = peekScriptData(name);
            }
            else {
                data = JSON.parse(xhr.responseText);
            }
            if (data == null)
                handleScriptDataError(name);
            if (!dynJS && name.startsWith("Lookup."))
                data = new Lookup(data.Params, data.Items);
            set(name, data);
            return data;
        }
        finally {
            requestFinished();
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

    export const set = setScriptData;
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

export function getRemoteData<TData = any>(key: string): TData {
    return ScriptData.ensure('RemoteData.' + key);
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