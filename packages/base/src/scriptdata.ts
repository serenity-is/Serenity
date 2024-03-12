import { blockUI, blockUndo } from "./blockui";
import { htmlEncode } from "./html";
import { Lookup } from "./lookup";
import { notifyError } from "./notify";
import { PropertyItemsData } from "./propertyitem";
import { requestFinished, requestStarting, resolveUrl } from "./services";
import { scriptDataHashSymbol, scriptDataSymbol } from "./symbols";
import { getGlobalObject } from "./system";

/**
 * Gets the known hash value for a given dynamic script name. They are usually
 * registered server-side via dynamic script manager and their latest known
 * hashes are passed to the client-side via a script element named RegisteredScripts.
 * @param name The dynamic script name
 * @param reload True to force resetting the script hash client side, e.g. for loading
 * lookups etc.
 * @returns The hash or null if no such known registration
 */
export function getScriptDataHash(name: string, reload?: boolean): string {
    let json: string;

    let scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
    if (!scriptDataHash &&
        typeof document !== "undefined" &&
        (json = (document.querySelector('script#RegisteredScripts')?.innerHTML ?? '').trim()) &&
        json.charAt(0) == '{') {
        var regs = JSON.parse(json);
        getGlobalObject()[scriptDataHashSymbol] = scriptDataHash = {};
        for (var i in regs) {
            scriptDataHash[i] = regs[i];
        }
    }

    if (!scriptDataHash) {
        if (reload)
            getGlobalObject()[scriptDataHashSymbol] = scriptDataHash = {};
        else
            return null;
    }

    if (reload)
        return (scriptDataHash[name] = new Date().getTime().toString());

    return scriptDataHash[name];
}

let fetchPromises: { [key: string]: Promise<any> } = {}

/**
 * Fetches a script data with given name via ~/DynamicData endpoint
 * @param name Dynamic script name
 * @returns A promise that will return data if successfull
 */
export function fetchScriptData<TData>(name: string): Promise<TData> {
    let key = name + '?' + (getScriptDataHash(name) ?? '');

    var promise: Promise<TData> = fetchPromises[key];
    if (promise != null)
        return promise;

    if (typeof fetch === "undefined")
        return Promise.reject("The fetch method is not available!");

    return fetchPromises[key] = (async function () {
        try {
            blockUI();
            try {
                let url = resolveUrl('~/DynamicData/') + name + '?v=' +
                    (getScriptDataHash(name) ?? new Date().getTime());

                requestStarting();
                try {
                    var response = await fetch(url, {
                        method: 'GET',
                        cache: "force-cache",
                        headers: {
                            "Accept": "application/json"
                        }
                    });

                    if (!response.ok)
                        handleScriptDataError(name, response.status, response.statusText ?? '');

                    const data = await response.json();
                    if (name.startsWith("Lookup."))
                        return new Lookup(data.Params, data.Items);
                    return data;
                }
                finally {
                    requestFinished();
                }
            }
            finally {
                blockUndo();
            }
        }
        finally {
            delete fetchPromises[key];
        }
    })();
}

/**
 * Returns the script data from cache if available, or via a fetch
 * request to ~/DynamicData endpoint
 * @param name 
 * @param reload Clear cache and force reload
 * @returns 
 */
export async function getScriptData<TData = any>(name: string, reload?: boolean): Promise<TData> {
    let data: any;
    if (reload) {
        getScriptDataHash(name, true);
        setScriptData(name, undefined);
    }
    else if ((data = peekScriptData(name)) != null)
        return data;

    data = await fetchScriptData<TData>(name);
    setScriptData(name, data);
    return data;
}

/**
 * Gets or loads a [ColumnsScript] data 
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
export async function getColumnsScript(key: string): Promise<PropertyItemsData> {
    return getScriptData<PropertyItemsData>('Columns.' + key);
}

/**
 * Gets or loads a [FormScript] data 
 * @param key Form key
 * @returns A property items data object containing items and additionalItems properties
 */
export async function getFormScript(key: string): Promise<PropertyItemsData> {
    return await getScriptData<PropertyItemsData>('Form.' + key);
}

/**
 * Gets or loads a Lookup
 * @param key Lookup key
 */
export async function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>> {
    return await getScriptData<Lookup<TItem>>('Lookup.' + key);
}

/**
 * Gets or loads a [RemoteData]
 * @param key Remote data key
 */
export async function getRemoteDataAsync<TData = any>(key: string): Promise<TData> {
    return await getScriptData<TData>('RemoteData.' + key);
}

/**
 * Shows a suitable error message for errors occured during loading of
 * a dynamic script data.
 * @param name Name of the dynamic script
 * @param status HTTP status returned if available
 * @param statusText HTTP status text returned if available
 */
export function handleScriptDataError(name: string, status?: number, statusText?: string, shouldThrow = true): string {

    var isLookup = name?.startsWith("Lookup.");
    var message: string;
    if ((status == null && statusText == null) || (status === 404)) {
        if (isLookup)
            message = 'No lookup with key "' + name.substring(7) + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + name.substring(7) + '")] attribute in server side code on top of a row / custom lookup and ' +
                ' its key is exactly the same.';
        else
            message = `Can't load dynamic data: ${name}!`;

        notifyError(message);
    }
    else if (status == 403 && isLookup) {
        message = '<p>Access denied while trying to load the lookup: "<b>' +
            htmlEncode(name.substring(7)) + '</b>". Please check if current user has required permissions for this lookup.</p> ' +
            '<p><em>Lookups use the ReadPermission of their row by default. You may override that for the lookup ' +
            'like [LookupScript("Some.Lookup", Permission = "?")] to grant all ' +
            'authenticated users to read it (or use "*" for public).</em></p>' +
            '<p><em>Note that this might be a security risk if the lookup contains sensitive data, ' +
            'so it could be better to set a separate permission for lookups, like "MyModule:Lookups".</em></p>';

        notifyError(message, null, {
            timeOut: 10000,
            escapeHtml: false
        });
    }
    else {
        message = "An error occurred while trying to load" +
            (isLookup ? ' the lookup: "' + name.substring(7) : ' dynamic data: "' + name) +
            '"!. Please check the error message displayed in the console for more info.';
        notifyError(message);

        if (!status)
            console.log("An unknown connection error occurred!");
        else if (status == 500)
            console.log("HTTP 500: Connection refused!");
        else
            console.log("HTTP " + status + ': ' + statusText);
    }

    if (shouldThrow)
        throw message;

    return message;
}

export function peekScriptData(name: string): any {
    return getGlobalObject()[scriptDataSymbol]?.[name];
}

/**
 * Forces reload of a lookup from the server. Note that only the
 * client side cache is cleared. This does not force reloading in the server-side.
 * @param key Lookup key
 * @returns Lookup
 */
export async function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>> {
    return await getScriptData('Lookup.' + key, true);
}

export function setRegisteredScripts(scripts: any[]) {
    var t = new Date().getTime().toString();
    var scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
    if (!scriptDataHash)
        getGlobalObject()[scriptDataHashSymbol] = scriptDataHash = {};
    for (var k in scripts) {
        scriptDataHash[k] = scripts[k] || t;
    }
}

export function setScriptData(name: string, value: any) {
    let scriptDataStore = getGlobalObject()[scriptDataSymbol];
    if (!scriptDataStore)
        getGlobalObject()[scriptDataSymbol] = scriptDataStore = {};
    scriptDataStore[name] = value;
    typeof document !== "undefined" && document.dispatchEvent?.(new Event("scriptdatachange." + name));
}

