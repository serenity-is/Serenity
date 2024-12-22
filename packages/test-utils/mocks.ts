import { ScriptData, scriptDataHooks } from "@serenity-is/corelib";
import { inject, Mock, vi } from "vitest";

let orgFetchScriptData: any;

export function mockDynamicData() {

    if (orgFetchScriptData != void 0)
        return;

    orgFetchScriptData = scriptDataHooks.fetchScriptData ?? null;
    scriptDataHooks.fetchScriptData = <TData>(name: string) => {
        try {
            // @ts-ignore
            const json = inject("dynamic-data/" + name.split("?")[0] + ".json" as any);
            if (json != null)
                return JSON.parse(json);
        }
        catch (e) {
            console.warn("Failed to load mock dynamic data for: " + name);
        }
    }
}

export function unmockDynamicData() {
    if (!orgFetchScriptData)
        return;

    scriptDataHooks.fetchScriptData = orgFetchScriptData == null ? void 0 : orgFetchScriptData;
}

import { resolveServiceUrl } from "@serenity-is/corelib";

export type MockFetchInfo = {
    url: string;
    init: RequestInit | BodyInit;
    data: any;
    status?: number;
    statusText?: string;
    aborted?: boolean;
    responseHeaders: Record<string, string>;
}

var orgFetch: any;
var fetchSpy: Mock<(url: string, init: RequestInit) => Promise<any>> & { requests: MockFetchInfo[] }
var fetchMap: Record<string, (info: MockFetchInfo) => any> = {};

export function mockFetch(map?: { [urlOrService: string]: ((info: MockFetchInfo) => any) }) {
    if (!fetchSpy) {
        orgFetch = (window as any).fetch;
        fetchSpy = (window as any).fetch = vi.fn(async (url: string, init: RequestInit) => {
            var callback = fetchMap[url] ?? fetchMap["*"];
            if (!callback) {
                console.error(`Mock fetch is not configured for URL: (${url})!`);
                throw `Mock fetch is not configured for URL: (${url})!`;
            }

            var requestData = typeof init.body == "string" ? JSON.parse(init.body) : null;

            var info: MockFetchInfo = {
                url: url,
                init: init,
                data: requestData,
                status: 200,
                aborted: false,
                responseHeaders: {
                    "content-type": "application/json"
                }
            }

            if (init && init.signal) {
                init.signal.addEventListener("abort", () => { info.aborted = true });
            }

            fetchSpy.requests.push(info);

            var responseData = callback(info);
            return new JsonResponse(responseData, info);
        }) as any;
        fetchSpy.requests = [];
    }

    if (map) {
        for (var key of Object.keys(map)) {
            var url = key == "*" ? "*" : resolveServiceUrl(key);
            fetchMap[url] = map[key];
        }
    }

    mockXHR();

    return fetchSpy;
}

export function unmockFetch() {
    if (fetchSpy !== null &&
        (window as any).fetch === fetchSpy) {
        fetchSpy = null;
        (window as any).fetch = orgFetch;
    }
    unmockXHR();
}

class JsonResponse {
    constructor(private response: any, private info: MockFetchInfo) {
    }

    get ok() { return this.info.status >= 200 && this.info.status < 300 }

    get headers() {
        return ({
            get: (x) => {
                return this.info.responseHeaders[x?.toLowerCase()];
            }
        })
    }

    get status() {
        return this.info.status;
    }

    get statusText() {
        return this.info.statusText ?? "";
    }

    text() {
        return typeof this.response === "string" ? this.response : JSON.stringify(this.response);
    }

    json() {
        return this.response;
    }
}

var xhrOriginal: any;

class MockXHR {
    declare public _info: MockFetchInfo;
    declare public _responseData: any;

    get status() { return this._info.status ?? 200; }
    get statusText() { return this._info.statusText; }
    get responseText() { return JSON.stringify(this._responseData) }

    getResponseHeader(name: string): string {
        return this._info?.responseHeaders[name];
    }

    open(_method: string, url: string, _async?: boolean): void {
        this._info ??= {} as any;
        this._info.url = url;
    }

    abort() {
        this._info ??= {} as any;
        this._info.aborted = true;
    }

    send(body?: Document | XMLHttpRequestBodyInit): void {
        var url = this._info?.url;
        var callback = fetchMap[url] ?? fetchMap["*"];
        if (!callback) {
            console.error(`URL is not configured on the mock XHR implementation: (${url})!`);
            throw `URL is not configured on the mock XHR implementation: (${url})!`;
        }

        var requestData = typeof body == "string" ? JSON.parse(body) : null;

        this._info = {
            url: url,
            init: body instanceof Document ? null : body,
            data: requestData,
            status: 200,
            aborted: false,
            responseHeaders: {
                "content-type": "application/json"
            }
        }

        fetchSpy.requests.push(this._info);

        this._responseData = callback(this._info);
    }

    setRequestHeader(name: string, value: string): void {
    }
}

function mockXHR() {
    xhrOriginal ??= window.XMLHttpRequest;
    window.XMLHttpRequest = MockXHR as any;
}

function unmockXHR() {
    if (xhrOriginal) {
        window["XMLHttpRequest"] = (xhrOriginal ?? window["XMLHttpRequest"]) as any;
        xhrOriginal = null;
    }
}

export function mockAdmin() {
    ScriptData.set("RemoteData.UserData", { Username: "admin", IsAdmin: true });
}

export function mockGridSize() {
    if (document.getElementById('mockGridSize'))
        return;
    var style = document.createElement('style')
    style.id = "mockGridSize";
    style.innerHTML = `
    .grid-container { min-height: 10000px !important; height: 10000px !important; min-width: 10000px !important; width: 10000px !important; }
    .slick-header.ui-state-default, .slick-headerrow.ui-state-default, .slick-footerrow.ui-state-default, .slick-group-header.ui-state-default { width: 100%; }
    .slick-header-columns, .slick-headerrow-columns, .slick-footerrow-columns, .slick-group-header-columns { position: relative; }
    .slick-header-column.ui-state-default, .slick-group-header-column.ui-state-default { position: relative;display: inline-block; height: 16px; line-height: 16px; float: left; }
    .slick-footerrow-column.ui-state-default { border-right: 1px solid silver; float: left; line-height: 20px; }
    .slick-resizable-handle { position: absolute; font-size: 0.1px; display: block; width: 4px; right: 0px; top: 0; height: 100%; }
    .grid-canvas { position: relative; }
    .slick-row.ui-widget-content, .slick-row.ui-state-active { position: absolute; width: 100%; }
    .slick-cell, .slick-headerrow-column, .slick-footerrow-column { position: absolute; }
    .slick-group-toggle { display: inline-block; }
    .slick-selection { z-index: 10; position: absolute; }
    .slick-pane { position: absolute; outline: 0; width: 100%; }
    .slick-pane-header { display: block; }
    .slick-header { position: relative; }
    .slick-headerrow { position: relative; }
    .slick-top-panel-scroller { position: relative; }
    .slick-top-panel { width: 10000px }
    .slick-viewport { position: relative; outline: 0; width: 10000px !important; height: 10000px !important; }
    .slick-row { height: 20px !important; width: 10000px !important; }
    .slick-cell { width: 30px !important; height: 20px !important; }
    `;

    document.head.appendChild(style);
}
