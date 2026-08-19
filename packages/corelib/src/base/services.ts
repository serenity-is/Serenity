import { blockUI, blockUndo } from "./blockui";
import { Config } from "./config";
import { getjQuery } from "./environment";
import { ErrorHandling } from "./errorhandling";
import { RequestErrorInfo, ServiceOptions, ServiceResponse } from "./servicetypes";

/**
 * Resolves a `~/`-prefixed application-relative URL against {@link Config.applicationPath}.
 * Non-tilde URLs are returned unchanged.
 * @param url - URL to resolve; may be `null`/`undefined` or already absolute.
 * @returns The resolved absolute / root-relative URL.
 */
export function resolveUrl(url: string) {
    if (url != null && url.charAt(0) == '~' && url.charAt(1) == '/')
        return Config.applicationPath + url.substring(2);
    return url;
}

/**
 * Resolves a Serenity service endpoint to a full URL.
 * Bare service keys like `"Administration/User/List"` are prefixed with `~/Services/`;
 * already rooted (`~/`, `/`) or absolute (`://`) URLs are resolved via {@link resolveUrl} unchanged.
 * @param url - Service key or URL.
 * @returns The resolved service URL.
 */
export function resolveServiceUrl(url: string) {
    if (url && url.length && url.charAt(0) != '~' && url.charAt(0) != '/' && url.indexOf('://') < 0)
        return resolveUrl("~/Services/") + url;

    return resolveUrl(url);
}

/**
 * Reads a cookie value by name.
 * Prefers jQuery's `$.cookie` when available, otherwise parses `document.cookie`.
 * @param name - Cookie name to look up.
 * @returns The cookie value, or `undefined` / empty string when not found.
 */
export function getCookie(name: string) {
    let $ = getjQuery();
    if (typeof $?.cookie === "function")
        return $.cookie(name);

    name += '=';
    for (let ca = document.cookie.split(/;\s*/), i = ca.length - 1; i >= 0; i--)
        if (!ca[i].indexOf(name))
            return ca[i].replace(name, '');
}

/**
 * Checks whether a URL is same-origin with the current page.
 * Used to decide whether to attach the `X-CSRF-TOKEN` header.
 * @param url - URL to test (absolute or relative; relative URLs are resolved against `window.location.origin`).
 * @returns `true` if the URL shares hostname, port and protocol with `window.location`.
 */
export function isSameOrigin(url: string) {
    const loc = window.location;
    try {
        const u = new URL(url, loc.origin);
        return u.hostname == loc.hostname &&
            u.port == loc.port &&
            u.protocol == loc.protocol;
    } catch {
        return false;
    }
}

/**
 * Normalizes and enriches a {@link ServiceOptions} object with defaults and derived values.
 * Applies default `method` (`POST`), `allowRedirect`/`async`/`blockUI` flags, resolves `service`/`url`,
 * and injects `Accept`, `Content-Type` and same-origin `X-CSRF-TOKEN` headers.
 * @typeParam TResponse - Expected service response type.
 * @param options - Raw service options supplied by the caller.
 * @returns The normalized options object with `url` resolved and `headers` populated.
 */
export function getServiceOptions<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>) {

    options = Object.assign(<ServiceOptions<TResponse>>{
        allowRedirect: true,
        async: true,
        blockUI: true,
        method: 'POST',
    }, options);

    const url = options.url = options.service ? resolveServiceUrl(options.service) : resolveUrl(options.url);
    delete options.service;
    options.headers ??= {};
    options.headers["Accept"] ??= "application/json";
    options.headers["Content-Type"] ??= "application/json";
    if (isSameOrigin(url)) {
        const token = getCookie('CSRF-TOKEN');
        if (token)
            options.headers["X-CSRF-TOKEN"] = token;
    }
    return options;
}

let activeRequests: number = 0;

/**
 * Signals that an AJAX / service request has started.
 * Increments the internal active-request counter and triggers `ajaxStart` on jQuery (if present) or dispatches an `ajaxStart` DOM event. Used internally by `serviceFetch` / `serviceCall` and script-data loaders.
 */
export function requestStarting() {
    activeRequests++;
    let $ = getjQuery();
    if ($ && typeof $.active === "number") {
        ($.active++ === 0) && $.event?.trigger?.("ajaxStart");
    }
    else if (activeRequests === 1) {
        typeof document !== "undefined" && document.dispatchEvent(new Event("ajaxStart"));
    }
}

