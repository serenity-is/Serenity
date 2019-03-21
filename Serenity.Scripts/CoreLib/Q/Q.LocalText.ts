
namespace Q {

    export function text(key: string): string {
        let t = LT.$table[key];
        if (t == null) {
            t = key || '';
        }
        return t;
    }

    export function dbText(prefix: string): ((key: string) => string) {
        return function (key: string) {
            return text("Db." + prefix + "." + key);
        }
    }

    export function prefixedText(prefix: string) {

        return function (text: string, key: string | ((p?: string) => string)) {

            if (text != null && !Q.startsWith(text, '`')) {
                var local = Q.tryGetText(text);
                if (local != null) {
                    return local;
                }
            }

            if (text != null && Q.startsWith(text, '`')) {
                text = text.substr(1);
            }

            if (!Q.isEmptyOrNull(prefix)) {
                var textKey = typeof (key) == "function" ? key(prefix) : (prefix + key);
                var localText = Q.tryGetText(textKey);
                if (localText != null) {
                    return localText;
                }
            }

            return text;
        }
    }

    export function tryGetText(key: string): string {
        return LT.$table[key];
    }

    export function dbTryText(prefix: string): ((key: string) => string) {
        return function (key: string) {
            return text("Db." + prefix + "." + key);
        }
    }

    export function proxyTexts(o: Object, p: string, t: Object): Object {
        if (typeof window != 'undefined' && window['Proxy']) {
            return new window['Proxy'](o, {
                get: (x: Object, y: string) => {
                    var tv = t[y];
                    if (tv == null)
                        return;
                    if (typeof tv == 'number')
                        return Q.text(p + y);
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
                        get: () => Q.text(p + k)
                    });
                else
                    o[k] = proxyTexts({}, p + k + '.', t[k]);
            }
            return o;
        }
    }

    export class LT {
        static $table: { [key: string]: string } = {};
        static empty: LT = new LT('');

        constructor(private key: string) {
        }

        static add(obj: any, pre?: string) {
            if (!obj) {
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
                    LT.$table[actual] = o;
                }
            }
        }

        get() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        toString() {
            var t = LT.$table[this.key];
            if (t == null) {
                t = this.key || '';
            }
            return t;
        }

        static initializeTextClass = function (type: any, prefix: string) {
            var $t1 = Object.keys(type).slice();
            for (var $t2 = 0; $t2 < $t1.length; $t2++) {
                var member = $t1[$t2];
                var value = type[member];
                if (value instanceof LT) {
                    var lt = value;
                    var key = prefix + member;
                    LT.$table[key] = lt.key;
                    type[member] = new LT(key);
                }
            }
        }

        static getDefault = function (key: string, defaultText: string) {
            var t = LT.$table[key];
            if (t == null) {
                t = defaultText;
                if (t == null) {
                    t = key || '';
                }
            }
            return t;
        }
    }
}