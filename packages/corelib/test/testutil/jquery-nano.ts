function toggleClass(el: Element, cls: string, remove?: boolean) {
    if (cls == null || !cls.length)
        return;

    if (cls.indexOf(' ') >= 0) {
        var arr = cls.split(' ').map(x => x.trim()).filter(x => x.length);
        for (var a of arr)
            el.classList.toggle(a, remove);
    }
    else
        el.classList.toggle(cls, remove);
}

class $I extends Array<HTMLElement> {

    eq(i: number) {
        return $S(this[i]);
    }

    get(i: number) {
        return this[i];
    }

    ready(cb: () => void) {
        $S.ready(cb);
    }

    first() {
        return $S([this[0]]);
    }

    addClass(className: string) {
        this.forEach(el => { toggleClass(el, className, true); });
        return this;
    }

    removeClass(className: string) {
        this.forEach(el => { toggleClass(el, className, false); });
        return this;
    }

    toggleClass(className, toggle) {
        this.forEach(el => { toggleClass(el, className, toggle); });
        return this;
    }

    hasClass(className) {
        if (!this.length) {
            return false;
        }
        return !this.find(el => !el.classList.contains(className));
    }

    after(content) {
        return this.manipulate(content, 'afterend', HTMLElement.prototype.after);
    }

    before(content) {
        return this.manipulate(content, 'beforebegin', HTMLElement.prototype.before);
    }

    append(content) {
        return this.manipulate(content, 'beforeend', HTMLElement.prototype.append);
    }

    prepend(content) {
        return this.manipulate(content, 'afterbegin', HTMLElement.prototype.prepend);
    }

    manipulate(content, place, func) {
        this.forEach(el => {
            if (typeof content === "string") {
                el.insertAdjacentHTML(place, content);
            } else if (content instanceof $I) {
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

    val(value) {
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

    attr(name, value) {
        if (typeof value === 'undefined') {
            return this.length ? this[0].getAttribute(name) : undefined;
        }

        this.forEach(el => { el.setAttribute(name, value); });
        return this;
    }

    removeAttr(name) {
        this.forEach(el => { el.removeAttribute(name); });
        return this;
    }

    html(html) {
        if (typeof html === 'undefined') {
            return this.reduce((a, v) => a + v.innerHTML, '');
        }

        this.forEach(el => { el.innerHTML = html; });

        return this;
    }

    each(fn) {
        this.forEach((el, i) => { fn.call(el, i, el); });
        return this;
    }

    map(fn) {
        return this.map((el, i) => fn.call(el, i, el));
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

    toggle(show) {
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

    find(a): any {
        const nodes: HTMLElement[] = [];
        this.forEach(el => { nodes.push(...Array.from(el.querySelectorAll(a))) });
        return $S([...Array.from(new Set(nodes))]) as $I;
    }

    on(eventName, ...args: any[]) {
        this.forEach(el => {
            el.addEventListener(eventName, typeof args[0] === 'function' ? function (e) {
                if (args[0].call(el, e) === false) {
                    e.preventDefault();
                }
            } : function (e) {
                for (let target = e.target; target && target !== this; target = target.parentNode) {
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

    parent() {
        return $S([...Array.from(new Set(this.map(el => el.parentNode as HTMLElement)))]);
    }

    siblings() {
        const nodes: HTMLElement[] = [];

        this.forEach(el => {
                nodes.push(...Array.prototype.filter.call((el.parentNode as HTMLElement).children, child => child !== el));
        });

        return $S(nodes);
    }

    parents(selector) {
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

        return $S([...Array.from(new Set(nodes))]);
    }

    filter(a) {
        if (typeof a === 'string') {
            return $S(this.filter(el => el.matches(a)));
        } else if (typeof a === 'function') {
            return $S(this.filter(a));
        }
    }

    serialize() {
        return Array.from(new FormData(this[0] as HTMLFormElement) as any, e => (e as any).map(encodeURIComponent).join('=')).join('&')
    }

    click(callback) {
        return this.on('click', callback);
    }

    submit(callback) {
        return this.on('submit', callback);
    }

    keyup(callback) {
        return this.on('keyup', callback);
    }

    data(key: string, value?: any) {
        var el = this[0] as any;
        if (!el)
            return;
        if (typeof value === "undefined")
            return el._nanoData?.[key];
        el._nanoData = el._nanoData ?? {};
        el._nanoData[key] = value;
    }
}

const $S: any = function (a) {
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
        $S.ready(a);
    } else if (a && a.tagName || a === document) {
        nodes = [a];
    } else {
        return null;
    }

    var $ = $I.from(nodes);
    // workaround for subclassing array until corelib switched to ES6
    //@ts-ignore
    !$.eq && ($.__proto__ = $I.prototype);
    return $;
};

$S.ajaxSetup = function (options) {
    this.ajaxOptions = Object.assign({ headers: {} }, options);
};

$S.ajax = function (params) {
    params = Object.assign({method: 'get', dataType: 'html'}, params);
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

$S.debounce = function (callback, wait) {
    let timeout: number;

    return function() {
        const next = function() { callback.apply(this, arguments); }
        clearTimeout(timeout);
        timeout = window.setTimeout(next, wait);
    }
} as any;

$S.throttle = function (callback, limit) {
    let inThrottle = false;

    return function() {
        if (!inThrottle) {
            callback.apply(this, arguments);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit)
        }
    }
};

export default $S as unknown as JQueryStatic;