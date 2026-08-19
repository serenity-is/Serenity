import { blockUI, blockUndo } from "./blockui";
import { htmlEncode } from "./html";
import { Lookup } from "./lookup";
import { notifyError } from "./notify";
import { PropertyItemsData } from "./propertyitem";
import { requestFinished, requestStarting, resolveUrl } from "./services";
import { scriptDataHashSymbol, scriptDataSymbol } from "./symbols";
import { getGlobalObject, isPromiseLike } from "./system";

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
        const regs = JSON.parse(json);
        getGlobalObject()[scriptDataHashSymbol] = scriptDataHash = {};
        for (const i in regs) {
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

/**
 * Global hooks for script-data loading.
 * Allows tests or custom bootstrapping to intercept `fetchScriptData` / `ensureScriptDataSync`.
 * When the hook returns `undefined` the default `fetch` / XHR implementation is used.
 */
export const scriptDataHooks = {
    /**
     * Override for script-data fetching.
     * Return a value / promise to short-circuit the default loader, or `undefined` to fall back.
     * @param name - Dynamic script name (e.g. `"Lookup.MyLookup"`, `"Form.MyForm"`).
     * @param sync - When true the caller expects a synchronous result (legacy compat path). The hook must return data directly, not a promise.
     * @param dynJS - When true the script was requested as a legacy `DynJS.axd` JavaScript payload rather than JSON. Only relevant when `sync` is true.
     * @returns The script data directly (sync) or a promise of it, or `undefined` to use the default fetch.
     */
    fetchScriptData: void 0 as <TData>(name: string, sync?: boolean, dynJS?: boolean) => TData | Promise<TData>
}

let fetchPromises: { [key: string]: Promise<any> } = {}

/**
 * Fetches a dynamic script payload by name via the `~/DynamicData/` endpoint.
 * Results are de-duplicated per `name + hash` while a request is in flight and the request
 * participates in global AJAX / block-UI tracking. Lookup payloads are wrapped as {@link Lookup} instances.
 * @typeParam TData - Expected shape of the returned payload.
 * @param name - Dynamic script name (e.g. `"Lookup.Administration.User"`, `"Form.MyForm"`, `"RemoteData.MyData"`).
 * @returns A promise that resolves with the parsed payload, or rejects if fetch is unavailable or the HTTP request fails.
 */
export function fetchScriptData<TData>(name: string): Promise<TData> {

    const hookResult = scriptDataHooks.fetchScriptData?.<TData>(name);
    if (hookResult != void 0) {
        return Promise.resolve(hookResult).then(data => {
            if (name?.startsWith("Lookup.") && (data as any)?.Items && !(data as any).items && !(data as any).itemById)
                return new Lookup((data as any).Params, (data as any).Items) as any;
            return data;
        });
    }

    let key = name + '?' + (getScriptDataHash(name) ?? '');

    const promise: Promise<TData> = fetchPromises[key];
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
                    const response = await fetch(url, {
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
 * Returns cached script data if available, otherwise fetches it via `~/DynamicData/` and caches the result.
 * @typeParam TData - Expected payload type.
 * @param name - Dynamic script name.
 * @param reload - When true, busts the hash cache, clears the in-memory entry and forces a fresh fetch.
 * @returns A promise resolving to the script data.
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
 * Synchronous (blocking) version of {@link getScriptData} for legacy compatibility.
 * Avoid in new code — it performs a synchronous XHR and blocks the UI thread.
 * @typeParam TData - Expected payload type.
 * @param name - Dynamic script name.
 * @param dynJS - When true loads via `~/DynJS.axd/*.js` and evaluates the returned script instead of JSON. Legacy path only.
 * @returns The script data (wrapped as {@link Lookup} for `Lookup.*` keys).
 * @throws If the hook returns a promise in sync mode or the HTTP request fails.
 */
export function ensureScriptDataSync<TData = any>(name: string, dynJS?: boolean): TData {
    let data = peekScriptData(name);
    if (data != null)
        return data;

    data = scriptDataHooks.fetchScriptData?.<TData>(name, true, dynJS);
    if (data !== void 0) {
        if (isPromiseLike(data))
            throw new Error("fetchScriptData hook must return data synchronously when sync is true.");
        if (name.startsWith("Lookup.") && (data as any)?.Items)
            data = new Lookup((data as any).Params, (data as any).Items) as any;
        setScriptData(name, data);
        return data;
    }

    const url = resolveUrl(dynJS ? '~/DynJS.axd/' : '~/DynamicData/') + name + (dynJS ? '.js' : '') + '?v=' + (getScriptDataHash(name) ?? new Date().getTime());
    const xhr = new XMLHttpRequest();
    xhr.open("GET", url, false);
    requestStarting();
    try {

        xhr.send(null);
        if (xhr.status !== 200)
            handleScriptDataError(name, xhr.status, xhr.statusText);

        if (dynJS) {
            const script = document.createElement("script");
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
        setScriptData(name, data);
        return data;
    }
    finally {
        requestFinished();
    }
}

/**
 * Loads a `ColumnsScript` bundle for the given key.
 * @param key - Columns key (usually the row type name, e.g. `"Administration.User"`).
 * @returns A promise resolving to a {@link PropertyItemsData} containing `items` and `additionalItems` for grid columns.
 */
export function getColumnsScript(key: string): Promise<PropertyItemsData> {
    return getScriptData<PropertyItemsData>('Columns.' + key);
}

/**
 * Loads a `FormScript` bundle for the given key.
 * @param key - Form key (usually the row/form type name, e.g. `"Administration.User"`).
 * @returns A promise resolving to a {@link PropertyItemsData} describing the form fields.
 */
export function getFormScript(key: string): Promise<PropertyItemsData> {
    return getScriptData<PropertyItemsData>('Form.' + key);
}

/**
 * Loads a lookup by key.
 * @typeParam TItem - Row type of the lookup items.
 * @param key - Lookup key as registered server-side via `[LookupScript]` (e.g. `"Administration.User"`).
 * @returns A promise resolving to the {@link Lookup} instance.
 */
export function getLookupAsync<TItem>(key: string): Promise<Lookup<TItem>> {
    return getScriptData<Lookup<TItem>>('Lookup.' + key);
}

/**
 * Loads a `RemoteData` script by key.
 * @typeParam TData - Expected payload type.
 * @param key - Remote data key as registered server-side via `[RemoteDataScript]`.
 * @returns A promise resolving to the remote data payload.
 */
export function getRemoteDataAsync<TData = any>(key: string): Promise<TData> {
    return getScriptData<TData>('RemoteData.' + key);
}

/**
 * Synchronous version of {@link getRemoteDataAsync} for legacy compatibility. Blocks the UI thread.
 * @typeParam TData - Expected payload type.
 * @param key - Remote data key.
 * @returns The remote data payload.
 */
export function getRemoteData<TData = any>(key: string): TData {
    return ensureScriptDataSync('RemoteData.' + key);
}

/**
 * Shows a suitable error message for errors occured during loading of
 * a dynamic script data.
 * @param name Name of the dynamic script
 * @param status HTTP status returned if available
 * @param statusText HTTP status text returned if available
 */
export function handleScriptDataError(name: string, status?: number, statusText?: string, shouldThrow = true): string {

    const isLookup = name?.startsWith("Lookup.");
    let message: string;
    if ((status == null && statusText == null) || (status === 404)) {
        if (isLookup)
            message = 'No lookup with key "' + name.substring(7) + '" is registered. Please make sure you have a' +
                ' [LookupScript("' + name.substring(7) + '")] attribute in server side code on top of a row / custom lookup and' +
                ' its key is exactly the same.';
        else
            message = `Cannot load dynamic data: ${name}!`;

        notifyError(message);
    }
    else if (status == 403 && isLookup) {
        message = 'Access denied while trying to load the lookup: "' +
            htmlEncode(name.substring(7)) + '". Please check if current user has required permissions for this lookup.\n\n' +
            'Lookups use the ReadPermission of their row by default. You may override that for the lookup ' +
            'like [LookupScript("Some.Lookup", Permission = "?")] to grant all ' +
            'authenticated users to read it (or use "*" for public).\n\n' +
            'Note that this might be a security risk if the lookup contains sensitive data, ' +
            'so it could be better to set a separate permission for lookups, like "MyModule:Lookups".';

        notifyError(message, null, {
            timeOut: 10000,
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

/**
 * Returns cached script data without triggering a fetch.
 * @param name - Dynamic script name.
 * @returns The cached value or `undefined` if not loaded yet.
 */
export function peekScriptData(name: string): any {
    return getGlobalObject()[scriptDataSymbol]?.[name];
}

/**
 * Forces a reload of a lookup from the server, bypassing the client-side cache.
 * Note this only clears the browser cache entry; it does not invalidate server-side caches.
 * @typeParam TItem - Row type of the lookup items.
 * @param key - Lookup key to reload.
 * @returns A promise resolving to the freshly loaded {@link Lookup}.
 */
export async function reloadLookupAsync<TItem = any>(key: string): Promise<Lookup<TItem>> {
    return await getScriptData('Lookup.' + key, true);
}

/**
 * Seeds or updates the known script hashes (normally populated from the `RegisteredScripts` script tag).
 * Useful in tests or when bootstrapping hashes manually.
 * @param scripts - Map of script name to hash string. Falsy hash values are replaced with the current timestamp.
 */
export function setRegisteredScripts(scripts: Record<string, string>) {
    const t = new Date().getTime().toString();
    let scriptDataHash = getGlobalObject()[scriptDataHashSymbol];
    if (!scriptDataHash)
        getGlobalObject()[scriptDataHashSymbol] = scriptDataHash = {};
    for (const k in scripts) {
        scriptDataHash[k] = scripts[k] || t;
    }
}

/**
 * Stores a script data value in the in-memory cache and dispatches a `scriptdatachange.<name>` DOM event.
 * @param name - Dynamic script name.
 * @param value - Value to cache (use `undefined` to clear).
 */
export function setScriptData(name: string, value: any) {
    let scriptDataStore = getGlobalObject()[scriptDataSymbol];
    if (!scriptDataStore)
        getGlobalObject()[scriptDataSymbol] = scriptDataStore = {};
    scriptDataStore[name] = value;
    typeof document !== "undefined" && document.dispatchEvent?.(new Event("scriptdatachange." + name));
}

