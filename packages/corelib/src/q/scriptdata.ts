import { getStateStore, notifyError, stringFormat, Lookup, type PropertyItem, type PropertyItemsData, resolveUrl } from "@serenity-is/base";
import { blockUI, blockUndo } from "./blockui";
import { alertDialog, iframeDialog } from "./dialogs";

function getHash(key: string, reload?: boolean): string {
    let k: string;

    const stateStore = getStateStore();
    if (stateStore.__scriptHash == null &&
        typeof document !== "undefined" && 
        (k = (document.querySelector('script#RegisteredScripts')?.innerHTML ?? '').trim()) && 
        k.charAt(0) == '{') {
        var regs = JSON.parse(k);
        ScriptData.setRegisteredScripts(regs);
    }

    if (stateStore.__scriptHash == null) {
        if (reload)
            stateStore.__scriptHash = {};
        else
            return null;
    }

    if (reload)
        return (stateStore.__scriptHash[key] = new Date().getTime().toString());

    return stateStore.__scriptHash[key];
}

function setHash(key: string, value: string): void {
    getStateStore("__scriptHash")[key] = value;
}

function getLoadedData(name: string): any {
    return getStateStore("__scriptData")[name];
}

function setLoadedData(name: string, value: any): void {
    getStateStore("__scriptData")[name] = value;
}

export namespace ScriptData {
    
    export function bindToChange(name: string, regClass: string, onChange: () => void) {
        typeof $ !== "undefined" && typeof document !== "undefined" && ($(document.body) as any).bind('scriptdatachange.' + regClass, function (e: any, s: string) {
            if (s == name) {
                onChange();
            }
        });
    }

    export function triggerChange(name: string) {
        typeof $ !== "undefined" && typeof document !== "undefined" && $(document.body).triggerHandler('scriptdatachange', [name]);
    }

    export function unbindFromChange(regClass: string) {
        typeof $ !== "undefined" && typeof document !== "undefined" && $(document.body).unbind('scriptdatachange.' + regClass);
    }

    function loadOptions(name: string, dynJS: boolean, async: boolean): JQueryAjaxSettings {
        return {
            async: async,
            cache: true,
            type: 'GET',
            url: resolveUrl(dynJS ? '~/DynJS.axd/' : '~/DynamicData/') + name + (dynJS ? '.js' : '') + '?v=' + (getHash(name) ?? new Date().getTime()),
            data: null,
            dataType: dynJS ? 'text' : 'json',
            converters: {
                "text script": function (text: string) {
                    return text;
                }
            },
            success: dynJS ? function (data, textStatus, jqXHR) {
                $.globalEval(data);
            } : undefined,
            error: function (xhr, textStatus, errorThrown) {
                var isLookup = name?.startsWith("Lookup.");
                if (xhr.status == 403 && isLookup) {
                    notifyError('<p>Access denied while trying to load the lookup: "<b>' +
                        name.substring(7) + '</b>". Please check if current user has required permissions for this lookup.</p> ' +
                        '<p><em>Lookups use the ReadPermission of their row by default. You may override that for the lookup ' +
                        'like [LookupScript("Some.Lookup", Permission = "?")] to grant all ' +
                        'authenticated users to read it (or use "*" for public).</em></p>' + 
                        '<p><em>Note that this might be a security risk if the lookup contains sensitive data, ' +
                        'so it could be better to set a separate permission for lookups, like "MyModule:Lookups".</em></p>', null, {
                            timeOut: 10000,
                            escapeHtml: false
                        });
                    return;
                }

                notifyError("An error occurred while trying to load " +
                    (isLookup ? ' the lookup: "' + name.substring(7) :
                        ' dynamic script: "' + name) +
                    '"!. Please check the error message displayed in the dialog below for more info.');

                var html = xhr.responseText;
                if (!html) {
                    if (!xhr.status)
                        alertDialog("An unknown connection error occurred! Check browser console for details.");
                    else if (xhr.status == 500)
                        alertDialog("HTTP 500: Connection refused! Check browser console for details.");
                    else
                        alertDialog("HTTP " + xhr.status + ' error! Check browser console for details.');
                }
                else
                    iframeDialog({ html: html });
            }
        }
    }

    let loadScriptPromises: { [key: string]: Promise<any> }  = {}

    function loadScriptAsync<TData>(name: string, dynJS: boolean): Promise<TData> {
        let key = name + '?' + (getHash(name) ?? '');

        var promise = loadScriptPromises[key];
        if (promise)
            return promise;

        let options = loadOptions(name, dynJS, true);
        blockUI(null);
        loadScriptPromises[key] = promise = Promise.resolve(
            $.ajax(options).always(function () {
                delete loadScriptPromises[key];
                blockUndo();
            }));

        return promise;
    }

    function loadScriptData(name: string, dynJS: boolean): JQueryXHR {
        return $.ajax(loadOptions(name, dynJS, false));
    }

    async function loadScriptDataAsync<TData = any>(name: string, dynJS: boolean): Promise<TData> {
        return await loadScriptAsync<TData>(name, dynJS);
    }

