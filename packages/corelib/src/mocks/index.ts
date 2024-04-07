// @ts-ignore
import { readFileSync } from "fs";
// @ts-ignore
import { join, resolve } from "path";
import { resolveServiceUrl } from "../base/services";

const root = resolve('./');

const nscorelibPath = "~/out/index.global.js";

export function loadNSCorelib() {
    loadExternalScripts(nscorelibPath);
}

export function loadExternalScripts(...scripts: string[]) {
    scripts.forEach(path => {
        if (path.startsWith('~/'))
            path = join(root, path.substring(2));
        const src = readFileSync(path, 'utf8');
        const scriptEl = window.document.createElement("script");
        scriptEl.textContent = src;
        window.document.body.appendChild(scriptEl);
    });
}

export function mockJQuery(fn: any = {}) {
    let jQuery = function (selector: string | HTMLElement) {
        let result = {
            [0]: selector,
            length: 1,
            _selector: selector,
            _selectorHtml: typeof selector === 'string' ? selector : selector.outerHTML,
            html: function () {
                return <string>null;
            },
            eq: function () {
                return this;
            },
            get: function (i: number) {
                return i === 0 ? this._selector : null
            },
            find: function () {
                return this;
            },
            click: function () {
            },
            addClass: function (cls: string) {
                this._selector?.classList?.add?.(cls);
                return this;
            },
            appendTo: function () {
                return this;
            },
            attr: function (k: string, v: string) {
                if (typeof v === "undefined")
                    return this._selector?.getAttribute?.(k);
                this._selector?.setAttribute?.(k, v);
                return this;
            },
            off: function (ev: string, handler: () => void) {
                this._selector?.removeEventListener?.(ev, handler);
                return this;
            },
            on: function (ev: string, handler: () => void) {
                this._selector?.addEventListener?.(ev, handler);
                return this;
            },
            trigger: function (ev: Event) {
                this._selector?.dispatchEvent?.(typeof ev === "string" ? new Event(ev) : ev);
                return this;
            },
            triggerHandler: function (ev: any) {
                this._selector?.dispatchEvent?.(typeof ev === "string" ? new Event(ev) : ev);
                return this;
            },
            hasClass: function (cls: string) {
                return !!this._selector?.classList?.contains(cls);
            },
            one: function (ev: string, handler: () => void) {
                this._selector?.addEventListener?.(ev, handler);
                return this;
            },
            destroy: function () {
                return this;
            },
            remove: function () {
                return this;
            },
            jquery: "3.0.0"
        } as any;
        if (fn != null) {
            for (let x of Object.keys(fn)) {
                result[x] = fn[x];
            }
        }
        return result;
    } as any;

    jQuery.fn = fn;
    jQuery.Event = (name: string, prm: any) => {
        var ev: any = new Event(name);
        if (prm) {
            for (var i in prm) {
                if (i != "type" && i != "target") {
                    ev[i] = prm[i];
                }
            }
        }
        ev.isDefaultPrevented = () => false;
        ev.preventDefault = () => { ev.isDefaultPrevented = () => true; }
        return ev;
    }
    (window as any)["jQuery"] = jQuery;
    return jQuery;
}

export function unmockBSAndJQuery() {
    delete (window as any).$;
    delete (window as any).jQuery;
    delete (window as any).bootstrap;
}

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
var fetchSpy: jest.Mock<Promise<any>, [url: string, init: RequestInit], any> & { requests: MockFetchInfo[] }
var fetchMap: Record<string, (info: MockFetchInfo) => any> = {};

export function mockFetch(map?: { [urlOrService: string]: ((info: MockFetchInfo) => any) }) {
    if (!fetchSpy) {
        orgFetch = (window as any).fetch;
        fetchSpy = (window as any).fetch = jest.fn(async (url: string, init: RequestInit) => {
            var callback = fetchMap[url] ?? fetchMap["*"];
            if (!callback) {
                console.error(`Fetch is not configured on the mock fetch implementation: (${url})!`);
                throw `Fetch is not configured on the mock fetch implementation: (${url})!`;
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
            get: (x: string) => {
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
    public _info: MockFetchInfo;
    public _responseData: any;

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