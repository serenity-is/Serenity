// @ts-ignore
import { readFileSync } from "fs";
// @ts-ignore
import { join, resolve } from "path";

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