    function loadError(name: string) {
        var message;

        if (name != null &&
            name.startsWith('Lookup.')) {
            message = 'No lookup with key "' + name.substring(7) + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + name.substring(7) + '")] attribute in server side code on top of a row / custom lookup and ' +
                ' its key is exactly the same.';
        } else {
            message = stringFormat("Can't load script data: {0}!", name);
        }

        notifyError(message);
        throw new Error(message);
    }

    export function ensure<TData = any>(name: string, dynJS?: boolean): TData {
        var data = getLoadedData(name);
        if (data != null)
            return data;

        if (dynJS) {
            loadScriptData(name, dynJS);
            data = getLoadedData(name);
            if (data == null)
                loadError(name);
            return data;
        }
        data = loadScriptData(name, dynJS).responseJSON;
        if (data == null)
            loadError(name);
        if (name.startsWith("Lookup."))
            data = new Lookup(data.Params, data.Items);
        set(name, data);
        return data;
    }

    export async function ensureAsync<TData = any>(name: string, dynJS?: boolean): Promise<TData> {
        var data = getLoadedData(name);
        if (data != null)
            return data;

        if (dynJS) {
            await loadScriptDataAsync(name, dynJS);
            data = getLoadedData(name);
            if (data == null)
                loadError(name);
            return data;
        }

        data = await loadScriptData(name, dynJS);
        if (name.startsWith("Lookup."))
            data = new Lookup(data.Params, data.Items);
        set(name, data);
        return data;
    }

    export function reload<TData = any>(name: string, dynJS?: boolean): TData {
        getHash(name, true);
        loadScriptData(name, dynJS);
        var data = getLoadedData(name);
        return data;
    }

    export async function reloadAsync<TData = any>(name: string, dynJS?: boolean): Promise<TData> {
        getHash(name, true);
        await loadScriptDataAsync(name, dynJS);
        return getLoadedData(name);
    }

    export function canLoad(name: string) {
        return getLoadedData(name) != null || getHash(name) != null;
    }

    export function setRegisteredScripts(scripts: any[]) {
        var t = new Date().getTime().toString();
        for (var k in scripts) {
            setHash(k, scripts[k] || t);
        }
    }

    export function set(name: string, value: any) {
        setLoadedData(name, value);
        triggerChange(name);
    }
}

export function getRemoteData<TData = any>(key: string, dynJS?: boolean): TData {
    return ScriptData.ensure('RemoteData.' + key, dynJS);
}

export async function getRemoteDataAsync<TData = any>(key: string, dynJS?: boolean): Promise<TData> {
    return await ScriptData.ensureAsync('RemoteData.' + key, dynJS);
}

export function getLookup<TItem>(key: string, dynJS?: boolean): Lookup<TItem> {
    return ScriptData.ensure('Lookup.' + key, dynJS);
}

export async function getLookupAsync<TItem>(key: string, dynJS?: boolean): Promise<Lookup<TItem>> {
    return await ScriptData.ensureAsync('Lookup.' + key, dynJS);
}

export function reloadLookup<TItem = any>(key: string, dynJS?: boolean): Lookup<TItem> {
    return ScriptData.reload('Lookup.' + key, dynJS);
}

export async function reloadLookupAsync<TItem = any>(key: string, dynJS?: boolean): Promise<Lookup<TItem>> {
    return await ScriptData.reloadAsync('Lookup.' + key, dynJS);
}

export function getColumns(key: string, dynJS?: boolean): PropertyItem[] {
    return getColumnsData(key, dynJS).items;
}

export function getColumnsData(key: string, dynJS?: boolean): PropertyItemsData {
    return ScriptData.ensure('Columns.' + key, dynJS);
}

export async function getColumnsAsync(key: string, dynJS?: boolean): Promise<PropertyItem[]> {
    return (await getColumnsDataAsync(key, dynJS)).items;
}

export async function getColumnsDataAsync(key: string, dynJS?: boolean): Promise<PropertyItemsData> {
    return ScriptData.ensureAsync('Columns.' + key, dynJS);
}

export function getForm(key: string, dynJS?: boolean): PropertyItem[] {
    return getFormData(key, dynJS).items;
}

export function getFormData(key: string, dynJS?: boolean): PropertyItemsData {
    return ScriptData.ensure('Form.' + key, dynJS);
}

export async function getFormAsync(key: string, dynJS?: boolean): Promise<PropertyItem[]> {
    return (await getFormDataAsync(key, dynJS)).items;
}

export async function getFormDataAsync(key: string, dynJS?: boolean): Promise<PropertyItemsData> {
    return await ScriptData.ensureAsync('Form.' + key, dynJS);
}

export function getTemplate(key: string, dynJS?: boolean): string {
    return ScriptData.ensure('Template.' + key, dynJS);
}

export async function getTemplateAsync(key: string, dynJS?: boolean): Promise<string> {
    return await ScriptData.ensureAsync('Template.' + key, dynJS);
}

export function canLoadScriptData(name: string): boolean {
    return ScriptData.canLoad(name);
}

var exported = {
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
    getTemplateAsync,
    reloadLookup,
    reloadLookupAsync,
    ScriptData
}

if (typeof globalThis !== "undefined") {
    const Q = (globalThis as any).Q || ((globalThis as any).Q = {});
    for (var i in exported)
        if (Q[i] == null)
            Q[i] = (exported as any)[i];
}