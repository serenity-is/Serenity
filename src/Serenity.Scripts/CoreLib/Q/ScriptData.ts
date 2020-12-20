import { resolveUrl } from "./Services";
import { startsWith } from "./Strings";
import { blockUI, blockUndo } from "./BlockUI";
import { alert, iframeDialog } from "./Dialogs";
import { notifyError } from "./Notify";
import { format } from "./Formatting";

export namespace ScriptData {
    let registered: { [key: string]: any } = {};
    let loadedData: { [key: string]: any } = {};

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
            url: resolveUrl('~/DynJS.axd/') + name + '.js?' + registered[name],
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
        if (registered[name] == null) {
            throw new Error(format('Script data {0} is not found in registered script list!', name));
        }

        $.ajax(loadOptions(name, false));
    }

    function loadScriptDataAsync(name: string): PromiseLike<any> {
        return Promise.resolve().then(function () {
            if (registered[name] == null) {
                throw new Error(format('Script data {0} is not found in registered script list!', name));
            }

            return loadScriptAsync(name);
        }, null);
    }

    export function ensure(name: string) {
        var data = loadedData[name];
        if (data == null) {
            loadScriptData(name);
        }

        data = loadedData[name];

        if (data == null)
            throw new Error(format("Can't load script data: {0}!", name));

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
                if (data == null) {
                    throw new Error(format("Can't load script data: {0}!", name));
                }
                return data;
            }, null);
        }, null);
    }

    export function reload(name: string) {
        if (registered[name] == null) {
            throw new Error(format('Script data {0} is not found in registered script list!', name));
        }
        registered[name] = (new Date()).getTime().toString();
        loadScriptData(name);
        var data = loadedData[name];
        return data;
    }

    export function reloadAsync(name: string) {
        return Promise.resolve().then(function () {
            if (registered[name] == null) {
                throw new Error(format('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            return loadScriptDataAsync(name).then(function () {
                return loadedData[name];
            }, null);
        }, null);
    }

    export function canLoad(name: string) {
        return (loadedData[name] != null || registered[name] != null);
    }

    export function setRegisteredScripts(scripts: any[]) {
        registered = {};
        var t = new Date().getTime();
        for (var k in scripts) {
            registered[k] = scripts[k] || t;
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
    if (!ScriptData.canLoad(name)) {
        var message = 'No lookup with key "' + key + '" is registered. Please make sure you have a' +
            ' [LookupScript("' + key + '")] attribute in server side code on top of a row / custom lookup and ' +
            ' its key is exactly the same.';
        notifyError(message);
        throw new Error(message);
    }
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

export function getColumns(key: string): Serenity.PropertyItem[] {
    return ScriptData.ensure('Columns.' + key);
}

export function getColumnsAsync(key: string): PromiseLike<Serenity.PropertyItem[]> {
    return ScriptData.ensureAsync('Columns.' + key);
}

export function getForm(key: string): Serenity.PropertyItem[] {
    return ScriptData.ensure('Form.' + key);
}

export function getFormAsync(key: string): PromiseLike<Serenity.PropertyItem[]> {
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
