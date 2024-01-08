export function clearMockGlobals() {
    delete (window as any)["$"]
    delete (window as any)["jQuery"];
    delete (window as any)["flatpickr"];
    delete (window as any)["bootstrap"];
    delete (window as any)["Select2"];
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
                if (typeof handler === "function")
                    this._selector?.removeEventListener?.(ev, handler);
                return this;
            },
            on: function (ev: string, handler: () => void) {
                if (typeof handler === "function")
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

export function mockJQueryWithBootstrapModal(): any {
    let modal = jest.fn(function () {
        return this;
    }) as any;

    modal.Constructor = {
        VERSION: null
    };

    return mockJQuery({
        modal
    });
}

export function mockJQueryWithUIDialog(): any {
    let dialog = jest.fn(function () {
        return this;
    }) as any;

    let jQuery = mockJQuery({
        dialog
    });

    jQuery.ui = {
        dialog: dialog
    }

    return jQuery;
}


export function mockUndefinedJQuery() {
    delete (window as any)["jQuery"]
}

export function mockUndefinedBS5Plus() {
    delete (window as any)["bootstrap"]
}

export function mockEnvironmentWithBrowserDialogsOnly() {
    mockUndefinedBS5Plus();
    mockUndefinedJQuery();
}

export function mockBS5Plus() {
    let modal = jest.fn(function (div: HTMLElement) {
        return {
            show: jest.fn(function () {
                div.dataset.showCalls = (parseInt(div.dataset.showCalls ?? "0", 10) + 1).toString();
            }),
            hide: jest.fn(function () {
                div.dataset.hideCalls = (parseInt(div.dataset.hideClass ?? "0", 10) + 1).toString();
            })
        }
    }) as any;

    modal.VERSION = "5.3.2";
    modal.getInstance = function (el: HTMLElement) {
        if (el && el.dataset.options) {
            return {
                opt: JSON.parse(el.dataset.options),
                show: function () {
                    el.dataset.shown = (parseInt(el.dataset.shown ?? "0", 10) + 1).toString();
                },
                hide: function () {
                    el.dataset.hidden = (parseInt(el.dataset.hidden ?? "0", 10) + 1).toString();
                }
            }
        }
    };

    return ((window as any)["bootstrap"] = {
        Modal: modal
    });
}

function mockBS5PlusWithUndefinedJQuery() {
    mockUndefinedJQuery();
    return mockBS5Plus();
}