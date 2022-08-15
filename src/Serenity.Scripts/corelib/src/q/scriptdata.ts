import { resolveUrl } from "./services";
import { startsWith, trimToNull } from "./strings";
import { blockUI, blockUndo } from "./blockui";
import { alert, iframeDialog } from "./dialogs";
import { notifyError } from "./notify";
import { format } from "./formatting";
import { PropertyItem } from "./propertyitem";

export namespace ScriptData {
    let _hash: { [key: string]: string };
    let loadedData: { [key: string]: any } = {};

    function getHash(key: string, reload?: boolean): string {
        let k: string;
        
        if (_hash == null &&
            typeof document !== "undefined" && 
            (k = trimToNull((document.querySelector('script#RegisteredScripts') || {}).innerHTML)) != null &&
            k.charAt(0) == '{') {
            var regs = JSON.parse(k);
            setRegisteredScripts(regs);
        }

        if (_hash == null && reload)
            _hash = {};

        if (reload)
            return (_hash[key] = new Date().getTime().toString());

        return _hash[key];
    }

    export function bindToChange(name: string, regClass: string, onChange: () => void) {
        ($(document.body) as any).bind('scriptdatachange.' + regClass, function (e: any, s: string) {
            if (s == name) {
                onChange();
            }
        });
    }

    export function triggerChange(name: string) {
        $(document.body).triggerHandler('scriptdatachange', [name]);
    }

    export function unbindFromChange(regClass: string) {
        $(document.body).unbind('scriptdatachange.' + regClass);
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

    function loadScriptAsync(name: string) {
        return Promise.resolve().then(function () {
            blockUI(null);
            return Promise.resolve(
                $.ajax(loadOptions(name, true))
                    .always(function () {
                        blockUndo();
                    }));
        }, null);
    }

    function loadScriptData(name: string) {
        $.ajax(loadOptions(name, false));
    }

    function loadScriptDataAsync(name: string): PromiseLike<any> {
        return Promise.resolve().then(function () {
            return loadScriptAsync(name);
        }, null);
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
        var data = loadedData[name];
        if (data == null) {
            loadScriptData(name);
        }

        data = loadedData[name];

        if (data == null) 
            loadError(name);

        return data;
    }

    export function ensureAsync(name: string): PromiseLike<any> {
        return Promise.resolve().then(function () {
            var data = loadedData[name];
            if (data != null) {
                return Promise.resolve(data);
            }

            return loadScriptDataAsync(name).then(function () {
                data = loadedData[name];
                if (data == null)
                    loadError(name);
                return data;
            }, null);
        }, null);
    }

    export function reload(name: string) {
        getHash(name, true);
        loadScriptData(name);
        var data = loadedData[name];
        return data;
    }

    export function reloadAsync(name: string) {
        return Promise.resolve().then(function () {
            getHash(name, true);
            return loadScriptDataAsync(name).then(function () {
                return loadedData[name];
            }, null);
        }, null);
    }

    export function canLoad(name: string) {
        return loadedData[name] != null || (_hash != null && _hash[name] != null);
    }

    export function setRegisteredScripts(scripts: any[]) {
        _hash = {};
        var t = new Date().getTime().toString();
        for (var k in scripts) {
            _hash[k] = scripts[k] || t;
        }
    }

    export function set(name: string, value: any) {
        loadedData[name] = value;
        triggerChange(name);
    }
}

export function getRemoteData(key: string) {
    return ScriptData.ensure('RemoteData.' + key);
}

export function getRemoteDataAsync(key: string) {
    return ScriptData.ensureAsync('RemoteData.' + key);
}

export function getLookup<TItem>(key: string): Q.Lookup<TItem> {
    var name = 'Lookup.' + key;
    return ScriptData.ensure('Lookup.' + key);
}

export function getLookupAsync<TItem>(key: string): PromiseLike<Q.Lookup<TItem>> {
    return ScriptData.ensureAsync('Lookup.' + key);
}

export function reloadLookup(key: string) {
    ScriptData.reload('Lookup.' + key);
}

export function reloadLookupAsync(key: string) {
    return ScriptData.reloadAsync('Lookup.' + key);
}

export function getColumns(key: string): PropertyItem[] {
    return ScriptData.ensure('Columns.' + key);
}

export function getColumnsAsync(key: string): PromiseLike<PropertyItem[]> {
    return ScriptData.ensureAsync('Columns.' + key);
}

export function getForm(key: string): PropertyItem[] {
    return ScriptData.ensure('Form.' + key);
}

export function getFormAsync(key: string): PromiseLike<PropertyItem[]> {
    return ScriptData.ensureAsync('Form.' + key);
}

export function getTemplate(key: string): string {
    return ScriptData.ensure('Template.' + key);
}

export function getTemplateAsync(key: string): PromiseLike<string> {
    return ScriptData.ensureAsync('Template.' + key);
}

export function canLoadScriptData(name: string): boolean {
    return ScriptData.canLoad(name);
}
