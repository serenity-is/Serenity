
namespace Q {

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

        function syncLoadScript(url: string) {
            $.ajax({ async: false, cache: true, type: 'GET', url: url, data: null, dataType: 'script' });
        }

        function loadScriptAsync(url: string) {
            return Promise.resolve().then(function () {
                Q.blockUI(null);
                return Promise.resolve($.ajax({ async: true, cache: true, type: 'GET', url: url, data: null, dataType: 'script' }).always(function () {
                    Q.blockUndo();
                }));
            }, null);
        }

        function loadScriptData(name: string) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }

            name = name + '.js?' + registered[name];
            syncLoadScript(Q.resolveUrl('~/DynJS.axd/') + name);
        }

        function loadScriptDataAsync(name: string): PromiseLike<any> {
            return Promise.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
                }

                name = name + '.js?' + registered[name];
                return loadScriptAsync(Q.resolveUrl('~/DynJS.axd/') + name);
            }, null);
        }

        export function ensure(name: string) {
            var data = loadedData[name];
            if (data == null) {
                loadScriptData(name);
            }

            data = loadedData[name];

            if (data == null)
                throw new Error(Q.format("Can't load script data: {0}!", name));

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
                        throw new Error(Q.format("Can't load script data: {0}!", name));
                    }
                    return data;
                }, null);
            }, null);
        }

        export function reload(name: string) {
            if (registered[name] == null) {
                throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
            }
            registered[name] = (new Date()).getTime().toString();
            loadScriptData(name);
            var data = loadedData[name];
            return data;
        }

        export function reloadAsync(name: string) {
            return Promise.resolve().then(function () {
                if (registered[name] == null) {
                    throw new Error(Q.format('Script data {0} is not found in registered script list!', name));
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

    export function getLookup<TItem>(key: string): Lookup<TItem> {
        var name = 'Lookup.' + key;
        if (!ScriptData.canLoad(name)) {
            var message = 'No lookup with key "' + key + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + key + '")] attribute in server side code on top of a row / custom lookup and ' +
                ' its key is exactly the same.';
            Q.notifyError(message);
            throw new Error(message);
        }
        return ScriptData.ensure('Lookup.' + key);
    }

    export function getLookupAsync<TItem>(key: string): PromiseLike<Lookup<TItem>> {
        return ScriptData.ensureAsync('Lookup.' + key);
    }

    export function reloadLookup(key: string) {
        ScriptData.reload('Lookup.' + key);
    }

    export function reloadLookupAsync(key: string) {
        return ScriptData.reloadAsync('Lookup.' + key);
    }

    export function getColumns(key: string) {
        return ScriptData.ensure('Columns.' + key);
    }

    export function getColumnsAsync(key: string) {
        return ScriptData.ensureAsync('Columns.' + key);
    }

    export function getForm(key: string) {
        return ScriptData.ensure('Form.' + key);
    }

    export function getFormAsync(key: string) {
        return ScriptData.ensureAsync('Form.' + key);
    }

    export function getTemplate(key: string) {
        return ScriptData.ensure('Template.' + key);
    }

    export function getTemplateAsync(key: string) {
        return ScriptData.ensureAsync('Template.' + key);
    }

    export function canLoadScriptData(name: string) {
        return ScriptData.canLoad(name);
    }
}
