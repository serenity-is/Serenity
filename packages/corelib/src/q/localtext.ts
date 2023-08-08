import { getStateStore } from "./system";
import { isEmptyOrNull, startsWith } from "./strings";

export function localText(key: string): string {
    let t: string = getTable()[key];
    if (t == null) {
        t = key ?? '';
    }
    return t;
}

/** @obsolete prefer localText for better discoverability */
export const text = localText;

export function dbText(prefix: string): ((key: string) => string) {
    return function (key: string) {
        return localText("Db." + prefix + "." + key);
    }
}

function getTable(): { [key: string]: string } {
    return getStateStore("__localText");
}

export function prefixedText(prefix: string) {

    return function (text: string, key: string | ((p?: string) => string)) {

        if (text != null && !startsWith(text, '`')) {
            var local = tryGetText(text);
            if (local != null) {
                return local;
            }
        }

        if (text != null && startsWith(text, '`')) {
            text = text.substr(1);
        }

        if (!isEmptyOrNull(prefix)) {
            var textKey = typeof (key) == "function" ? key(prefix) : (prefix + key);
            var localText = tryGetText(textKey);
            if (localText != null) {
                return localText;
            }
        }

        return text;
    }
}

export function tryGetText(key: string): string {
    var value = getTable()[key];
    return value;
}

export function dbTryText(prefix: string): ((key: string) => string) {
    return function (key: string) {
        return localText("Db." + prefix + "." + key);
    }
}

export function proxyTexts(o: Record<string, any>, p: string, t: Record<string, any>): Object {
    if (typeof window != 'undefined' && window['Proxy']) {
        return new window['Proxy'](o, {
            get: (x: Object, y: string) => {
                var tv = t[y];
                if (tv == null)
                    return localText(p + y);
                else {
                    var z = o[y];
                    if (z != null)
                        return z;
                    o[y] = z = proxyTexts({}, p + y + '.', tv);
                    return z;
                }
            },
            ownKeys: (x: Object) => Object.keys(t)
        });
    }
    else {
        for (var k of Object.keys(t)) {
            if (typeof t[k] == 'number')
                Object.defineProperty(o, k, {
                    get: () => localText(p + k)
                });
            else
                o[k] = proxyTexts({}, p + k + '.', t[k]);
        }
        return o;
    }
}

export class LT {
    static empty: LT = new LT('');

    constructor(private key: string) {
    }

    static add(key: string, value: string): void;
    static add(obj: any, pre?: string) {
        if (!obj) {
            return;
        }
        
        let table = getTable();

        if (typeof obj === "string") {
            table[obj] = pre;
            return;
        }

        pre = pre || '';
        for (let k of Object.keys(obj)) {
            let actual = pre + k;
            let o = obj[k];
            if (typeof (o) === 'object') {
                LT.add(o, actual + '.');
            }
            else {
                table[actual] = o;
            }
        }
    }

    get() {
        var t = getTable()[this.key];
        if (t == null) {
            t = this.key || '';
        }
        return t;
    }

    toString() {
        var t = getTable()[this.key];
        if (t == null) {
            t = this.key || '';
        }
        return t;
    }

    static initializeTextClass = function (type: any, prefix: string) {
        var $t1 = Object.keys(type).slice();
        var table = getTable();
        for (var $t2 = 0; $t2 < $t1.length; $t2++) {
            var member = $t1[$t2];
            var value = type[member];
            if (value instanceof LT) {
                var lt = value;
                var key = prefix + member;
                table[key] = lt.key;
                type[member] = new LT(key);
            }
        }
    }

    static getDefault = function (key: string, defaultText: string) {
        var t = getTable()[key];
        if (t == null) {
            t = defaultText;
            if (t == null) {
                t = key || '';
            }
        }
        return t;
    }
}

if (typeof globalThis !== "undefined") {
    const Q = (globalThis as any).Q || ((globalThis as any).Q = {});
    if (Q.LT == null)
        Q.LT = LT;
}