/**
 * Signals that an AJAX / service request has finished.
 * Decrements the active-request counter and triggers `ajaxStop` when the count reaches zero.
 */
export function requestFinished() {
    activeRequests--;
    let $ = getjQuery();
    if ($ && typeof $.active === "number") {
        !(--$.active) && $.event?.trigger?.("ajaxStop");
    }
    else if (!activeRequests) {
        typeof document !== "undefined" && document.dispatchEvent(new Event("ajaxStop"));
    }
}

/**
 * Returns the number of currently active service / AJAX requests.
 * @returns Active request count (0 when idle).
 */
export function getActiveRequests() {
    return activeRequests;
}

function serviceFetch<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): Promise<TResponse> {

    if (typeof fetch !== "function")
        return Promise.reject(reason("The fetch method is not available!", "fetch-missing"));

    return (async function () {

        options = getServiceOptions(options);
        const url = options.url;

        requestStarting();
        try {
            options.blockUI && blockUI();

            try {
                let {
                    allowRedirect: _1,
                    async: _2,
                    blockUI: _3,
                    request: _4,
                    service: _5,
                    url: _6,
                    onCleanup: _7,
                    onError: _8,
                    onSuccess: _9,
                    ...fetchInit
                } = options;

                fetchInit.body = JSON.stringify(options.request);

                let fetchResponse: Response;
                try {
                    fetchResponse = await fetch(url, fetchInit);
                }
                catch (ex) {
                    if (ex.name === "AbortError") {
                        return Promise.reject(reason(`Service fetch to '${url}' was aborted!`,
                            "abort", { cause: ex, url }));
                    }

                    throw ex;
                }

                if (!fetchResponse.ok) {
                    await handleFetchError(fetchResponse, options);
                    return Promise.reject(reason(`Service fetch to '${url}' resulted in HTTP ${fetchResponse.status} error: ${fetchResponse.statusText}!`,
                        "http-error", { fetchResponse, url }));
                }

                let response = await fetchResponse.json() as TResponse;
                if (!response)
                    return Promise.reject(reason(`Received empty response from service fetch to '${url}'!`,
                        "empty-response", { fetchResponse, url }));

                if (response.Error) {
                    handleError(response, { status: fetchResponse.status, statusText: fetchResponse.statusText }, options);
                    return Promise.reject(reason(`Service fetch to '${url}' resulted in error: ${response.Error.Message ?? response.Error.Code}!`,
                        "service-error", { response, fetchResponse, url }));
                }

                options.onSuccess?.(response);
                return response;
            }
            finally {
                options.blockUI && blockUndo();
                options.onCleanup && options.onCleanup();
            }
        }
        finally {
            requestFinished();
        }
    })();
}

function reason(message: string, kind: string, extra?: any) {
    let error: Error;
    if (extra?.cause != null) {
        error = (Error as any)(message, { cause: extra.cause });
    }
    else {
        error = Error(message);
    }
    if (kind != null) {
        (error as any).kind = kind;
    }
    (error as any).origin = "serviceCall";
    if (extra != null) {
        if ((error as any).cause)
            delete extra.cause;
        Object.assign(error, extra);
    }

    return error;
}

/**
 * Executes a Serenity service call.
 * Prefers `fetch` (async) when `options.async` is `true` (default); falls back to synchronous `XMLHttpRequest` when `async` is `false` (blocks the UI — avoid in new code).
 * Handles CSRF headers, `blockUI`, redirect handling, and delegates error display to the global error handler unless suppressed via `errorMode` or `onError`.
 * @typeParam TResponse - Service response type (extends {@link ServiceResponse}).
 * @param options - Service options including `service`/`url`, `request` payload and callbacks.
 * @returns A `PromiseLike` that resolves with the parsed response on success or rejects with an enriched `Error` (with `kind` and `origin === "serviceCall"`) on failure.
 */
