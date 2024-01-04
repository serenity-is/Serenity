import { toggleClass } from "@serenity-is/base";
import jQuery from "@optionaldeps/jquery";

class sQ extends Array<HTMLElement> {

    jquery: "squery";
    isMock: true;

    eq(i: number) {
        return SQ(this[i]);
    }

    get(i: number) {
        return this[i];
    }

    ready(cb: () => void) {
        SQ.ready(cb);
    }

    first() {
        return SQ([this[0]]);
    }

    addClass(className: string) {
        this.forEach(el => { toggleClass(el, className, true); });
        return this;
    }

    removeClass(className: string) {
        this.forEach(el => { toggleClass(el, className, false); });
        return this;
    }

    toggleClass(className: string, toggle?: boolean) {
        this.forEach(el => { toggleClass(el, className, toggle); });
        return this;
    }

    hasClass(className: string): boolean {
        if (!this.length) {
            return false;
        }
        return !this.find((el: any) => !el.classList.contains(className));
    }

    after(content: any) {
        return this.manipulate(content, 'afterend', HTMLElement.prototype.after);
    }

    before(content: any) {
        return this.manipulate(content, 'beforebegin', HTMLElement.prototype.before);
    }

    append(content: any) {
        return this.manipulate(content, 'beforeend', HTMLElement.prototype.append);
    }

    prepend(content: any) {
        return this.manipulate(content, 'afterbegin', HTMLElement.prototype.prepend);
    }

    manipulate(content: any, place: any, func: Function) {
        this.forEach(el => {
            if (typeof content === "string") {
                el.insertAdjacentHTML(place, content);
            } else if (content instanceof sQ) {
                func.apply(el, content);
            } else if (content instanceof Node) {
                func.call(el, content);
            }
        });

        return this;
    }

    text() {
        return this.reduce((a, v) => a + v.textContent, '');
    }

    val(value: any) {
        if (typeof value === 'undefined') {
            switch (this.length) {
                case 0: return undefined;
                case 1: return (this[0] as HTMLInputElement).value;
                default: return '';
            }
        }

        this.forEach(el => { (el as HTMLInputElement).value = value; })

        return this;
    }

    attr(name: string, value: string) {
        if (typeof value === 'undefined') {
            return this.length ? this[0].getAttribute(name) : undefined;
        }

        this.forEach(el => { el.setAttribute(name, value); });
        return this;
    }

    removeAttr(name: string) {
        this.forEach(el => { el.removeAttribute(name); });
        return this;
    }

    html(html: string) {
        if (typeof html === 'undefined') {
            return this.reduce((a, v) => a + v.innerHTML, '');
        }

        this.forEach(el => { el.innerHTML = html; });

        return this;
    }

    each(fn: Function) {
        this.forEach((el, i) => { fn.call(el, i, el); });
        return this;
    }

    map(fn: any): any[] {
        return Array.prototype.map.call(this, (el: HTMLElement, i: number) => fn.call(el, i, el));
    }

    show() {
        this.forEach(el => { el.style.display = '' });
        return this;
    }

    hide() {
        this.forEach(el => { el.style.display = 'none' });
        return this;
    }

    is(selector: string): boolean {
        return this.every(x => {
            if (selector === ':input')
                return x.tagName === "INPUT" || x.tagName === "TEXTAREA" || x.tagName === "SELECT";

            x.matches(selector);
        })
    }

    remove() {
        this.forEach(el => { if (el.parentNode) { el.parentNode.removeChild(el); } });
        return this;
    }

    toggle(show: boolean) {
        if (typeof show === 'undefined') {
            this.forEach(el => { el.style.display = (el.style.display === 'none' ? '' : 'none'); });
        } else {
            show ? this.show() : this.hide();
        }
        return this;
    }

    focus() {
        this[0] && this[0].focus();
        return this;
    }

    find(a: any): any {
        const nodes: Element[] = [];
        this.forEach(el => { nodes.push(...Array.from(el.querySelectorAll(a))) });
        return SQ([...Array.from(new Set(nodes))]) as sQ;
    }

    on(eventName: string, ...args: any[]) {
        this.forEach(el => {
            el.addEventListener(eventName, typeof args[0] === 'function' ? function (e) {
                if (args[0].call(el, e) === false) {
                    e.preventDefault();
                }
            } : function (e) {
                for (let target: any = e.target; target && target !== this; target = target.parentNode) {
                    if (target.matches(args[0])) {
                        if (args[1].call(target, e) === false) {
                            e.preventDefault();
                        }
                        break;
                    }
                }
            });
        });

        return this;
    }

    children() {
        var items: HTMLElement[] = [];
        Array.prototype.forEach.call(this, (el: HTMLElement) => {
            items = items.concat(Array.from(el.children) as HTMLElement[]);
        });
        return sQ.from(Array.from(new Set(items)));
    }

    parent() {
        var items = Array.prototype.map.call(this, (el: HTMLElement) => el.parentNode as HTMLElement);
        items = Array.prototype.filter.call(items, (x: any) => x != null);
        return sQ.from(Array.from(new Set(items)));
    }

