import {
    Lookup, getColumnsScript, getFormScript, getLookupAsync, getRemoteDataAsync, getScriptData, getScriptDataHash, getStateStore, globalObject,
    handleScriptDataError, peekScriptData, reloadLookupAsync, resolveUrl, setScriptData, type PropertyItem, type PropertyItemsData
} from "@serenity-is/base";

export namespace ScriptData {

    export function bindToChange(name: string, onChange: () => void): void | (() => void) {
        if (typeof document !== "undefined" && document.addEventListener) {
            var unbind = function() {
                onChange && typeof document !== "undefined" && document.removeEventListener?.('scriptdatachange.' + name, onChange); 
                onChange = null;
            }
            document.addEventListener('scriptdatachange.' + name, onChange);
            return unbind;
        }
    }

    function loadOptions(name: string, dynJS: boolean, async: boolean): JQueryAjaxSettings {
        return {
            async: async,
            cache: true,
            type: 'GET',
            url: resolveUrl(dynJS ? '~/DynJS.axd/' : '~/DynamicData/') + name + (dynJS ? '.js' : '') + '?v=' + (getScriptDataHash(name) ?? new Date().getTime()),
            data: null,
            dataType: dynJS ? 'text' : 'json',
            converters: {
                "text script": function (text: string) {
                    return text;
                }
            },
            success: dynJS ? function (data) {
                $.globalEval(data);
            } : undefined,
            error: function (xhr) {
                handleScriptDataError(name, xhr.status, xhr.statusText);
            }
        }
    }

    function legacyLoad(name: string, dynJS: boolean): JQueryXHR {
        return $.ajax(loadOptions(name, dynJS, false));
    }

    export const canLoad = canLoadScriptData;

    export function ensure<TData = any>(name: string, dynJS?: boolean): TData {
        var data = peekScriptData(name);
        if (data != null)
            return data;

        if (dynJS) {
            legacyLoad(name, dynJS);
            data = peekScriptData(name);
            if (data == null)
                handleScriptDataError(name);
            return data;
        }
        data = legacyLoad(name, dynJS).responseJSON;
        if (data == null)
            handleScriptDataError(name);
        if (name.startsWith("Lookup."))
            data = new Lookup(data.Params, data.Items);
        set(name, data);
        return data;
    }

    export function reload<TData = any>(name: string): TData {
        getScriptDataHash(name, true);
        legacyLoad(name, false);
        return peekScriptData(name);
    }

    export async function reloadAsync<TData = any>(name: string): Promise<TData> {
        return await getScriptData(name, true);
    }

    export function setRegisteredScripts(scripts: any[]) {
        var t = new Date().getTime().toString();
        var store = getStateStore("__scriptHash")
        for (var k in scripts) {
            store[k], scripts[k] || t;
        }
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

export function getTemplate(key: string): string {
    return ScriptData.ensure('Template.' + key, false);
}

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
    getTemplate,
    reloadLookup,
    reloadLookupAsync,
    ScriptData
}

let serenity = (globalObject.Serenity || (globalObject.Serenity = Object.create(null)));
for (var i in compatExports)
    if (serenity[i] == null)
        serenity[i] = (compatExports as any)[i];