export function serviceCall<TResponse extends ServiceResponse>(options: ServiceOptions<TResponse>): PromiseLike<TResponse> {

    if (options?.async ?? true)
        return serviceFetch(options);

    let url: string;
    return new Promise((resolve, reject) => {
        try {
            options = getServiceOptions(options);
            url = options.url;

            const xhr = new XMLHttpRequest();
            xhr.open(options.method, url, false);

            if (options.cache == "no-store")
                options.headers["Cache-Control"] ??= "no-cache, no-store, max-age=0";
            else if (options.cache === "no-cache")
                options.headers["Cache-Control"] ??= "no-cache";

            for (const x in options.headers) {
                xhr.setRequestHeader(x, options.headers[x]);
            }

            requestStarting();
            try {
                if (options.signal) {
                    options.signal.addEventListener("abort", () => {
                        xhr.abort();
                    }, { once: true });
                }

                xhr.send(JSON.stringify(options.request));
                try {
                    if (xhr.status !== 200) {
                        handleXHRError(xhr, options);
                        return reject(reason(`Service call to '${url}' resulted in HTTP ${xhr.status} error: ${xhr.statusText}!`,
                            "http-error", { status: xhr.status, statusText: xhr.statusText, url }));
                    }

                    let response = JSON.parse(xhr.responseText) as TResponse;
                    if (!response)
                        return reject(reason(`Empty response received on service call to '${url}'!`,
                            "empty-response", { url }));

                    if (response.Error) {
                        handleError(response, { status: xhr.status, statusText: xhr.statusText }, options);
                        return reject(reason(`Service call to '${url}' resulted in error: ${response.Error.Message ?? response.Error.Code}!`,
                            "service-error", { response, url }));
                    }

                    options.onSuccess?.(response);
                    return resolve(response);
                }
                finally {
                    options.blockUI && blockUndo();
                    options.onCleanup && options.onCleanup();
                }
            }
            finally {
                requestFinished();
            }
        }
        catch (exception) {
            reject(reason(`Service call to '${url}' thrown exception: ${exception.toString()}`,
                "exception", { cause: exception, url }));
        }
    });
}

/**
 * Convenience wrapper around {@link serviceCall} that takes a service key as the first argument.
 * @typeParam TResponse - Service response type.
 * @param service - Service endpoint key (e.g. `"Administration/User/List"`) or full URL.
 * @param request - Request DTO serialized as the POST body.
 * @param onSuccess - Optional success callback invoked with the response before the promise resolves.
 * @param options - Additional {@link ServiceOptions} merged with the above (e.g. `errorMode`, `blockUI`, `signal`).
 * @returns A `PromiseLike` resolving to the service response.
 */
export function serviceRequest<TResponse extends ServiceResponse>(service: string, request?: any,
    onSuccess?: (response: TResponse) => void, options?: ServiceOptions<TResponse>): PromiseLike<TResponse> {
    return serviceCall(Object.assign({
        service: service,
        request: request,
        onSuccess: onSuccess
    }, options));
}

function handleError(response: any, errorInfo: RequestErrorInfo, options: ServiceOptions<any>) {

    if (Config.notLoggedInHandler != null &&
        response &&
        response.Error &&
        response.Error.Code == 'NotLoggedIn' &&
        Config.notLoggedInHandler(options, response)) {
        return;
    }

    if (options?.onError?.(response, errorInfo) ||
        options?.errorMode === 'none')
        return;

    ErrorHandling.showServiceError(response?.Error, errorInfo, options?.errorMode);
};

function handleRedirect(getHeader: (key: string) => string): boolean {
    let l: any = null;
    try {
        l = getHeader('Location');
    }
    catch ($t1) {
        l = null;
    }
    if (l) {
        window.top.location.href = l;
        return true;
    }
}

async function handleFetchError(response: Response, options: ServiceOptions<any>): Promise<void> {

    if (response.status === 403 && options.allowRedirect && handleRedirect(response.headers.get.bind(response.headers)))
        return;

    if ((response.headers.get('content-type') || '').toLowerCase().indexOf('json') >= 0) {
        const json = (await response.json()) as ServiceResponse;
        if (json && json.Error) {
            handleError(json, {
                status: response.status,
                statusText: response.statusText
            }, options);
            return;
        }
    }

    handleError(null, {
        status: response.status,
        statusText: response.statusText,
        responseText: await response.text()
    }, options);
}

function handleXHRError(xhr: XMLHttpRequest, options: ServiceOptions<any>) {
    if (xhr.status === 403 && options.allowRedirect && handleRedirect(xhr.getResponseHeader.bind(xhr)))
        return;

    if ((xhr.getResponseHeader('content-type') || '')
        .toLowerCase().indexOf('application/json') >= 0) {
        const json = JSON.parse(xhr.responseText);
        if (json && json.Error) {
            handleError(json, { status: xhr.status, statusText: xhr.statusText }, options);
            return;
        }
    }

    handleError(null, { status: xhr.status, statusText: xhr.statusText, responseText: xhr.responseText }, options);
}