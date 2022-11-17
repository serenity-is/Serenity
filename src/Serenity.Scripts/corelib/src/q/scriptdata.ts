import { resolveUrl } from "./services";
import { startsWith, trimToNull } from "./strings";
import { blockUI, blockUndo } from "./blockui";
import { alert, iframeDialog } from "./dialogs";
import { notifyError } from "./notify";
import { format } from "./formatting";
import { PropertyItem, PropertyItemsData } from "./propertyitem";
import { getStateStore } from "./system";

function getHash(key: string, reload?: boolean): string {
    let k: string;

    const stateStore = getStateStore();
    if (stateStore.__hash == null &&
        typeof document !== "undefined" && 
        (k = trimToNull((document.querySelector('script#RegisteredScripts') || {}).innerHTML)) != null &&
        k.charAt(0) == '{') {
        var regs = JSON.parse(k);
        ScriptData.setRegisteredScripts(regs);
    }

    if (stateStore.__hash == null) {
        if (reload)
            stateStore.__hash = {};
        else
            return null;
    }

    if (reload)
        return (stateStore.__hash[key] = new Date().getTime().toString());

    return stateStore.__hash[key];
}

function setHash(key: string, value: string): void {
    const stateStore = getStateStore();
    stateStore.__hash = stateStore.__hash || (stateStore.__hash = {});
    stateStore.__hash[key] = value;
}

function getLoadedData(name: string): any {
    const stateStore = getStateStore();
    const loadedData = stateStore.__data || (stateStore.__data = {} );
    return loadedData[name];
}

function setLoadedData(name: string, value: any): void {
    const stateStore = getStateStore();
    const loadedData = stateStore.__data || ( stateStore.__data = {} );
    loadedData[name] = value;
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

    function loadOptions(name: string, async: boolean): JQueryAjaxSettings {
        return {
            async: async,
            cache: true,
            type: 'GET',
            url: resolveUrl('~/DynJS.axd/') + name + '.js?' + getHash(name),
            data: null,
            dataType: 'text',
            converters: {
                "text script": function (text: string) {
                    return text;
                }
            },
            success: function (data, textStatus, jqXHR) {
                $.globalEval(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                var isLookup = startsWith(name, "Lookup.");
                if (xhr.status == 403 && isLookup) {
                    notifyError('<p>Access denied while trying to load the lookup: "<b>' +
                        name.substr(7) + '</b>". Please check if current user has required permissions for this lookup.</p> ' +
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
                    (isLookup ? ' the lookup: "' + name.substr(7) :
                        ' dynamic script: "' + name) +
                    '"!. Please check the error message displayed in the dialog below for more info.');

                var html = xhr.responseText;
                if (!html) {
                    if (!xhr.status)
                        alert("An unknown connection error occurred! Check browser console for details.");
                    else if (xhr.status == 500)
                        alert("HTTP 500: Connection refused! Check browser console for details.");
                    else
                        alert("HTTP " + xhr.status + ' error! Check browser console for details.');
                }
                else
                    iframeDialog({ html: html });
            }
        }
    }

    async function loadScriptAsync(name: string): Promise<any> {
        blockUI(null);
        return await Promise.resolve(
            $.ajax(loadOptions(name, true))
                .always(function () {
                    blockUndo();
                }));
    }

    function loadScriptData(name: string) {
        $.ajax(loadOptions(name, false));
    }

    async function loadScriptDataAsync(name: string): Promise<any> {
        return await loadScriptAsync(name);
    }

    function loadError(name: string) {
        var message;

        if (name != null &&
            name.startsWith('Lookup.')) {
            message = 'No lookup with key "' + name.substring(7) + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + name.substring(7) + '")] attribute in server side code on top of a row / custom lookup and ' +
                ' its key is exactly the same.';
        } else {
            message = format("Can't load script data: {0}!", name);
        }

        notifyError(message);
        throw new Error(message);
    }

    export function ensure(name: string) {
        var data = getLoadedData(name);
        if (data == null) {
            loadScriptData(name);
        }

        data = getLoadedData(name);

        if (data == null) 
            loadError(name);

        return data;
    }

    export async function ensureAsync(name: string): Promise<any> {
        var data = getLoadedData(name);
        if (data != null)
            return data;
        await loadScriptDataAsync(name);
        data = getLoadedData(name);
        if (data == null)
            loadError(name);
        return data;
    }

    export function reload(name: string) {
        getHash(name, true);
        loadScriptData(name);
        var data = getLoadedData(name);
        return data;
    }

    export async function reloadAsync(name: string) {
        getHash(name, true);
        await loadScriptDataAsync(name);
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

export function getRemoteData(key: string) {
    return ScriptData.ensure('RemoteData.' + key);
}

export async function getRemoteDataAsync(key: string): Promise<any> {
    return await ScriptData.ensureAsync('RemoteData.' + key);
}

export function getLookup<TItem>(key: string): Q.Lookup<TItem> {
    return ScriptData.ensure('Lookup.' + key);
}

export async function getLookupAsync<TItem>(key: string): Promise<Q.Lookup<TItem>> {
    return await ScriptData.ensureAsync('Lookup.' + key);
}

export function reloadLookup(key: string) {
    ScriptData.reload('Lookup.' + key);
}

export async function reloadLookupAsync(key: string): Promise<any> {
    return await ScriptData.reloadAsync('Lookup.' + key);
}

export function getColumns(key: string): PropertyItem[] {
    return getColumnsData(key).items;
}

export function getColumnsData(key: string): PropertyItemsData {
    return ScriptData.ensure('Columns.' + key);
}

export async function getColumnsAsync(key: string): Promise<PropertyItem[]> {
    return (await getColumnsDataAsync(key)).items;
}

export async function getColumnsDataAsync(key: string): Promise<PropertyItemsData> {
    return ScriptData.ensureAsync('Columns.' + key);
}

export function getForm(key: string): PropertyItem[] {
    return getFormData(key).items;
}

export function getFormData(key: string): PropertyItemsData {
    return ScriptData.ensure('Form.' + key);
}

export async function getFormAsync(key: string): Promise<PropertyItem[]> {
    return (await getFormDataAsync(key)).items;
}

export async function getFormDataAsync(key: string): Promise<PropertyItemsData> {
    return await ScriptData.ensureAsync('Form.' + key);
}

export function getTemplate(key: string): string {
    return ScriptData.ensure('Template.' + key);
}

export async function getTemplateAsync(key: string): Promise<string> {
    return await ScriptData.ensureAsync('Template.' + key);
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
            Q[i] = exported[i];
}