    siblings() {
        const nodes: HTMLElement[] = [];

        this.forEach(el => {
            nodes.push(...Array.prototype.filter.call((el.parentNode as HTMLElement).children, (child: any) => child !== el));
        });

        return SQ(nodes);
    }

    parents(selector: any) {
        const nodes: HTMLElement[] = [];

        this.forEach(el => {
            let parent: HTMLElement = el;
            while (parent = parent.parentNode as HTMLElement) {
                if (parent.matches(selector)) {
                    nodes.push(parent as HTMLElement);
                    break;
                }
            }
        });

        return SQ([...Array.from(new Set(nodes))]);
    }

    not(a: any): any {
        if (typeof a === 'string') {
            return SQ(Array.prototype.filter.call(this, (el: any) => !el.matches(a)));
        } else if (typeof a === 'function') {
            return SQ(Array.prototype.filter.call(this, (el: any) => !a(el)));
        } else
            return SQ(Array.prototype.filter.call(this, (el: any) => el !== a));
    }

    filter(a: any): any {
        if (typeof a === 'string') {
            return SQ(Array.prototype.filter.call(this, (el: any) => el.matches(a)));
        } else if (typeof a === 'function') {
            return SQ(Array.prototype.filter.call(this, a));
        } else
            return SQ(Array.prototype.filter.call(this, (el: any) => el === a));
    }

    serialize(): any {
        return Array.from(new FormData(this[0] as HTMLFormElement) as any, e => (e as any).map(encodeURIComponent).join('=')).join('&')
    }

    click(callback: any) {
        return this.on('click', callback);
    }

    submit(callback: any) {
        return this.on('submit', callback);
    }

    keyup(callback: any) {
        return this.on('keyup', callback);
    }

    data(key: string, value?: any) {
        var el = this[0] as any;
        if (!el)
            return;
        if (typeof value === "undefined")
            return el.__squeryData?.[key];
        el.__squeryData = el.__squeryData ?? {};
        el.__squeryData[key] = value;
    }
}

const SQ: any = function (a: any) {
    let nodes: HTMLElement[] = [];

    if (typeof a === 'string') {
        a = a.trim();
        if (!a.length) {
            return null;
        }
        if (a[0] === '<') {
            const div = document.createElement('div');
            div.insertAdjacentHTML('afterbegin', a);
            nodes = [div.firstChild as HTMLElement];
        } else {
            nodes = Array.from(document.querySelectorAll(a));
        }
    } else if (typeof a === 'function') {
        (typeof document !== "undefined" && document.readyState === 'loading') ? document.addEventListener("DOMContentLoaded", a) : a();
    } else if (a && a.tagName || a === document) {
        nodes = [a];
    }
    else if (Array.isArray(a)) {
        nodes = a;
    }
    else {
        return null;
    }

    var $ = sQ.from(nodes);
    return $;
};

SQ.ajaxSetup = function (options: any) {
    this.ajaxOptions = Object.assign({ headers: {} }, options);
};

SQ.ajax = function (params: any) {
    params = Object.assign({ method: 'get', dataType: 'html' }, params);
    const headers = Object.assign({}, this.ajaxOptions.headers);
    const isJson = params.dataType.toLowerCase() === 'json';

    if (params.method.toLowerCase() === 'post') {
        headers['Content-Type'] = 'application/x-www-form-urlencoded; charset=UTF-8';
    }
    if (isJson) {
        headers['Accept'] = 'application/json';
    }

    return new Promise(function (resolve, reject) {
        const request = new XMLHttpRequest();

        request.onload = function () {
            if (this.status >= 200 && this.status < 400) {
                resolve(isJson ? JSON.parse(this.response) : this.response);
            } else {
                if (isJson) {
                    (request as any).responseJSON = JSON.parse(request.responseText);
                }
                reject(request);
            }
        };

        request.onerror = function () {
            reject(request);
        };

        if (params['method'].toLowerCase() === 'post') {
            request.open('POST', params['url']);
            Object.keys(headers).forEach(header => { request.setRequestHeader(header, headers[header]); });
            request.send(params['data']);
        } else {
            request.open('GET',
                params['url']
                + (
                    'data' in params
                        ? '?' + Object.keys(params['data'])
                            .map(key => key + '=' + encodeURIComponent(params['data'][key]))
                            .join('&')
                        : ''
                ));
            Object.keys(headers).forEach(header => { request.setRequestHeader(header, headers[header]); });
            request.send();
        }
    });
} as any;

SQ.debounce = function (callback: any, wait: any) {
    let timeout: number;

    return function () {
        const next = function () { callback.apply(this, arguments); }
        clearTimeout(timeout);
        timeout = window.setTimeout(next, wait);
    }
} as any;

SQ.throttle = function (callback: any, limit: number) {
    let inThrottle = false;

    return function () {
        if (!inThrottle) {
            callback.apply(this, arguments);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit)
        }
    }
};

SQ.fn = sQ.prototype;

SQ.isMock = true;

declare global {
    interface JQueryStatic {
        isMock?: boolean;
    }
}

export default (jQuery || SQ as unknown as JQueryStatic);